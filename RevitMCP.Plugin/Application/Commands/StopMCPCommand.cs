using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Diagnostics;
using RevitMCP.Plugin.Infrastructure.Communication;

namespace RevitMCP.Plugin.Application.Commands
{
    /// <summary>
    /// 停止MCP服务器的命令
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class StopMCPCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                // 获取MCP服务器进程ID
                int processId = MCPServerManager.Instance.ServerProcessId;
                
                // 检查进程ID是否有效
                if (processId <= 0)
                {
                    TaskDialog.Show("RevitMCP", "MCP服务器未运行");
                    return Result.Succeeded;
                }
                
                try
                {
                    // 尝试获取进程
                    Process process = Process.GetProcessById(processId);
                    
                    // 关闭进程
                    process.Kill();
                    process.WaitForExit();
                    
                    // 重置进程ID
                    MCPServerManager.Instance.ServerProcessId = 0;
                    
                    // 显示成功消息
                    TaskDialog.Show("RevitMCP", "MCP服务器已成功停止");
                }
                catch (ArgumentException)
                {
                    // 进程不存在，重置进程ID
                    MCPServerManager.Instance.ServerProcessId = 0;
                    TaskDialog.Show("RevitMCP", "MCP服务器未运行");
                }
                
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = $"停止MCP服务器时发生错误: {ex.Message}";
                return Result.Failed;
            }
        }
    }
}
