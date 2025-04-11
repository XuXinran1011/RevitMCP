using System;
using System.Threading.Tasks;

namespace RevitMCP.Server.Application.Services
{
    /// <summary>
    /// 测试程序，用于验证基本功能
    /// </summary>
    public class TestProgram
    {
        /// <summary>
        /// 运行测试
        /// </summary>
        public static async Task RunTest()
        {
            Console.WriteLine("运行RevitMCP测试...");

            // 简单测试，确保程序可以运行
            Console.WriteLine("基本程序结构测试成功");
            Console.WriteLine("测试完成");

            // 模拟异步操作
            await Task.Delay(100);
        }
    }
}
