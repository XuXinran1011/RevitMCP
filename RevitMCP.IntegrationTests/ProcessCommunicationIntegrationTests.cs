using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using RevitMCP.Plugin.Infrastructure.Communication;
using RevitMCP.Shared.Communication;
using Xunit;

namespace RevitMCP.IntegrationTests
{
    public class ProcessCommunicationIntegrationTests
    {
        // TODO: 可通过配置文件或环境变量指定Server可执行文件路径
        private const string ServerExePath = @"D:\MyPrograms\RevitMCP.Server\bin\Debug\net9.0\RevitMCP.Server.exe";

        [Fact(DisplayName = "Plugin与Server端到端Ping-Pong通信")] 
        public async Task PluginServer_PingPong_Success()
        {
            if (!File.Exists(ServerExePath))
                throw new FileNotFoundException($"未找到Server可执行文件: {ServerExePath}");

            var processStartInfo = new ProcessStartInfo
            {
                FileName = ServerExePath,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using var serverProcess = Process.Start(processStartInfo);
            Assert.NotNull(serverProcess);

            // 可选：输出Server端错误信息
            _ = Task.Run(() =>
            {
                while (!serverProcess.StandardError.EndOfStream)
                {
                    var line = serverProcess.StandardError.ReadLine();
                    if (!string.IsNullOrWhiteSpace(line))
                        Console.WriteLine("[Server] " + line);
                }
            });

            var comm = new ProcessCommunication(serverProcess);
            var query = new QueryMessage
            {
                MessageType = IPCProtocol.MessageTypeQuery,
                RequestId = Guid.NewGuid().ToString(),
                QueryText = "Ping"
            };
            var response = await comm.SendQueryAsync(query);
            Assert.NotNull(response);
            Assert.True(response.Success);
            Assert.Equal("Pong", response.Message);

            // 关闭Server进程
            try { serverProcess.Kill(); } catch { /* 忽略 */ }
        }
    }
} 