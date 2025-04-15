namespace RevitMCP.Shared.Interfaces.Server
{
    /// <summary>
    /// 元素仓储抽象接口，定义元素的基本数据访问操作。
    /// </summary>
    public interface IElementRepository
    {
        /// <summary>
        /// 根据元素ID获取元素信息。
        /// </summary>
        /// <param name="elementId">元素唯一标识符</param>
        /// <returns>元素信息对象</returns>
        Task<IRevitElement?> GetElementByIdAsync(string elementId);

        /// <summary>
        /// 查询所有元素。
        /// </summary>
        /// <returns>元素信息集合</returns>
        Task<IEnumerable<IRevitElement>> GetAllElementsAsync();

        /// <summary>
        /// 按类别查询元素。
        /// </summary>
        /// <param name="categoryId">类别ID</param>
        /// <returns>属于该类别的元素集合</returns>
        Task<IEnumerable<IRevitElement>> GetElementsByCategoryAsync(int categoryId);

        /// <summary>
        /// 新增或更新元素信息。
        /// </summary>
        /// <param name="element">元素对象</param>
        /// <returns>操作是否成功</returns>
        Task<bool> UpsertElementAsync(IRevitElement element);

        /// <summary>
        /// 删除指定元素。
        /// </summary>
        /// <param name="elementId">元素唯一标识符</param>
        /// <returns>操作是否成功</returns>
        Task<bool> DeleteElementAsync(string elementId);
    }
} 