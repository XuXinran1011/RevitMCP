using System;
using System.Threading.Tasks;
using RevitMCP.Server.Infrastructure.Communication;

namespace RevitMCP.Server
{
    internal class Program
    {
        /// <summary>
        /// Server端主入口，监听标准输入输出，处理QueryMessage。
        /// </summary>
        static async Task Main(string[] args)
        {
            var comm = new ProcessCommunication(Console.OpenStandardInput(), Console.OpenStandardOutput());
            Console.Error.WriteLine("[Server] MCP Server已启动，等待消息...");
            await comm.ListenAsync();
        }
    }
} 