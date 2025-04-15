namespace RevitMCP.Shared.Interfaces
{
    /// <summary>
    /// Revit类别的跨端契约接口，定义所有类别应实现的基础属性。
    /// </summary>
    public interface IRevitCategory
    {
        /// <summary>
        /// 类别唯一标识符。
        /// </summary>
        int Id { get; }

        /// <summary>
        /// 类别名称。
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 父类别标识符（可选）。
        /// </summary>
        int? ParentId { get; }
    }
} 