using System;
using System.Collections.Generic;
using RevitMCP.Shared.DTOs;
using RevitMCP.Shared.Models;
using Xunit;

namespace RevitMCP.Tests.DTOs
{
    public class FamilyMetadataMapperTests
    {
        [Fact]
        public void Model_ToDTO_And_Back_Should_Be_Equivalent()
        {
            // 构造领域模型
            var model = new FamilyMetadata(
                "F001",
                "测试族",
                "结构",
                new List<string> { "标签1", "标签2" },
                new Dictionary<string, Parameter> { { "Height", new Parameter("Height", "number", "mm", true, "高度", 3000) } },
                "描述",
                null,
                null,
                DateTime.Now
            );

            // 领域模型→DTO
            var dto = FamilyMetadataMapper.ToDTO(model);
            // DTO→领域模型
            var model2 = FamilyMetadataMapper.ToModel(dto);

            // 验证主要属性一致
            Assert.Equal(model.Id, model2.Id);
            Assert.Equal(model.Name, model2.Name);
            Assert.Equal(model.Category, model2.Category);
            Assert.Equal(model.Tags, model2.Tags);
            Assert.Equal(model.Description, model2.Description);
            Assert.Equal(model.PreviewImagePath, model2.PreviewImagePath);
            Assert.Equal(model.CreatedBy, model2.CreatedBy);
            Assert.Equal(model.LastModified, model2.LastModified);
            Assert.Equal(model.Parameters.Count, model2.Parameters.Count);
            Assert.Equal(model.Parameters["Height"].Name, model2.Parameters["Height"].Name);
        }

        [Fact]
        public void DTO_ToModel_And_Back_Should_Be_Equivalent()
        {
            // 构造DTO
            var dto = new FamilyMetadataDTO
            {
                Id = "F002",
                Name = "空参数族",
                Category = "结构",
                Tags = new List<string>(),
                Parameters = new List<ParameterDTO>(),
                Description = null,
                PreviewImagePath = null,
                CreatedBy = null,
                LastModified = DateTime.Now
            };

            // DTO→领域模型
            var model = FamilyMetadataMapper.ToModel(dto);
            // 领域模型→DTO
            var dto2 = FamilyMetadataMapper.ToDTO(model);

            // 验证主要属性一致
            Assert.Equal(dto.Id, dto2.Id);
            Assert.Equal(dto.Name, dto2.Name);
            Assert.Equal(dto.Category, dto2.Category);
            Assert.Equal(dto.Tags, dto2.Tags);
            Assert.Equal(dto.Description, dto2.Description);
            Assert.Equal(dto.PreviewImagePath, dto2.PreviewImagePath);
            Assert.Equal(dto.CreatedBy, dto2.CreatedBy);
            Assert.Equal(dto.LastModified, dto2.LastModified);
            Assert.Equal(dto.Parameters.Count, dto2.Parameters.Count);
        }

        [Fact]
        public void Mapper_Should_Handle_Empty_And_Null_Values()
        {
            var model = new FamilyMetadata(
                "F003",
                "空标签族",
                "结构",
                null,
                new Dictionary<string, Parameter>(),
                null,
                null,
                null,
                DateTime.Now
            );
            var dto = FamilyMetadataMapper.ToDTO(model);
            Assert.NotNull(dto.Tags);
            Assert.NotNull(dto.Parameters);
            Assert.Null(dto.Description);
            Assert.Null(dto.PreviewImagePath);
            Assert.Null(dto.CreatedBy);

            var model2 = FamilyMetadataMapper.ToModel(dto);
            Assert.NotNull(model2.Tags);
            Assert.NotNull(model2.Parameters);
        }

        [Fact]
        public void ToDTO_Should_Throw_On_Null_Model()
        {
            Assert.Throws<ArgumentNullException>(() => FamilyMetadataMapper.ToDTO(null!));
        }

        [Fact]
        public void ToModel_Should_Throw_On_Null_DTO()
        {
            Assert.Throws<ArgumentNullException>(() => FamilyMetadataMapper.ToModel(null!));
        }

        [Fact]
        public void Mapper_Should_Handle_Empty_Strings_And_Collections()
        {
            var model = new FamilyMetadata(
                string.Empty,
                string.Empty,
                string.Empty,
                new List<string>(),
                new Dictionary<string, Parameter>(),
                string.Empty,
                string.Empty,
                string.Empty,
                DateTime.MinValue
            );
            var dto = FamilyMetadataMapper.ToDTO(model);
            Assert.Equal(string.Empty, dto.Id);
            Assert.Equal(string.Empty, dto.Name);
            Assert.Equal(string.Empty, dto.Category);
            Assert.Empty(dto.Tags);
            Assert.Empty(dto.Parameters);
            Assert.Equal(string.Empty, dto.Description);
            Assert.Equal(string.Empty, dto.PreviewImagePath);
            Assert.Equal(string.Empty, dto.CreatedBy);
        }

        [Fact]
        public void Batch_DTO_ToModel_And_Back_Should_Be_Equivalent()
        {
            var dtoList = new List<FamilyMetadataDTO>
            {
                new FamilyMetadataDTO
                {
                    Id = "F100",
                    Name = "族A",
                    Category = "结构",
                    Tags = new List<string> { "A" },
                    Parameters = new List<ParameterDTO>(),
                    Description = null,
                    PreviewImagePath = null,
                    CreatedBy = null,
                    LastModified = DateTime.Now
                },
                new FamilyMetadataDTO
                {
                    Id = "F200",
                    Name = "族B",
                    Category = "结构",
                    Tags = new List<string> { "B" },
                    Parameters = new List<ParameterDTO>(),
                    Description = null,
                    PreviewImagePath = null,
                    CreatedBy = null,
                    LastModified = DateTime.Now
                }
            };
            // 批量DTO→模型
            var modelList = new List<FamilyMetadata>();
            foreach (var dto in dtoList)
                modelList.Add(FamilyMetadataMapper.ToModel(dto));
            // 批量模型→DTO
            var dtoList2 = new List<FamilyMetadataDTO>();
            foreach (var model in modelList)
                dtoList2.Add(FamilyMetadataMapper.ToDTO(model));
            Assert.Equal(dtoList.Count, dtoList2.Count);
            for (int i = 0; i < dtoList.Count; i++)
            {
                Assert.Equal(dtoList[i].Id, dtoList2[i].Id);
                Assert.Equal(dtoList[i].Name, dtoList2[i].Name);
                Assert.Equal(dtoList[i].Category, dtoList2[i].Category);
                Assert.Equal(dtoList[i].Tags, dtoList2[i].Tags);
                Assert.Equal(dtoList[i].Description, dtoList2[i].Description);
                Assert.Equal(dtoList[i].PreviewImagePath, dtoList2[i].PreviewImagePath);
                Assert.Equal(dtoList[i].CreatedBy, dtoList2[i].CreatedBy);
                Assert.Equal(dtoList[i].LastModified, dtoList2[i].LastModified);
                Assert.Equal(dtoList[i].Parameters.Count, dtoList2[i].Parameters.Count);
            }
        }

        [Fact]
        public void Batch_Model_ToDTO_And_Back_Should_Be_Equivalent()
        {
            var modelList = new List<FamilyMetadata>
            {
                new FamilyMetadata(
                    "F100",
                    "族A",
                    "结构",
                    new List<string> { "A" },
                    new Dictionary<string, Parameter>(),
                    null,
                    null,
                    null,
                    DateTime.Now
                ),
                new FamilyMetadata(
                    "F200",
                    "族B",
                    "结构",
                    new List<string> { "B" },
                    new Dictionary<string, Parameter>(),
                    null,
                    null,
                    null,
                    DateTime.Now
                )
            };
            // 批量模型→DTO
            var dtoList = new List<FamilyMetadataDTO>();
            foreach (var model in modelList)
                dtoList.Add(FamilyMetadataMapper.ToDTO(model));
            // 批量DTO→模型
            var modelList2 = new List<FamilyMetadata>();
            foreach (var dto in dtoList)
                modelList2.Add(FamilyMetadataMapper.ToModel(dto));
            Assert.Equal(modelList.Count, modelList2.Count);
            for (int i = 0; i < modelList.Count; i++)
            {
                Assert.Equal(modelList[i].Id, modelList2[i].Id);
                Assert.Equal(modelList[i].Name, modelList2[i].Name);
                Assert.Equal(modelList[i].Category, modelList2[i].Category);
                Assert.Equal(modelList[i].Tags, modelList2[i].Tags);
                Assert.Equal(modelList[i].Description, modelList2[i].Description);
                Assert.Equal(modelList[i].PreviewImagePath, modelList2[i].PreviewImagePath);
                Assert.Equal(modelList[i].CreatedBy, modelList2[i].CreatedBy);
                Assert.Equal(modelList[i].LastModified, modelList2[i].LastModified);
                Assert.Equal(modelList[i].Parameters.Count, modelList2[i].Parameters.Count);
            }
        }

        [Fact]
        public void Batch_Empty_And_Single_Element_Collections_Should_Work()
        {
            // 空集合
            var emptyDtoList = new List<FamilyMetadataDTO>();
            var emptyModelList = new List<FamilyMetadata>();
            Assert.Empty(emptyDtoList);
            Assert.Empty(emptyModelList);
            // 单元素集合
            var singleDtoList = new List<FamilyMetadataDTO>
            {
                new FamilyMetadataDTO { Id = "F300", Name = "单族", Category = "测试", Tags = new List<string>(), Parameters = new List<ParameterDTO>(), LastModified = DateTime.Now }
            };
            var singleModelList = new List<FamilyMetadata>
            {
                new FamilyMetadata(
                    "F301",
                    "单族模型",
                    "测试",
                    new List<string>(),
                    new Dictionary<string, Parameter>(),
                    "",
                    "",
                    "",
                    DateTime.Now
                )
            };
            // 转换不应抛异常
            foreach (var dto in singleDtoList)
                FamilyMetadataMapper.ToModel(dto);
            foreach (var model in singleModelList)
                FamilyMetadataMapper.ToDTO(model);
        }
    }
} 