using System;
using System.Diagnostics;

namespace RevitMCP.Plugin.Infrastructure.Communication
{
    /// <summary>
    /// MCP服务器管理器，负责管理MCP服务器进程
    /// </summary>
    public class MCPServerManager
    {
        private static readonly Lazy<MCPServerManager> _instance = new Lazy<MCPServerManager>(() => new MCPServerManager());
        
        /// <summary>
        /// 获取MCPServerManager的单例实例
        /// </summary>
        public static MCPServerManager Instance => _instance.Value;
        
        /// <summary>
        /// MCP服务器进程ID
        /// </summary>
        public int ServerProcessId { get; set; }
        
        /// <summary>
        /// MCP服务器是否正在运行
        /// </summary>
        public bool IsServerRunning
        {
            get
            {
                if (ServerProcessId <= 0)
                {
                    return false;
                }
                
                try
                {
                    Process process = Process.GetProcessById(ServerProcessId);
                    return !process.HasExited;
                }
                catch
                {
                    return false;
                }
            }
        }
        
        /// <summary>
        /// 私有构造函数，防止外部实例化
        /// </summary>
        private MCPServerManager()
        {
            ServerProcessId = 0;
        }
    }
}
