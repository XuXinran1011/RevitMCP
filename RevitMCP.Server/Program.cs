using System;
using System.Threading.Tasks;
using RevitMCP.Server.Presentation.MCP;
using RevitMCP.Server.Application.Services;

namespace RevitMCP.Server
{
    /// <summary>
    /// RevitMCP服务器程序入口点
    /// </summary>
    public class Program
    {
        /// <summary>
        /// 程序入口点
        /// </summary>
        public static async Task Main(string[] args)
        {
            try
            {
                // 检查是否为测试模式
                if (args.Length > 0 && args[0] == "--test")
                {
                    Console.WriteLine("开始运行测试模式...");
                    try
                    {
                        // 运行测试
                        await TestProgram.RunTest();
                        Console.WriteLine("测试完成，程序退出");
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine($"测试过程中发生错误: {ex}");
                    }
                    return;
                }

                // 解析命令行参数
                string mode = ParseMode(args);

                Console.WriteLine($"启动RevitMCP服务器，模式: {mode}");

                // 创建并启动MCP服务器
                var server = new MCPServer(mode);
                await server.StartAsync();

                Console.WriteLine("按任意键停止服务器...");
                Console.ReadKey();

                // 停止服务器
                await server.StopAsync();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"RevitMCP服务器发生错误: {ex}");
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// 从命令行参数中解析模式
        /// </summary>
        private static string ParseMode(string[] args)
        {
            string mode = "default";

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--mode" && i + 1 < args.Length)
                {
                    mode = args[i + 1];
                    break;
                }
            }

            return mode;
        }
    }
}
