namespace RevitMCP.Shared.Communication
{
    /// <summary>
    /// 进程间通信协议常量定义。
    /// </summary>
    public static class IPCProtocol
    {
        /// <summary>
        /// 查询消息类型
        /// </summary>
        public const string MessageTypeQuery = "Query";

        /// <summary>
        /// 响应消息类型
        /// </summary>
        public const string MessageTypeResponse = "Response";

        /// <summary>
        /// 默认通信通道名
        /// </summary>
        public const string DefaultChannel = "RevitMCP_Channel";
    }
} 