using System;
using System.Collections.Generic;

namespace RevitMCP.Shared.Models
{
    /// <summary>
    /// Revit元素信息模型
    /// </summary>
    public class RevitElementInfo
    {
        /// <summary>
        /// 元素ID
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// 元素名称
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// 元素类别
        /// </summary>
        public string Category { get; set; }
        
        /// <summary>
        /// 元素类型ID
        /// </summary>
        public int TypeId { get; set; }
        
        /// <summary>
        /// 元素类型名称
        /// </summary>
        public string TypeName { get; set; }
        
        /// <summary>
        /// 元素参数
        /// </summary>
        public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
    }
}
