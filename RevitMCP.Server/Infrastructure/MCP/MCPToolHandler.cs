using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using RevitMCP.Server.Application.Commands;
using RevitMCP.Server.Application.Queries;
using RevitMCP.Server.Application.Services;
using RevitMCP.Shared.Communication;

namespace RevitMCP.Server.Infrastructure.MCP
{
    /// <summary>
    /// MCP工具处理器
    /// </summary>
    public class MCPToolHandler
    {
        private readonly MCPToolService _toolService;
        private readonly GetElementCommand _getElementCommand;
        private readonly GetElementsByCategoryQuery _getElementsByCategoryQuery;
        
        /// <summary>
        /// 初始化MCP工具处理器
        /// </summary>
        public MCPToolHandler()
        {
            _toolService = new MCPToolService();
            _getElementCommand = new GetElementCommand(_toolService);
            _getElementsByCategoryQuery = new GetElementsByCategoryQuery(_toolService);
        }
        
        /// <summary>
        /// 处理MCP请求
        /// </summary>
        /// <param name="request">MCP请求</param>
        /// <returns>MCP响应</returns>
        public async Task<MCPResponse> HandleRequestAsync(MCPRequest request)
        {
            MCPResponse response = new MCPResponse();
            
            try
            {
                switch (request.Method)
                {
                    case "getElement":
                        response.Result = await HandleGetElementAsync(request.Params);
                        break;
                    case "getElementsByCategory":
                        response.Result = await HandleGetElementsByCategoryAsync(request.Params);
                        break;
                    case "modifyElementParameter":
                        response.Result = await HandleModifyElementParameterAsync(request.Params);
                        break;
                    default:
                        response.Error = new MCPError
                        {
                            Code = 404,
                            Message = $"未知方法: {request.Method}"
                        };
                        break;
                }
            }
            catch (Exception ex)
            {
                response.Error = new MCPError
                {
                    Code = 500,
                    Message = $"处理请求时发生错误: {ex.Message}"
                };
            }
            
            return response;
        }
        
        /// <summary>
        /// 处理获取元素请求
        /// </summary>
        /// <param name="parameters">请求参数</param>
        /// <returns>元素信息</returns>
        private async Task<object> HandleGetElementAsync(object parameters)
        {
            // 解析参数
            JsonElement paramsElement = (JsonElement)parameters;
            int elementId = paramsElement.GetProperty("elementId").GetInt32();
            
            // 执行命令
            return await _getElementCommand.ExecuteAsync(elementId);
        }
        
        /// <summary>
        /// 处理按类别获取元素请求
        /// </summary>
        /// <param name="parameters">请求参数</param>
        /// <returns>元素信息列表</returns>
        private async Task<object> HandleGetElementsByCategoryAsync(object parameters)
        {
            // 解析参数
            JsonElement paramsElement = (JsonElement)parameters;
            string category = paramsElement.GetProperty("category").GetString();
            
            // 执行查询
            return await _getElementsByCategoryQuery.ExecuteAsync(category);
        }
        
        /// <summary>
        /// 处理修改元素参数请求
        /// </summary>
        /// <param name="parameters">请求参数</param>
        /// <returns>是否修改成功</returns>
        private async Task<object> HandleModifyElementParameterAsync(object parameters)
        {
            // 解析参数
            JsonElement paramsElement = (JsonElement)parameters;
            int elementId = paramsElement.GetProperty("elementId").GetInt32();
            string parameterName = paramsElement.GetProperty("parameterName").GetString();
            JsonElement parameterValue = paramsElement.GetProperty("parameterValue");
            
            // 根据参数类型转换值
            object value;
            switch (parameterValue.ValueKind)
            {
                case JsonValueKind.Number:
                    if (parameterValue.TryGetInt32(out int intValue))
                    {
                        value = intValue;
                    }
                    else
                    {
                        value = parameterValue.GetDouble();
                    }
                    break;
                case JsonValueKind.String:
                    value = parameterValue.GetString();
                    break;
                case JsonValueKind.True:
                case JsonValueKind.False:
                    value = parameterValue.GetBoolean();
                    break;
                default:
                    value = parameterValue.GetRawText();
                    break;
            }
            
            // 执行命令
            return await _toolService.ModifyElementParameterAsync(elementId, parameterName, value);
        }
    }
}
