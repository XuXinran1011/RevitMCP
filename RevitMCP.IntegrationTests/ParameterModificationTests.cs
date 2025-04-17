using System.Linq;
using System.Threading.Tasks;
using Xunit;
using RevitMCP.Shared.Models;

namespace RevitMCP.IntegrationTests
{
    /// <summary>
    /// 集成测试：参数修改与同步
    /// </summary>
    public class ParameterModificationTests
    {
        [Fact(DisplayName = "修改元素参数后应正确同步")]
        public async Task ModifyElementParameter_UpdatesValue()
        {
            // Arrange
            var client = new MCPTestClient();
            var elementId = "test-wall-001";
            var modifyCommand = new QueryMessage
            {
                QueryType = "ModifyElementParameter",
                Payload = new { ElementId = elementId, Parameter = "Height", Value = 4000 }
            };

            // Act
            var response = await client.SendQueryAsync(modifyCommand);

            // Assert
            Assert.True(response.Success);

            // 再次查询验证
            var query = new QueryMessage
            {
                QueryType = "GetElementById",
                Payload = new { ElementId = elementId }
            };
            var queryResponse = await client.SendQueryAsync(query);
            var element = queryResponse.Data as RevitElementInfo;
            Assert.NotNull(element);
            Assert.Equal(4000, element.Parameters.First(p => p.Name == "Height").Value);
        }
    }
} 