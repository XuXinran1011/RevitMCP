using System;

namespace RevitMCP.Shared.Models
{
    /// <summary>
    /// 通用查询消息模型，适用于跨进程通信的标准查询格式。
    /// </summary>
    public class QueryMessage
    {
        /// <summary>
        /// 查询类型（如元素查询、参数查询等）。
        /// </summary>
        public string QueryType { get; set; } = string.Empty;

        /// <summary>
        /// 查询负载（参数、条件等，序列化为JSON）。
        /// </summary>
        public object? Payload { get; set; }

        /// <summary>
        /// 请求唯一标识符。
        /// </summary>
        public string RequestId { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// 发送方标识（可选）。
        /// </summary>
        public string? Sender { get; set; }

        /// <summary>
        /// 查询时间戳。
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
} 