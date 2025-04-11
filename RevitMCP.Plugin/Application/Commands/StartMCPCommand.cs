using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using RevitMCP.Plugin.Infrastructure.Communication;

namespace RevitMCP.Plugin.Application.Commands
{
    /// <summary>
    /// 启动MCP服务器的命令
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class StartMCPCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                // 获取当前程序集所在目录
                string assemblyLocation = Assembly.GetExecutingAssembly().Location;
                string assemblyDirectory = Path.GetDirectoryName(assemblyLocation);
                
                // 构建MCP服务器路径
                string mcpServerPath = Path.Combine(assemblyDirectory, "RevitMCP.Server.dll");
                
                // 检查服务器文件是否存在
                if (!File.Exists(mcpServerPath))
                {
                    message = $"找不到MCP服务器文件: {mcpServerPath}";
                    return Result.Failed;
                }
                
                // 启动MCP服务器进程
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = $"\"{mcpServerPath}\" --mode revit",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                
                Process mcpServerProcess = new Process
                {
                    StartInfo = startInfo
                };
                
                // 启动进程
                mcpServerProcess.Start();
                
                // 存储进程ID，以便后续停止
                MCPServerManager.Instance.ServerProcessId = mcpServerProcess.Id;
                
                // 显示成功消息
                TaskDialog.Show("RevitMCP", "MCP服务器已成功启动");
                
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = $"启动MCP服务器时发生错误: {ex.Message}";
                return Result.Failed;
            }
        }
    }
}
