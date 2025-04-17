using System.Threading.Tasks;
using Xunit;
using RevitMCP.Shared.Models;

namespace RevitMCP.IntegrationTests
{
    /// <summary>
    /// 集成测试：族库管理与检索
    /// </summary>
    public class FamilyLibraryTests
    {
        [Fact(DisplayName = "添加并查询族应成功")]
        public async Task AddAndQueryFamily_Success()
        {
            // Arrange
            var client = new MCPTestClient();
            var family = new FamilyMetadata { Id = "fam-001", Name = "TestFamily", Category = "Doors" };

            // Act
            var addResponse = await client.SendQueryAsync(new QueryMessage
            {
                QueryType = "AddFamily",
                Payload = family
            });

            // Assert
            Assert.True(addResponse.Success);

            // 查询
            var queryResponse = await client.SendQueryAsync(new QueryMessage
            {
                QueryType = "GetFamilyById",
                Payload = new { FamilyId = "fam-001" }
            });
            Assert.True(queryResponse.Success);
            Assert.Equal("TestFamily", ((FamilyMetadata)queryResponse.Data).Name);
        }
    }
} 