namespace RevitMCP.Shared.Interfaces
{
    /// <summary>
    /// Revit参数的跨端契约接口，定义所有参数应实现的基础属性。
    /// </summary>
    public interface IRevitParameter
    {
        /// <summary>
        /// 参数名称。
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 参数值。
        /// </summary>
        object? Value { get; }

        /// <summary>
        /// 参数类型（如字符串、整数、双精度等）。
        /// </summary>
        string Type { get; }

        /// <summary>
        /// 单位（如毫米、平方米等，可选）。
        /// </summary>
        string? Unit { get; }
    }
} 