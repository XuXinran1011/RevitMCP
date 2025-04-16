using System;
using System.Collections.Generic;

namespace RevitMCP.Shared.Models
{
    /// <summary>
    /// Revit元素信息模型，描述单个元素的基础属性，支持序列化和跨端传输。
    /// </summary>
    public class RevitElementInfo
    {
        /// <summary>
        /// 元素唯一标识符。
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// 元素名称。
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 元素类别信息。
        /// </summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// 元素参数集合。
        /// </summary>
        public List<RevitParameterInfo> Parameters { get; set; } = new();

        /// <summary>
        /// 元素几何信息（可选，序列化格式）。
        /// </summary>
        public string? Geometry { get; set; }

        /// <summary>
        /// 元素描述（可选）。
        /// </summary>
        public string? Description { get; set; }
    }
} 