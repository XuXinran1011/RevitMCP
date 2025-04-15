using System;

namespace RevitMCP.Shared.Models
{
    /// <summary>
    /// 表示Revit参数的基础信息，适用于跨进程通信和序列化。
    /// </summary>
    public class RevitParameterInfo
    {
        /// <summary>
        /// 参数名称。
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 参数值。
        /// </summary>
        public object? Value { get; set; }

        /// <summary>
        /// 参数类型（如字符串、整数、双精度等）。
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// 单位（如毫米、平方米等，可选）。
        /// </summary>
        public string? Unit { get; set; }
    }
} 