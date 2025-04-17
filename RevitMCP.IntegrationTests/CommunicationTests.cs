using System.Threading.Tasks;
using Xunit;
using RevitMCP.Shared.Models;

namespace RevitMCP.IntegrationTests
{
    /// <summary>
    /// 集成测试：跨进程通信异常处理
    /// </summary>
    public class CommunicationTests
    {
        [Fact(DisplayName = "发送无效命令应返回错误响应")]
        public async Task SendInvalidMessage_ReturnsError()
        {
            // Arrange
            var client = new MCPTestClient();
            var invalidQuery = new QueryMessage
            {
                QueryType = "NonExistentCommand",
                Payload = null
            };

            // Act
            var response = await client.SendQueryAsync(invalidQuery);

            // Assert
            Assert.False(response.Success);
            Assert.False(string.IsNullOrEmpty(response.ErrorCode));
        }
    }
} 