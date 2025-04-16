using RevitMCP.Shared.Models;
using System.Collections.Generic;

namespace RevitMCP.Shared.Interfaces.Server
{
    /// <summary>
    /// 族库仓储抽象接口，定义族及其元数据的基本数据访问操作。
    /// </summary>
    public interface IFamilyRepository
    {
        /// <summary>
        /// 获取所有族库。
        /// </summary>
        /// <returns>族库集合</returns>
        Task<IEnumerable<IFamilyLibrary>> GetAllLibrariesAsync();

        /// <summary>
        /// 根据族ID获取族元数据。
        /// </summary>
        /// <param name="familyId">族唯一标识符</param>
        /// <returns>族元数据对象</returns>
        Task<FamilyMetadata?> GetFamilyByIdAsync(string familyId);

        /// <summary>
        /// 按条件搜索族。
        /// </summary>
        /// <param name="criteria">搜索条件</param>
        /// <returns>族元数据集合</returns>
        Task<IEnumerable<FamilyMetadata>> SearchFamiliesAsync(object criteria);

        /// <summary>
        /// 更新族元数据。
        /// </summary>
        /// <param name="familyId">族唯一标识符</param>
        /// <param name="metadata">族元数据对象</param>
        /// <returns>操作是否成功</returns>
        Task<bool> UpdateFamilyMetadataAsync(string familyId, FamilyMetadata metadata);
    }
} 