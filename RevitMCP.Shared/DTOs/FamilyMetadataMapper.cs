using System;
using System.Collections.Generic;
using System.Linq;
using RevitMCP.Shared.Models;

namespace RevitMCP.Shared.DTOs
{
    /// <summary>
    /// 族元数据与DTO的映射工具，支持领域模型与DTO的双向转换。
    /// </summary>
    public static class FamilyMetadataMapper
    {
        /// <summary>
        /// 领域模型转DTO。
        /// </summary>
        public static FamilyMetadataDTO ToDTO(FamilyMetadata model)
        {
            return new FamilyMetadataDTO
            {
                Id = model.Id,
                Name = model.Name,
                Category = model.Category,
                Tags = model.Tags?.ToList() ?? new List<string>(),
                Parameters = model.Parameters?.Values.Select(ParameterMapper.ToDTO).ToList() ?? new List<ParameterDTO>(),
                Description = model.Description,
                PreviewImagePath = model.PreviewImagePath,
                CreatedBy = model.CreatedBy,
                LastModified = model.LastModified
            };
        }

        /// <summary>
        /// DTO转领域模型。
        /// </summary>
        public static FamilyMetadata ToModel(FamilyMetadataDTO dto)
        {
            var paramDict = dto.Parameters?.ToDictionary(
                p => p.Name,
                p => ParameterMapper.ToModel(p)
            ) ?? new Dictionary<string, Parameter>();
            return new FamilyMetadata(
                dto.Id,
                dto.Name,
                dto.Category,
                dto.Tags ?? new List<string>(),
                paramDict,
                dto.Description ?? string.Empty,
                dto.PreviewImagePath ?? string.Empty,
                dto.CreatedBy ?? string.Empty,
                dto.LastModified
            );
        }
    }

    /// <summary>
    /// 族参数与DTO的映射工具。
    /// </summary>
    public static class ParameterMapper
    {
        /// <summary>
        /// 领域模型转DTO。
        /// </summary>
        public static ParameterDTO ToDTO(Parameter model)
        {
            return new ParameterDTO
            {
                Name = model.Name,
                Type = model.Type,
                Unit = model.Unit,
                Required = model.Required,
                Description = model.Description,
                DefaultValue = model.DefaultValue
            };
        }

        /// <summary>
        /// DTO转领域模型。
        /// </summary>
        public static Parameter ToModel(ParameterDTO dto)
        {
            return new Parameter(
                dto.Name,
                dto.Type,
                dto.Unit ?? string.Empty,
                dto.Required,
                dto.Description ?? string.Empty,
                dto.DefaultValue
            );
        }
    }
} 