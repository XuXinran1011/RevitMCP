using System;
using System.Diagnostics;
using System.Threading.Tasks;
using RevitMCP.Shared.Communication;

namespace RevitMCP.Plugin.Infrastructure.Communication
{
    /// <summary>
    /// Plugin端通信Demo，演示如何启动Server进程并进行Ping-Pong通信。
    /// </summary>
    public static class ProcessCommunicationDemo
    {
        /// <summary>
        /// 启动Server进程，发送Ping消息，输出Pong响应。
        /// </summary>
        public static async Task RunDemoAsync(string serverExePath)
        {
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
            if (serverProcess == null)
            {
                Console.WriteLine("[Plugin] 启动Server进程失败");
                return;
            }
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
            Console.WriteLine("[Plugin] 发送Ping...");
            var response = await comm.SendQueryAsync(query);
            if (response != null && response.Success)
                Console.WriteLine($"[Plugin] 收到响应: {response.Message}");
            else
                Console.WriteLine("[Plugin] 未收到有效响应");
        }
    }
} 