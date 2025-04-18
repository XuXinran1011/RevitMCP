using System.Collections.Generic;
using System.Threading.Tasks;
using RevitMCP.Shared.Models;

namespace RevitMCP.Shared.Interfaces
{
    /// <summary>
    /// 族库搜索服务接口，支持自然语言、标签和多条件族元数据检索。
    /// </summary>
    public interface IFamilySearchService
    {
        /// <summary>
        /// 使用自然语言查询族元数据。
        /// </summary>
        /// <param name="query">自然语言查询字符串</param>
        /// <param name="maxResults">最大返回结果数</param>
        /// <returns>族元数据搜索结果列表</returns>
        Task<IEnumerable<FamilyMetadata>> SearchByNaturalLanguageAsync(string query, int maxResults = 20);

        /// <summary>
        /// 根据标签集合检索族元数据。
        /// </summary>
        /// <param name="tags">标签集合</param>
        /// <param name="maxResults">最大返回结果数</param>
        /// <returns>族元数据搜索结果列表</returns>
        Task<IEnumerable<FamilyMetadata>> SearchByTagsAsync(IEnumerable<string> tags, int maxResults = 20);

        /// <summary>
        /// 按自定义条件检索族元数据。
        /// </summary>
        /// <param name="criteria">搜索条件对象</param>
        /// <param name="maxResults">最大返回结果数</param>
        /// <returns>族元数据搜索结果列表</returns>
        Task<IEnumerable<FamilyMetadata>> SearchByCriteriaAsync(FamilySearchCriteria criteria, int maxResults = 20);
    }

    /// <summary>
    /// 族库搜索条件对象，支持多字段复合检索。
    /// </summary>
    public class FamilySearchCriteria
    {
        /// <summary>族名称关键字</summary>
        public string? NameKeyword { get; set; }
        /// <summary>类别名称</summary>
        public string? Category { get; set; }
        /// <summary>标签集合</summary>
        public IEnumerable<string>? Tags { get; set; }
        /// <summary>参数名关键字</summary>
        public string? ParameterName { get; set; }
        /// <summary>参数值关键字</summary>
        public string? ParameterValue { get; set; }
    }
} 