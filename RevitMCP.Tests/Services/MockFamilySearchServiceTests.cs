using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RevitMCP.Shared.Interfaces;
using RevitMCP.Shared.Models;
using Xunit;

namespace RevitMCP.Tests.Services
{
    /// <summary>
    /// 内存Mock族库搜索服务，仅用于单元测试。
    /// </summary>
    public class MockFamilySearchService : IFamilySearchService
    {
        private readonly List<FamilyMetadata> _families;
        public MockFamilySearchService(IEnumerable<FamilyMetadata> families)
        {
            _families = families.ToList();
        }

        public Task<IEnumerable<FamilyMetadata>> SearchByNaturalLanguageAsync(string query, int maxResults = 20)
        {
            var result = _families.Where(f =>
                (f.Name?.Contains(query ?? string.Empty, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (f.Category?.Contains(query ?? string.Empty, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (f.Tags?.Any(t => t.Contains(query ?? string.Empty, StringComparison.OrdinalIgnoreCase)) ?? false)
            ).Take(maxResults);
            return Task.FromResult(result);
        }

        public Task<IEnumerable<FamilyMetadata>> SearchByTagsAsync(IEnumerable<string> tags, int maxResults = 20)
        {
            var tagSet = new HashSet<string>(tags ?? Enumerable.Empty<string>(), StringComparer.OrdinalIgnoreCase);
            var result = _families.Where(f => f.Tags != null && tagSet.Overlaps(f.Tags ?? new List<string>())).Take(maxResults);
            return Task.FromResult(result);
        }

        public Task<IEnumerable<FamilyMetadata>> SearchByCriteriaAsync(FamilySearchCriteria criteria, int maxResults = 20)
        {
            var result = _families.AsEnumerable();
            if (!string.IsNullOrEmpty(criteria.NameKeyword))
                result = result.Where(f => f.Name.Contains(criteria.NameKeyword, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(criteria.Category))
                result = result.Where(f => f.Category.Equals(criteria.Category, StringComparison.OrdinalIgnoreCase));
            if (criteria.Tags != null && criteria.Tags.Any())
                result = result.Where(f => f.Tags != null && criteria.Tags.Intersect(f.Tags ?? new List<string>(), StringComparer.OrdinalIgnoreCase).Any());
            if (!string.IsNullOrEmpty(criteria.ParameterName))
                result = result.Where(f => (f.Parameters ?? new Dictionary<string, Parameter>()).ContainsKey(criteria.ParameterName));
            if (!string.IsNullOrEmpty(criteria.ParameterValue))
                result = result.Where(f => (f.Parameters?.Values.Any(p => p.DefaultValue?.ToString() == criteria.ParameterValue) ?? false));
            return Task.FromResult(result.Take(maxResults));
        }
    }

    public class MockFamilySearchServiceTests
    {
        private List<FamilyMetadata> GetSampleFamilies() => new()
        {
            new FamilyMetadata("F001", "墙体", "结构", new[]{"承重", "外墙"}, new Dictionary<string, Parameter>{{"Height", new Parameter("Height", "number", "mm", true, "高度", 3000)}}, "描述1", null, null, DateTime.Now),
            new FamilyMetadata("F002", "门", "建筑", new[]{"入口", "室内"}, new Dictionary<string, Parameter>{{"Width", new Parameter("Width", "number", "mm", true, "宽度", 900)}}, "描述2", null, null, DateTime.Now),
            new FamilyMetadata("F003", "窗", "建筑", new[]{"外窗"}, new Dictionary<string, Parameter>{{"Width", new Parameter("Width", "number", "mm", true, "宽度", 1200)}}, "描述3", null, null, DateTime.Now),
            new FamilyMetadata("F004", "柱", "结构", new[]{"承重"}, new Dictionary<string, Parameter>(), "描述4", null, null, DateTime.Now)
        };

        [Fact]
        public async Task SearchByNaturalLanguageAsync_Should_Match_Name_Or_Category_Or_Tag()
        {
            var service = new MockFamilySearchService(GetSampleFamilies());
            var result = await service.SearchByNaturalLanguageAsync("墙");
            Assert.Contains(result, f => f.Name.Contains("墙"));
            var result2 = await service.SearchByNaturalLanguageAsync("结构");
            Assert.Equal(2, result2.Count());
            var result3 = await service.SearchByNaturalLanguageAsync("承重");
            Assert.Equal(2, result3.Count());
        }

        [Fact]
        public async Task SearchByTagsAsync_Should_Return_Intersection()
        {
            var service = new MockFamilySearchService(GetSampleFamilies());
            var result = await service.SearchByTagsAsync(new[] { "承重" });
            Assert.Equal(2, result.Count());
            var result2 = await service.SearchByTagsAsync(new[] { "外窗", "入口" });
            Assert.Equal(2, result2.Count());
        }

        [Fact]
        public async Task SearchByCriteriaAsync_Should_Support_Complex_Conditions()
        {
            var service = new MockFamilySearchService(GetSampleFamilies());
            var criteria = new FamilySearchCriteria { NameKeyword = "门", Category = "建筑" };
            var result = await service.SearchByCriteriaAsync(criteria);
            Assert.Single(result);
            Assert.Equal("门", result.First().Name);

            var criteria2 = new FamilySearchCriteria { Tags = new[] { "承重" }, Category = "结构" };
            var result2 = await service.SearchByCriteriaAsync(criteria2);
            Assert.Equal(2, result2.Count());

            var criteria3 = new FamilySearchCriteria { ParameterName = "Width" };
            var result3 = await service.SearchByCriteriaAsync(criteria3);
            Assert.Equal(2, result3.Count());
        }

        [Fact]
        public async Task Search_Should_Handle_Empty_And_No_Match()
        {
            var service = new MockFamilySearchService(GetSampleFamilies());
            var result = await service.SearchByNaturalLanguageAsync("");
            Assert.Equal(4, result.Count());
            var result2 = await service.SearchByTagsAsync(new string[0]);
            Assert.Empty(result2);
            var criteria = new FamilySearchCriteria { NameKeyword = "不存在" };
            var result3 = await service.SearchByCriteriaAsync(criteria);
            Assert.Empty(result3);
        }
    }
} 