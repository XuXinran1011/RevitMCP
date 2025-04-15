using System.Collections.Generic;

namespace RevitMCP.Shared.Communication
{
    /// <summary>
    /// 查询消息，表示自然语言或结构化查询请求。
    /// </summary>
    public class QueryMessage : MCPMessage
    {
        /// <summary>
        /// 查询文本（如自然语言指令）
        /// </summary>
        public string QueryText { get; set; } = string.Empty;

        /// <summary>
        /// 可选参数（如元素类型、尺寸等）
        /// </summary>
        public Dictionary<string, object> Parameters { get; set; } = new();
    }
} 