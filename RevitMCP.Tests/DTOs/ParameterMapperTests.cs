using System;
using RevitMCP.Shared.DTOs;
using RevitMCP.Shared.Models;
using Xunit;

namespace RevitMCP.Tests.DTOs
{
    public class ParameterMapperTests
    {
        [Fact]
        public void Model_ToDTO_And_Back_Should_Be_Equivalent()
        {
            var model = new Parameter("Length", "number", "mm", true, "长度", 1000);
            var dto = ParameterMapper.ToDTO(model);
            var model2 = ParameterMapper.ToModel(dto);

            Assert.Equal(model.Name, model2.Name);
            Assert.Equal(model.Type, model2.Type);
            Assert.Equal(model.Unit, model2.Unit);
            Assert.Equal(model.Required, model2.Required);
            Assert.Equal(model.Description, model2.Description);
            Assert.Equal(model.DefaultValue, model2.DefaultValue);
        }

        [Fact]
        public void DTO_ToModel_And_Back_Should_Be_Equivalent()
        {
            var dto = new ParameterDTO
            {
                Name = "Height",
                Type = "number",
                Unit = "mm",
                Required = true,
                Description = "高度",
                DefaultValue = 3000
            };
            var model = ParameterMapper.ToModel(dto);
            var dto2 = ParameterMapper.ToDTO(model);

            Assert.Equal(dto.Name, dto2.Name);
            Assert.Equal(dto.Type, dto2.Type);
            Assert.Equal(dto.Unit, dto2.Unit);
            Assert.Equal(dto.Required, dto2.Required);
            Assert.Equal(dto.Description, dto2.Description);
            Assert.Equal(dto.DefaultValue, dto2.DefaultValue);
        }

        [Fact]
        public void Mapper_Should_Handle_Null_And_Empty_Values()
        {
            var dto = new ParameterDTO
            {
                Name = "",
                Type = "",
                Unit = null,
                Required = false,
                Description = null,
                DefaultValue = null
            };
            var model = ParameterMapper.ToModel(dto);
            Assert.Equal("", model.Name);
            Assert.Equal("", model.Type);
            Assert.Equal("", model.Unit);
            Assert.False(model.Required);
            Assert.Equal("", model.Description);
            Assert.Null(model.DefaultValue);

            var dto2 = ParameterMapper.ToDTO(model);
            Assert.Equal("", dto2.Name);
            Assert.Equal("", dto2.Type);
            Assert.Null(dto2.Unit); // 保持序列化友好
            Assert.False(dto2.Required);
            Assert.Equal("", dto2.Description);
            Assert.Null(dto2.DefaultValue);
        }

        [Fact]
        public void ToModel_Should_Throw_On_Null_DTO()
        {
            Assert.Throws<ArgumentNullException>(() => ParameterMapper.ToModel(null!));
        }

        [Fact]
        public void ToDTO_Should_Throw_On_Null_Model()
        {
            Assert.Throws<ArgumentNullException>(() => ParameterMapper.ToDTO(null!));
        }
    }
} 