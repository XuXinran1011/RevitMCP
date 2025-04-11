using System;
using System.Text.Json.Serialization;

namespace RevitMCP.Shared.Communication
{
    /// <summary>
    /// MCP消息基类
    /// </summary>
    public class MCPMessage
    {
        /// <summary>
        /// 消息ID
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        /// <summary>
        /// 消息类型
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }
        
        /// <summary>
        /// 创建时间戳
        /// </summary>
        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }
    
    /// <summary>
    /// MCP请求消息
    /// </summary>
    public class MCPRequest : MCPMessage
    {
        /// <summary>
        /// 请求方法
        /// </summary>
        [JsonPropertyName("method")]
        public string Method { get; set; }
        
        /// <summary>
        /// 请求参数
        /// </summary>
        [JsonPropertyName("params")]
        public object Params { get; set; }
        
        /// <summary>
        /// 初始化MCP请求消息
        /// </summary>
        public MCPRequest()
        {
            Type = "request";
        }
    }
    
    /// <summary>
    /// MCP响应消息
    /// </summary>
    public class MCPResponse : MCPMessage
    {
        /// <summary>
        /// 响应结果
        /// </summary>
        [JsonPropertyName("result")]
        public object Result { get; set; }
        
        /// <summary>
        /// 错误信息
        /// </summary>
        [JsonPropertyName("error")]
        public MCPError Error { get; set; }
        
        /// <summary>
        /// 初始化MCP响应消息
        /// </summary>
        public MCPResponse()
        {
            Type = "response";
        }
    }
    
    /// <summary>
    /// MCP错误信息
    /// </summary>
    public class MCPError
    {
        /// <summary>
        /// 错误代码
        /// </summary>
        [JsonPropertyName("code")]
        public int Code { get; set; }
        
        /// <summary>
        /// 错误消息
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; set; }
        
        /// <summary>
        /// 错误数据
        /// </summary>
        [JsonPropertyName("data")]
        public object Data { get; set; }
    }
}
