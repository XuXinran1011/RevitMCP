using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RevitMCP.Shared.DTOs;
using RevitMCP.Shared.Interfaces;
using RevitMCP.Shared.Models;
using Xunit;

namespace RevitMCP.Tests.Repositories
{
    /// <summary>
    /// 内存Mock族库仓储实现，仅用于单元测试。
    /// </summary>
    public class MockFamilyRepository : IFamilyRepository
    {
        private readonly Dictionary<string, FamilyMetadata> _store = new();

        public Task<IEnumerable<FamilyMetadata>> GetAllFamiliesAsync()
            => Task.FromResult(_store.Values.AsEnumerable());

        public Task<FamilyMetadata?> GetFamilyByIdAsync(string familyId)
        {
            _store.TryGetValue(familyId, out var value);
            return Task.FromResult(value);
        }

        public Task<IEnumerable<FamilyMetadata>> SearchFamiliesAsync(string keyword, int maxResults = 20)
        {
            var result = _store.Values
                .Where(f => f.Name.Contains(keyword ?? string.Empty, StringComparison.OrdinalIgnoreCase) ||
                            f.Category.Contains(keyword ?? string.Empty, StringComparison.OrdinalIgnoreCase))
                .Take(maxResults);
            return Task.FromResult(result);
        }

        public Task SaveOrUpdateFamilyAsync(FamilyMetadata metadata)
        {
            if (metadata == null) throw new ArgumentNullException(nameof(metadata));
            _store[metadata.Id] = metadata;
            return Task.CompletedTask;
        }

        public Task DeleteFamilyAsync(string familyId)
        {
            _store.Remove(familyId);
            return Task.CompletedTask;
        }
    }

    public class MockFamilyRepositoryTests
    {
        private FamilyMetadata CreateSample(string id, string name = "族", string category = "结构")
            => new FamilyMetadata(id, name, category, new List<string> { "标签" }, new Dictionary<string, Parameter>(), "描述", null, null, DateTime.Now);

        [Fact]
        public async Task Add_And_Get_Family_Should_Work()
        {
            var repo = new MockFamilyRepository();
            var family = CreateSample("F001", "墙体");
            await repo.SaveOrUpdateFamilyAsync(family);
            var fetched = await repo.GetFamilyByIdAsync("F001");
            Assert.NotNull(fetched);
            Assert.Equal("墙体", fetched.Name);
        }

        [Fact]
        public async Task Update_Family_Should_Overwrite()
        {
            var repo = new MockFamilyRepository();
            var family = CreateSample("F002", "门");
            await repo.SaveOrUpdateFamilyAsync(family);
            var updated = CreateSample("F002", "新门");
            await repo.SaveOrUpdateFamilyAsync(updated);
            var fetched = await repo.GetFamilyByIdAsync("F002");
            Assert.Equal("新门", fetched.Name);
        }

        [Fact]
        public async Task Delete_Family_Should_Remove()
        {
            var repo = new MockFamilyRepository();
            var family = CreateSample("F003");
            await repo.SaveOrUpdateFamilyAsync(family);
            await repo.DeleteFamilyAsync("F003");
            var fetched = await repo.GetFamilyByIdAsync("F003");
            Assert.Null(fetched);
        }

        [Fact]
        public async Task Search_Families_Should_Return_Match()
        {
            var repo = new MockFamilyRepository();
            await repo.SaveOrUpdateFamilyAsync(CreateSample("F004", "墙体"));
            await repo.SaveOrUpdateFamilyAsync(CreateSample("F005", "门"));
            var result = await repo.SearchFamiliesAsync("墙");
            Assert.Single(result);
            Assert.Equal("墙体", result.First().Name);
        }

        [Fact]
        public async Task Search_Families_With_Empty_Keyword_Should_Return_All()
        {
            var repo = new MockFamilyRepository();
            await repo.SaveOrUpdateFamilyAsync(CreateSample("F006", "A"));
            await repo.SaveOrUpdateFamilyAsync(CreateSample("F007", "B"));
            var result = await repo.SearchFamiliesAsync("");
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetFamilyByIdAsync_Should_Return_Null_If_Not_Exist()
        {
            var repo = new MockFamilyRepository();
            var fetched = await repo.GetFamilyByIdAsync("NotExist");
            Assert.Null(fetched);
        }

        [Fact]
        public async Task SaveOrUpdateFamilyAsync_Should_Throw_On_Null()
        {
            var repo = new MockFamilyRepository();
            await Assert.ThrowsAsync<ArgumentNullException>(() => repo.SaveOrUpdateFamilyAsync(null));
        }

        [Fact]
        public async Task DeleteFamilyAsync_Should_Not_Throw_If_Not_Exist()
        {
            var repo = new MockFamilyRepository();
            await repo.DeleteFamilyAsync("NotExist"); // 不应抛异常
        }
    }
} 