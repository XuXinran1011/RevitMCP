namespace RevitMCP.Shared.Communication
{
    /// <summary>
    /// 响应消息，表示对查询或命令的结果反馈。
    /// </summary>
    public class ResponseMessage : MCPMessage
    {
        /// <summary>
        /// 操作是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 结果消息（如错误、提示等）
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// 结果数据（可为任意对象，序列化为JSON）
        /// </summary>
        public object? Data { get; set; }
    }
} 