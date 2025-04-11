using System;
using System.Threading.Tasks;
using RevitMCP.Server.Application.Services;
using RevitMCP.Shared.Models;

namespace RevitMCP.Server.Application.Commands
{
    /// <summary>
    /// 获取元素命令
    /// </summary>
    public class GetElementCommand
    {
        private readonly MCPToolService _toolService;
        
        /// <summary>
        /// 初始化获取元素命令
        /// </summary>
        /// <param name="toolService">MCP工具服务</param>
        public GetElementCommand(MCPToolService toolService)
        {
            _toolService = toolService ?? throw new ArgumentNullException(nameof(toolService));
        }
        
        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="elementId">元素ID</param>
        /// <returns>元素信息</returns>
        public async Task<RevitElementInfo> ExecuteAsync(int elementId)
        {
            return await _toolService.GetElementAsync(elementId);
        }
    }
}
