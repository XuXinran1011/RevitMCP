namespace RevitMCP.Shared.Interfaces.Server
{
    /// <summary>
    /// MCP服务器通信接口，定义跨进程消息交互的基础契约。
    /// </summary>
    public interface IMCPServerCommunication
    {
        /// <summary>
        /// 发送查询消息并异步获取响应。
        /// </summary>
        /// <param name="queryMessage">查询消息对象</param>
        /// <returns>响应消息对象</returns>
        Task<ResponseMessage> SendQueryAsync(QueryMessage queryMessage);

        /// <summary>
        /// 检查服务器是否运行中。
        /// </summary>
        /// <returns>服务器运行状态</returns>
        Task<bool> IsServerRunningAsync();
    }
} 