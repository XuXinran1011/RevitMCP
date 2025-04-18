using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RevitMCP.Shared.Models
{
    /// <summary>
    /// 族元数据领域模型，包含族的基本信息、参数定义、描述、预览图、创建者、最后修改时间等。
    /// </summary>
    public class FamilyMetadata
    {
        /// <summary>族唯一标识</summary>
        public string Id { get; }
        /// <summary>族名称</summary>
        public string Name { get; }
        /// <summary>类别（如Wall、Door等）</summary>
        public string Category { get; }
        /// <summary>参数定义表（参数名-参数定义）</summary>
        public IDictionary<string, Parameter> Parameters { get; }
        /// <summary>族描述</summary>
        public string? Description { get; }
        /// <summary>标签集合</summary>
        public IEnumerable<string> Tags { get; }
        /// <summary>预览图路径</summary>
        public string? PreviewImagePath { get; }
        /// <summary>创建者</summary>
        public string? CreatedBy { get; }
        /// <summary>最后更新时间</summary>
        public DateTime LastModified { get; }

        /// <summary>
        /// 构造函数，初始化所有族元数据属性。
        /// </summary>
        public FamilyMetadata(
            string id,
            string name,
            string category,
            IEnumerable<string> tags,
            IDictionary<string, Parameter> parameters,
            string? description,
            string? previewImagePath,
            string? createdBy,
            DateTime lastModified)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentException("Id不能为空", nameof(id));
            if (string.IsNullOrEmpty(name)) throw new ArgumentException("Name不能为空", nameof(name));
            if (string.IsNullOrEmpty(category)) throw new ArgumentException("Category不能为空", nameof(category));

            Id = id;
            Name = name;
            Category = category;
            Tags = tags ?? new List<string>();
            Parameters = parameters ?? new Dictionary<string, Parameter>();
            Description = description;
            PreviewImagePath = previewImagePath;
            CreatedBy = createdBy;
            LastModified = lastModified;
        }
    }
} 