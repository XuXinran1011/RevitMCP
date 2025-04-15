// 通用MCP消息基类
namespace RevitMCP.Shared.Communication
{
    /// <summary>
    /// MCP消息基类，所有消息类型应继承此类。
    /// </summary>
    public abstract class MCPMessage
    {
        /// <summary>
        /// 消息类型（如Query、Response等）
        /// </summary>
        public string MessageType { get; set; }

        /// <summary>
        /// 请求唯一标识符，用于请求-响应关联
        /// </summary>
        public string RequestId { get; set; }
    }
} 