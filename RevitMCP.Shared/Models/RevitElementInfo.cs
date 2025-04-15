using System;
using System.Collections.Generic;

namespace RevitMCP.Shared.Models
{
    /// <summary>
    /// 表示Revit元素的基础信息，适用于跨进程通信和序列化。
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
        /// 元素类别。
        /// </summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// 元素参数信息集合。
        /// </summary>
        public List<RevitParameterInfo> Parameters { get; set; } = new();
    }
} 