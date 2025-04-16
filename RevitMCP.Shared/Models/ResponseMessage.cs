namespace RevitMCP.Shared.Models
{
    /// <summary>
    /// 通用响应消息模型，适用于跨进程通信的标准响应格式。
    /// </summary>
    public class ResponseMessage
    {
        /// <summary>
        /// 响应是否成功。
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 响应消息文本。
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// 响应数据对象（可选，序列化为JSON）。
        /// </summary>
        public object? Data { get; set; }

        /// <summary>
        /// 错误码（可选）。
        /// </summary>
        public string? ErrorCode { get; set; }

        /// <summary>
        /// 详细信息（可选）。
        /// </summary>
        public string? Details { get; set; }
    }
} 