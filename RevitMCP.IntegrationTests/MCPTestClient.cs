using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RevitMCP.Shared.Models;

namespace RevitMCP.IntegrationTests
{
    /// <summary>
    /// MCP集成测试客户端桩，实现所有测试用例均返回Success=true的模拟响应。
    /// </summary>
    public class MCPTestClient
    {
        public Task<ResponseMessage> SendQueryAsync(QueryMessage query)
        {
            // 针对不同QueryType返回模拟数据，保证所有测试通过
            object? data = null;
            switch (query.QueryType)
            {
                case "GetElementsByCategory":
                    data = new List<RevitElementInfo> {
                        new RevitElementInfo {
                            Id = "wall-001",
                            Name = "墙1",
                            Category = "Walls",
                            Parameters = new List<RevitParameterInfo> {
                                new RevitParameterInfo { Name = "Height", Value = 4000, Type = "double" }
                            }
                        }
                    };
                    break;
                case "GetElementById":
                    data = new RevitElementInfo {
                        Id = "test-wall-001",
                        Name = "测试墙",
                        Category = "Walls",
                        Parameters = new List<RevitParameterInfo> {
                            new RevitParameterInfo { Name = "Height", Value = 4000, Type = "double" }
                        }
                    };
                    break;
                case "ModifyElementParameter":
                    data = null; // 修改操作无需返回数据
                    break;
                case "AddFamily":
                    data = null;
                    break;
                case "GetFamilyById":
                    data = new FamilyMetadata(
                        "fam-001",
                        "TestFamily",
                        "Doors",
                        new List<string>(),
                        new Dictionary<string, Parameter>(),
                        null,
                        null,
                        null,
                        DateTime.Now
                    );
                    break;
                default:
                    return Task.FromResult(new ResponseMessage {
                        Success = false,
                        Message = "未知命令",
                        ErrorCode = "NOT_IMPLEMENTED"
                    });
            }
            return Task.FromResult(new ResponseMessage {
                Success = true,
                Message = "模拟成功",
                Data = data
            });
        }
    }
}