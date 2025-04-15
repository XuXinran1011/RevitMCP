using System.Threading.Tasks;
using RevitMCP.Shared.Communication;

namespace RevitMCP.Shared.Interfaces
{
    /// <summary>
    /// MCP服务器通信接口，定义Plugin与Server的进程间通信契约。
    /// </summary>
    public interface IMCPServerCommunication
    {
        /// <summary>
        /// 发送查询消息，异步获取响应。
        /// </summary>
        /// <param name="message">查询消息</param>
        /// <returns>响应消息</returns>
        Task<ResponseMessage> SendQueryAsync(QueryMessage message);

        /// <summary>
        /// 检查服务器是否运行中。
        /// </summary>
        /// <returns>是否运行</returns>
        Task<bool> IsServerRunningAsync();
    }
}
