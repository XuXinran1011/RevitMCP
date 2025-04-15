namespace RevitMCP.Shared.Interfaces.Plugin
{
    /// <summary>
    /// Revit API适配器接口，封装对Revit API的常用操作，便于跨层解耦和测试。
    /// </summary>
    public interface IRevitAPIAdapter
    {
        /// <summary>
        /// 根据元素ID获取元素信息。
        /// </summary>
        /// <param name="elementId">元素唯一标识符</param>
        /// <returns>元素信息对象</returns>
        Task<IRevitElement?> GetElementAsync(string elementId);

        /// <summary>
        /// 设置元素参数值。
        /// </summary>
        /// <param name="elementId">元素唯一标识符</param>
        /// <param name="parameterName">参数名称</param>
        /// <param name="value">参数值</param>
        /// <returns>操作是否成功</returns>
        Task<bool> SetParameterValueAsync(string elementId, string parameterName, object value);

        /// <summary>
        /// 查找符合条件的元素。
        /// </summary>
        /// <param name="filter">过滤条件对象</param>
        /// <returns>元素信息集合</returns>
        Task<IEnumerable<IRevitElement>> FindElementsByFilterAsync(object filter);

        /// <summary>
        /// 获取当前文档的所有元素。
        /// </summary>
        /// <returns>元素信息集合</returns>
        Task<IEnumerable<IRevitElement>> GetAllElementsAsync();
    }
} 