using RevitMCP.Shared.Models;
using System.Collections.Generic;

namespace RevitMCP.Shared.Interfaces.Server
{
    /// <summary>
    /// 族库接口，定义族库的基础属性和操作，便于族管理和查询。
    /// </summary>
    public interface IFamilyLibrary
    {
        /// <summary>
        /// 族库名称。
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 族库类型（如本地、云端等）。
        /// </summary>
        string Type { get; }

        /// <summary>
        /// 获取所有族元数据。
        /// </summary>
        /// <returns>族元数据集合</returns>
        IEnumerable<FamilyMetadata> GetFamilies();

        /// <summary>
        /// 添加族元数据。
        /// </summary>
        /// <param name="family">族元数据对象</param>
        void AddFamily(FamilyMetadata family);

        /// <summary>
        /// 移除指定族。
        /// </summary>
        /// <param name="familyId">族唯一标识符</param>
        void RemoveFamily(string familyId);
    }
} 