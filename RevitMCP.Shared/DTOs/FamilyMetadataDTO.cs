using System;
using System.Collections.Generic;

namespace RevitMCP.Shared.DTOs
{
    /// <summary>
    /// 族元数据传输对象（DTO），用于跨进程或网络传输族基本信息。
    /// </summary>
    public class FamilyMetadataDTO
    {
        /// <summary>族唯一标识</summary>
        public string Id { get; set; } = string.Empty;
        /// <summary>族名称</summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>族类别</summary>
        public string Category { get; set; } = string.Empty;
        /// <summary>标签集合</summary>
        public List<string> Tags { get; set; } = new();
        /// <summary>参数集合</summary>
        public List<ParameterDTO> Parameters { get; set; } = new();
        /// <summary>族描述</summary>
        public string? Description { get; set; }
        /// <summary>预览图片路径</summary>
        public string? PreviewImagePath { get; set; }
        /// <summary>创建者</summary>
        public string? CreatedBy { get; set; }
        /// <summary>最后修改时间</summary>
        public DateTime LastModified { get; set; }
    }
} 