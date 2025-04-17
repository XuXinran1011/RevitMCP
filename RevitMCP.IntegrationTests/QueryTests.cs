using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using RevitMCP.Shared.Models;

namespace RevitMCP.IntegrationTests
{
    /// <summary>
    /// 集成测试：自然语言查询与元素检索
    /// </summary>
    public class QueryTests
    {
        [Fact(DisplayName = "按类别查询元素应返回正确结果")]
        public async Task QueryElements_ByCategory_ReturnsExpectedElements()
        {
            // Arrange
            var client = new MCPTestClient(); // 需实现跨进程通信的测试客户端
            var query = new QueryMessage
            {
                QueryType = "GetElementsByCategory",
                Payload = new { Category = "Walls" }
            };

            // Act
            var response = await client.SendQueryAsync(query);

            // Assert
            Assert.True(response.Success);
            var elements = response.Data as List<RevitElementInfo>;
            Assert.NotNull(elements);
            Assert.All(elements, e => Assert.Equal("Walls", e.Category));
        }
    }
} 