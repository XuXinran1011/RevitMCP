using System;
using System.Collections.Generic;

namespace RevitMCP.Shared.Models
{
    /// <summary>
    /// 族元数据模型，描述Revit族的基础信息，支持序列化和跨端传输。
    /// </summary>
    public class FamilyMetadata
    {
        /// <summary>
        /// 族唯一标识符。
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// 族名称。
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 族所属类别。
        /// </summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// 族标签集合。
        /// </summary>
        public List<string> Tags { get; set; } = new();

        /// <summary>
        /// 族参数集合（参数名-参数值）。
        /// </summary>
        public Dictionary<string, object> Parameters { get; set; } = new();

        /// <summary>
        /// 族描述（可选）。
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 族预览图路径（可选）。
        /// </summary>
        public string? PreviewImagePath { get; set; }

        /// <summary>
        /// 创建人（可选）。
        /// </summary>
        public string? CreatedBy { get; set; }

        /// <summary>
        /// 最后修改时间。
        /// </summary>
        public DateTime LastModified { get; set; } = DateTime.UtcNow;
    }
} 