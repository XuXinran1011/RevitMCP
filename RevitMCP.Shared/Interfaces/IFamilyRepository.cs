using System.Collections.Generic;
using System.Threading.Tasks;
using RevitMCP.Shared.Models;

namespace RevitMCP.Shared.Interfaces
{
    /// <summary>
    /// 族库仓储接口，定义族元数据的统一查询、搜索与维护方法。
    /// </summary>
    public interface IFamilyRepository
    {
        /// <summary>
        /// 获取所有族元数据。
        /// </summary>
        Task<IEnumerable<FamilyMetadata>> GetAllFamiliesAsync();

        /// <summary>
        /// 根据族ID获取族元数据。
        /// </summary>
        Task<FamilyMetadata?> GetFamilyByIdAsync(string familyId);

        /// <summary>
        /// 按条件搜索族元数据。
        /// </summary>
        Task<IEnumerable<FamilyMetadata>> SearchFamiliesAsync(string keyword, int maxResults = 20);

        /// <summary>
        /// 新增或更新族元数据。
        /// </summary>
        Task SaveOrUpdateFamilyAsync(FamilyMetadata metadata);

        /// <summary>
        /// 删除指定ID的族元数据。
        /// </summary>
        Task DeleteFamilyAsync(string familyId);
    }
} 