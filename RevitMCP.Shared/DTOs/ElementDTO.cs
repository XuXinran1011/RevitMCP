using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RevitMCP.Shared.DTOs
{
    /// <summary>
    /// 元素数据传输对象
    /// </summary>
    public class ElementDTO
    {
        /// <summary>
        /// 元素ID
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        /// <summary>
        /// 元素名称
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        /// <summary>
        /// 元素类别
        /// </summary>
        [JsonPropertyName("category")]
        public string Category { get; set; }
        
        /// <summary>
        /// 元素类型ID
        /// </summary>
        [JsonPropertyName("typeId")]
        public int TypeId { get; set; }
        
        /// <summary>
        /// 元素类型名称
        /// </summary>
        [JsonPropertyName("typeName")]
        public string TypeName { get; set; }
        
        /// <summary>
        /// 元素参数
        /// </summary>
        [JsonPropertyName("parameters")]
        public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
    }
}
