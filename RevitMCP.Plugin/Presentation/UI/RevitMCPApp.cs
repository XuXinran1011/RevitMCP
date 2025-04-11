using Autodesk.Revit.UI;
using System;
using System.Reflection;
using System.IO;

namespace RevitMCP.Plugin.Presentation.UI
{
    /// <summary>
    /// Revit外部应用类，负责启动RevitMCP服务器
    /// </summary>
    public class RevitMCPApp : IExternalApplication
    {
        private static readonly string TabName = "RevitMCP";
        private static readonly string PanelName = "MCP";
        
        /// <summary>
        /// 应用程序启动时执行
        /// </summary>
        public Result OnStartup(UIControlledApplication application)
        {
            try
            {
                // 创建功能区选项卡和面板
                CreateRibbonPanel(application);
                
                // 启动RevitMCP服务器进程
                StartMCPServer();
                
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                TaskDialog.Show("RevitMCP Error", $"启动RevitMCP时发生错误: {ex.Message}");
                return Result.Failed;
            }
        }

        /// <summary>
        /// 应用程序关闭时执行
        /// </summary>
        public Result OnShutdown(UIControlledApplication application)
        {
            try
            {
                // 关闭RevitMCP服务器进程
                StopMCPServer();
                
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                TaskDialog.Show("RevitMCP Error", $"关闭RevitMCP时发生错误: {ex.Message}");
                return Result.Failed;
            }
        }

        /// <summary>
        /// 创建功能区面板和按钮
        /// </summary>
        private void CreateRibbonPanel(UIControlledApplication application)
        {
            // 创建选项卡
            application.CreateRibbonTab(TabName);
            
            // 创建面板
            RibbonPanel panel = application.CreateRibbonPanel(TabName, PanelName);
            
            // 获取程序集路径
            string assemblyPath = Assembly.GetExecutingAssembly().Location;
            
            // 创建启动MCP按钮
            PushButtonData startButtonData = new PushButtonData(
                "StartMCP",
                "启动MCP",
                assemblyPath,
                "RevitMCP.Plugin.Application.Commands.StartMCPCommand");
            
            PushButton startButton = panel.AddItem(startButtonData) as PushButton;
            startButton.ToolTip = "启动Model Context Protocol服务器";
            
            // 创建停止MCP按钮
            PushButtonData stopButtonData = new PushButtonData(
                "StopMCP",
                "停止MCP",
                assemblyPath,
                "RevitMCP.Plugin.Application.Commands.StopMCPCommand");
            
            PushButton stopButton = panel.AddItem(stopButtonData) as PushButton;
            stopButton.ToolTip = "停止Model Context Protocol服务器";
        }

        /// <summary>
        /// 启动RevitMCP服务器进程
        /// </summary>
        private void StartMCPServer()
        {
            // TODO: 实现启动RevitMCP服务器进程的逻辑
            // 使用MCP SDK的StdioClientTransport
        }

        /// <summary>
        /// 停止RevitMCP服务器进程
        /// </summary>
        private void StopMCPServer()
        {
            // TODO: 实现停止RevitMCP服务器进程的逻辑
        }
    }
}
