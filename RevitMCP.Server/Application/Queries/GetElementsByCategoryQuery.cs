using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RevitMCP.Server.Application.Services;
using RevitMCP.Shared.Models;

namespace RevitMCP.Server.Application.Queries
{
    /// <summary>
    /// 按类别获取元素查询
    /// </summary>
    public class GetElementsByCategoryQuery
    {
        private readonly MCPToolService _toolService;
        
        /// <summary>
        /// 初始化按类别获取元素查询
        /// </summary>
        /// <param name="toolService">MCP工具服务</param>
        public GetElementsByCategoryQuery(MCPToolService toolService)
        {
            _toolService = toolService ?? throw new ArgumentNullException(nameof(toolService));
        }
        
        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="category">类别</param>
        /// <returns>元素信息列表</returns>
        public async Task<List<RevitElementInfo>> ExecuteAsync(string category)
        {
            return await _toolService.GetElementsByCategoryAsync(category);
        }
    }
}
