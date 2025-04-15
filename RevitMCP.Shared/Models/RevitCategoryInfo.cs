using System;

namespace RevitMCP.Shared.Models
{
    /// <summary>
    /// 表示Revit类别的基础信息，适用于跨进程通信和序列化。
    /// </summary>
    public class RevitCategoryInfo
    {
        /// <summary>
        /// 类别唯一标识符。
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// 类别名称。
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 父类别Id（如有）。
        /// </summary>
        public string? ParentId { get; set; }
    }
} 