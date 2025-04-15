namespace RevitMCP.Shared.Interfaces
{
    /// <summary>
    /// Revit元素的跨端契约接口，定义所有Revit元素应实现的基础属性。
    /// </summary>
    public interface IRevitElement
    {
        /// <summary>
        /// 元素唯一标识符。
        /// </summary>
        string Id { get; }

        /// <summary>
        /// 元素名称。
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 元素类别。
        /// </summary>
        string Category { get; }
    }
} 