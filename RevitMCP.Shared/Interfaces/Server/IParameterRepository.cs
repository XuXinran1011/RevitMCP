namespace RevitMCP.Shared.Interfaces.Server
{
    /// <summary>
    /// 参数仓储抽象接口，定义参数的基本数据访问操作。
    /// </summary>
    public interface IParameterRepository
    {
        /// <summary>
        /// 根据参数ID获取参数信息。
        /// </summary>
        /// <param name="parameterId">参数唯一标识符</param>
        /// <returns>参数信息对象</returns>
        Task<IRevitParameter?> GetParameterByIdAsync(string parameterId);

        /// <summary>
        /// 查询所有参数。
        /// </summary>
        /// <returns>参数信息集合</returns>
        Task<IEnumerable<IRevitParameter>> GetAllParametersAsync();

        /// <summary>
        /// 按元素ID查询参数。
        /// </summary>
        /// <param name="elementId">元素唯一标识符</param>
        /// <returns>该元素的参数集合</returns>
        Task<IEnumerable<IRevitParameter>> GetParametersByElementIdAsync(string elementId);

        /// <summary>
        /// 新增或更新参数信息。
        /// </summary>
        /// <param name="parameter">参数对象</param>
        /// <returns>操作是否成功</returns>
        Task<bool> UpsertParameterAsync(IRevitParameter parameter);

        /// <summary>
        /// 删除指定参数。
        /// </summary>
        /// <param name="parameterId">参数唯一标识符</param>
        /// <returns>操作是否成功</returns>
        Task<bool> DeleteParameterAsync(string parameterId);
    }
} 