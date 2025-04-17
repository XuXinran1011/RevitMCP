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
        // 优先用环境变量，次选自动拼接当前配置路径
        private static string GetServerExePath()
        {
            var envPath = Environment.GetEnvironmentVariable("SERVER_EXE_PATH");
            if (!string.IsNullOrWhiteSpace(envPath) && File.Exists(envPath))
                return envPath;
            // 自动根据当前测试配置(Debug/Release)拼接路径
            var config = Environment.GetEnvironmentVariable("CONFIGURATION") ?? "Debug";
            var solutionRoot = AppContext.BaseDirectory;
            // 向上查找解决方案根目录
            var dir = new DirectoryInfo(solutionRoot);
            while (dir != null && !File.Exists(Path.Combine(dir.FullName, "RevitMCP.sln")))
                dir = dir.Parent;
            if (dir == null)
                throw new DirectoryNotFoundException("未找到解决方案根目录");
            var exePath = Path.Combine(dir.FullName, "RevitMCP.Server", "bin", config, "net9.0", "RevitMCP.Server.exe");
            return exePath;
        }

        [Fact(DisplayName = "Plugin与Server端到端Ping-Pong通信")] 
        public async Task PluginServer_PingPong_Success()
        {
            var serverExePath = GetServerExePath();
            if (!File.Exists(serverExePath))
                throw new FileNotFoundException($"未找到Server可执行文件: {serverExePath}");

            var processStartInfo = new ProcessStartInfo
            {
                FileName = serverExePath,
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