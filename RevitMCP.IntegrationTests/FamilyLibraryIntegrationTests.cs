using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RevitMCP.Shared.DTOs;
using RevitMCP.Shared.Interfaces;
using RevitMCP.Shared.Models;
using Xunit;

namespace RevitMCP.IntegrationTests
{
    /// <summary>
    /// 族库端到端集成测试，验证仓储、服务、DTO、映射器等多层协作。
    /// </summary>
    public class FamilyLibraryIntegrationTests
    {
        // 简单权限枚举
        enum UserRole { Admin, User, ReadOnly }

        class InMemoryFamilyRepository : IFamilyRepository
        {
            private readonly Dictionary<string, FamilyMetadata> _store = new();
            public Task<IEnumerable<FamilyMetadata>> GetAllFamiliesAsync() => Task.FromResult(_store.Values.AsEnumerable());
            public Task<FamilyMetadata?> GetFamilyByIdAsync(string familyId) => Task.FromResult(_store.TryGetValue(familyId, out var v) ? v : null);
            public Task<IEnumerable<FamilyMetadata>> SearchFamiliesAsync(string keyword, int maxResults = 20) => Task.FromResult(_store.Values.Where(f => f.Name.Contains(keyword)).Take(maxResults));
            public virtual Task SaveOrUpdateFamilyAsync(FamilyMetadata metadata) { _store[metadata.Id] = metadata; return Task.CompletedTask; }
            public virtual Task DeleteFamilyAsync(string familyId) { _store.Remove(familyId); return Task.CompletedTask; }
        }

        class SimpleFamilySearchService : IFamilySearchService
        {
            private readonly IFamilyRepository _repo;
            public SimpleFamilySearchService(IFamilyRepository repo) { _repo = repo; }
            public async Task<IEnumerable<FamilyMetadata>> SearchByNaturalLanguageAsync(string query, int maxResults = 20) => await _repo.SearchFamiliesAsync(query, maxResults);
            public async Task<IEnumerable<FamilyMetadata>> SearchByTagsAsync(IEnumerable<string> tags, int maxResults = 20) => (await _repo.GetAllFamiliesAsync()).Where(f => f.Tags.Intersect(tags).Any()).Take(maxResults);
            public async Task<IEnumerable<FamilyMetadata>> SearchByCriteriaAsync(FamilySearchCriteria criteria, int maxResults = 20) => (await _repo.GetAllFamiliesAsync()).Where(f => f.Name.Contains(criteria.NameKeyword ?? "")).Take(maxResults);
        }

        [Fact]
        public async Task Add_Family_And_Retrieve_Through_All_Layers_Should_Be_Consistent()
        {
            var repo = new InMemoryFamilyRepository();
            var service = new SimpleFamilySearchService(repo);
            // 1. 新增族元数据
            var model = new FamilyMetadata(
                id: "F900",
                name: "集成测试族",
                category: "测试",
                tags: new List<string> { "集成", "端到端" },
                parameters: new Dictionary<string, Parameter> { { "Height", new Parameter("Height", "number", "mm", true, "高度", 1234) } },
                description: "集成测试用例",
                previewImagePath: "test.png",
                createdBy: "tester",
                lastModified: DateTime.UtcNow
            );
            await repo.SaveOrUpdateFamilyAsync(model);
            // 2. 服务检索
            var found = (await service.SearchByNaturalLanguageAsync("集成测试族")).FirstOrDefault();
            Assert.NotNull(found);
            Assert.Equal(model.Id, found?.Id);
            // 3. DTO导出
            var dto = FamilyMetadataMapper.ToDTO(found!);
            // 4. DTO再导入为领域模型
            var model2 = FamilyMetadataMapper.ToModel(dto);
            // 5. 一致性校验
            Assert.Equal(model.Id, model2.Id);
            Assert.Equal(model.Name, model2.Name);
            Assert.Equal(model.Category, model2.Category);
            Assert.Equal(model.Tags ?? new List<string>(), model2.Tags ?? new List<string>());
            Assert.Equal(model.Parameters["Height"].Name, model2.Parameters["Height"].Name);
            Assert.Equal(model.Parameters["Height"].DefaultValue, model2.Parameters["Height"].DefaultValue);
            Assert.Equal(model.Description ?? "", model2.Description ?? "");
        }

        [Fact]
        public async Task FamilyLibrary_Change_Should_Trigger_Schema_Export()
        {
            var repo = new InMemoryFamilyRepository();
            // 新增族
            var model = new FamilyMetadata(
                id: "F901",
                name: "Schema族",
                category: "测试",
                tags: new List<string> { "Schema" },
                parameters: new Dictionary<string, Parameter> { { "Width", new Parameter("Width", "number", "mm", true, "宽度", 5678) } },
                description: "Schema导出测试",
                previewImagePath: "test2.png",
                createdBy: "tester",
                lastModified: DateTime.UtcNow
            );
            await repo.SaveOrUpdateFamilyAsync(model);
            // 模拟Schema导出
            var families = await repo.GetAllFamiliesAsync();
            var schema = families.Select(f => new
            {
                f.Name,
                Parameters = f.Parameters.ToDictionary(p => p.Key, p => new { p.Value.Type, p.Value.Unit, p.Value.Required, p.Value.Description })
            }).ToList();
            // 校验Schema内容与族库一致
            Assert.Single(schema);
            Assert.Equal("Schema族", schema[0].Name);
            Assert.True(schema[0].Parameters.ContainsKey("Width"));
            Assert.Equal("number", schema[0].Parameters["Width"].Type);
        }

        [Fact]
        public async Task Batch_Add_And_Retrieve_Should_Be_Consistent()
        {
            var repo = new InMemoryFamilyRepository();
            var service = new SimpleFamilySearchService(repo);
            var families = new List<FamilyMetadata>
            {
                new("B001", "批量族1", "结构", new[]{"批量", "结构"}, new Dictionary<string, Parameter>{{"A", new("A", "number", "mm", true, "A", 1)}}, "desc1", null, null, DateTime.Now),
                new("B002", "批量族2", "建筑", new[]{"批量", "建筑"}, new Dictionary<string, Parameter>{{"B", new("B", "number", "mm", true, "B", 2)}}, "desc2", null, null, DateTime.Now),
                new("B003", "批量族3", "设备", new[]{"批量", "设备"}, new Dictionary<string, Parameter>{{"C", new("C", "number", "mm", true, "C", 3)}}, "desc3", null, null, DateTime.Now)
            };
            foreach (var fam in families) await repo.SaveOrUpdateFamilyAsync(fam);
            var found = (await service.SearchByNaturalLanguageAsync("批量")).ToList();
            Assert.Equal(3, found.Count);
            // 批量DTO导出再导入
            var dtos = found.Select(FamilyMetadataMapper.ToDTO).ToList();
            var models2 = dtos.Select(FamilyMetadataMapper.ToModel).ToList();
            for (int i = 0; i < families.Count; i++)
            {
                Assert.Equal(families[i].Id, models2[i].Id);
                Assert.Equal(families[i].Name, models2[i].Name);
                Assert.Equal(families[i].Category, models2[i].Category);
            }
        }

        [Fact]
        public async Task Update_Family_Metadata_Should_Reflect_In_Retrieval()
        {
            var repo = new InMemoryFamilyRepository();
            var service = new SimpleFamilySearchService(repo);
            var fam = new FamilyMetadata("U001", "更新族", "结构", new[]{"更新"}, new Dictionary<string, Parameter>{{"P", new("P", "number", "mm", true, "P", 10)}}, "old desc", null, null, DateTime.Now);
            await repo.SaveOrUpdateFamilyAsync(fam);
            // 更新描述和参数
            var updated = new FamilyMetadata("U001", "更新族", "结构", new[]{"更新"}, new Dictionary<string, Parameter>{{"P", new("P", "number", "mm", true, "P", 20)}}, "new desc", null, null, DateTime.Now);
            await repo.SaveOrUpdateFamilyAsync(updated);
            var found = (await service.SearchByNaturalLanguageAsync("更新族")).FirstOrDefault();
            Assert.NotNull(found);
            Assert.Equal("new desc", found?.Description ?? "");
            Assert.Equal(20, found?.Parameters["P"].DefaultValue);
            // DTO一致性
            var dto = FamilyMetadataMapper.ToDTO(found!);
            var model2 = FamilyMetadataMapper.ToModel(dto);
            Assert.Equal("new desc", model2.Description ?? "");
            Assert.Equal(20, model2.Parameters["P"].DefaultValue);
        }

        [Fact]
        public async Task Delete_Family_Should_Remove_Only_Target()
        {
            var repo = new InMemoryFamilyRepository();
            var service = new SimpleFamilySearchService(repo);
            var fam1 = new FamilyMetadata("D001", "删除族1", "结构", new[]{"删除"}, new Dictionary<string, Parameter>(), "desc1", null, null, DateTime.Now);
            var fam2 = new FamilyMetadata("D002", "删除族2", "结构", new[]{"删除"}, new Dictionary<string, Parameter>(), "desc2", null, null, DateTime.Now);
            await repo.SaveOrUpdateFamilyAsync(fam1);
            await repo.SaveOrUpdateFamilyAsync(fam2);
            await repo.DeleteFamilyAsync("D001");
            var found1 = await repo.GetFamilyByIdAsync("D001");
            var found2 = await repo.GetFamilyByIdAsync("D002");
            Assert.Null(found1);
            Assert.NotNull(found2);
        }

        [Fact]
        public async Task Complex_Condition_Search_And_Schema_Export_Should_Work()
        {
            var repo = new InMemoryFamilyRepository();
            var service = new SimpleFamilySearchService(repo);
            var fam1 = new FamilyMetadata("C001", "复合族", "结构", new[]{"复合", "结构"}, new Dictionary<string, Parameter>{{"X", new("X", "number", "mm", true, "X", 100)}}, "desc", null, null, DateTime.Now);
            var fam2 = new FamilyMetadata("C002", "复合族2", "建筑", new[]{"复合", "建筑"}, new Dictionary<string, Parameter>{{"Y", new("Y", "number", "mm", true, "Y", 200)}}, "desc", null, null, DateTime.Now);
            await repo.SaveOrUpdateFamilyAsync(fam1);
            await repo.SaveOrUpdateFamilyAsync(fam2);
            // 复杂条件检索
            var criteria = new FamilySearchCriteria { Category = "结构", Tags = new[]{"复合"}, ParameterName = "X" };
            var found = (await service.SearchByCriteriaAsync(criteria)).ToList();
            Assert.Single(found);
            Assert.Equal("C001", found[0].Id);
            // Schema导出
            var families = await repo.GetAllFamiliesAsync();
            var schema = families.Select(f => new
            {
                f.Name,
                Parameters = f.Parameters.ToDictionary(p => p.Key, p => new { p.Value.Type, p.Value.Unit, p.Value.Required, p.Value.Description })
            }).ToList();
            Assert.Equal(2, schema.Count);
            Assert.Contains(schema, s => s.Name == "复合族");
            Assert.Contains(schema, s => s.Name == "复合族2");
        }

        [Fact]
        public async Task Duplicate_Id_Import_Should_Overwrite_Old_Family()
        {
            var repo = new InMemoryFamilyRepository();
            var service = new SimpleFamilySearchService(repo);
            var fam1 = new FamilyMetadata("R001", "重复族", "结构", new[]{"重复"}, new Dictionary<string, Parameter>{{"A", new("A", "number", "mm", true, "A", 1)}}, "desc1", null, null, DateTime.Now);
            var fam2 = new FamilyMetadata("R001", "重复族新", "建筑", new[]{"重复", "新"}, new Dictionary<string, Parameter>{{"B", new("B", "number", "mm", true, "B", 2)}}, "desc2", null, null, DateTime.Now);
            await repo.SaveOrUpdateFamilyAsync(fam1);
            await repo.SaveOrUpdateFamilyAsync(fam2); // 应覆盖
            var found = await repo.GetFamilyByIdAsync("R001");
            Assert.NotNull(found);
            Assert.Equal("重复族新", found?.Name);
            Assert.Equal("建筑", found?.Category);
            Assert.True(found?.Parameters.ContainsKey("B") ?? false);
            // DTO一致性
            var dto = FamilyMetadataMapper.ToDTO(found!);
            var model2 = FamilyMetadataMapper.ToModel(dto);
            Assert.Equal("重复族新", model2.Name);
            Assert.Equal("建筑", model2.Category);
        }

        [Fact]
        public async Task Import_Invalid_Family_Should_Throw_Or_Reject()
        {
            var repo = new InMemoryFamilyRepository();
            // 空ID
            await Assert.ThrowsAsync<ArgumentException>(() => repo.SaveOrUpdateFamilyAsync(new FamilyMetadata("", "无效族", "结构", new[]{"无效"}, new Dictionary<string, Parameter>(), "desc", null, null, DateTime.Now)));
            // 空名称
            await Assert.ThrowsAsync<ArgumentException>(() => repo.SaveOrUpdateFamilyAsync(new FamilyMetadata("E002", "", "结构", new[]{"无效"}, new Dictionary<string, Parameter>(), "desc", null, null, DateTime.Now)));
            // 空类别
            await Assert.ThrowsAsync<ArgumentException>(() => repo.SaveOrUpdateFamilyAsync(new FamilyMetadata("E003", "无效族", "", new[]{"无效"}, new Dictionary<string, Parameter>(), "desc", null, null, DateTime.Now)));
        }

        [Fact]
        public async Task Search_With_Empty_Inputs_Should_Return_Reasonable_Results()
        {
            var repo = new InMemoryFamilyRepository();
            var service = new SimpleFamilySearchService(repo);
            var fam = new FamilyMetadata("S001", "空检索族", "结构", new[]{"空"}, new Dictionary<string, Parameter>(), "desc", null, null, DateTime.Now);
            await repo.SaveOrUpdateFamilyAsync(fam);
            // 空关键词
            var all = (await service.SearchByNaturalLanguageAsync("")).ToList();
            Assert.Single(all);
            // 空标签
            var tags = (await service.SearchByTagsAsync(new string[0])).ToList();
            Assert.Empty(tags);
            // 空参数名
            var criteria = new FamilySearchCriteria { ParameterName = "" };
            var result = (await service.SearchByCriteriaAsync(criteria)).ToList();
            Assert.Single(result); // 不过滤
        }

        [Fact]
        public async Task Bulk_Import_And_Paginated_Retrieval_Should_Work()
        {
            var repo = new InMemoryFamilyRepository();
            var service = new SimpleFamilySearchService(repo);
            int total = 1200;
            for (int i = 0; i < total; i++)
            {
                var fam = new FamilyMetadata($"BULK{i:D4}", $"批量族{i}", "批量", new[]{"批量", $"标签{i%10}"}, new Dictionary<string, Parameter>{{"P", new("P", "number", "mm", true, "P", i)}}, $"desc{i}", null, null, DateTime.Now);
                await repo.SaveOrUpdateFamilyAsync(fam);
            }
            // 全量检索
            var all = (await service.SearchByNaturalLanguageAsync("批量")).ToList();
            Assert.Equal(total, all.Count);
            // 分页检索
            var page1 = (await service.SearchByNaturalLanguageAsync("批量", 100)).ToList();
            var page2 = (await service.SearchByNaturalLanguageAsync("批量", 200)).ToList();
            Assert.Equal(100, page1.Count);
            Assert.Equal(200, page2.Count);
            // 标签检索
            var tag5 = (await service.SearchByTagsAsync(new[]{"标签5"})).ToList();
            Assert.Equal(total/10, tag5.Count);
        }

        [Fact]
        public async Task Extreme_Parameter_And_Tag_Values_Should_Not_Break_System()
        {
            var repo = new InMemoryFamilyRepository();
            var service = new SimpleFamilySearchService(repo);
            var longTag = new string('A', 1024);
            var specialTag = "!@#$%^&*()_+|~`";
            var longParam = new string('B', 2048);
            var fam = new FamilyMetadata(
                "X001",
                "极端族",
                "特殊",
                new[]{longTag, specialTag, "重复", "重复"},
                new Dictionary<string, Parameter>{{longParam, new(longParam, "string", null, false, "超长", "值")}},
                "desc",
                null,
                null,
                DateTime.Now
            );
            await repo.SaveOrUpdateFamilyAsync(fam);
            var found = await repo.GetFamilyByIdAsync("X001");
            Assert.NotNull(found);
            Assert.Contains(longTag, found.Tags ?? new List<string>());
            Assert.Contains(specialTag, found.Tags ?? new List<string>());
            Assert.Equal(3, (found.Tags ?? new List<string>()).Distinct().Count()); // 重复标签去重
            Assert.True(found.Parameters.ContainsKey(longParam));
            // 检索和Schema导出不应异常
            var result = (await service.SearchByTagsAsync(new[]{specialTag})).ToList();
            Assert.Single(result);
            var families = await repo.GetAllFamiliesAsync();
            var schema = families.Select(f => new
            {
                f.Name,
                Parameters = f.Parameters.ToDictionary(p => p.Key, p => new { p.Value.Type, p.Value.Unit, p.Value.Required, p.Value.Description })
            }).ToList();
            Assert.Single(schema);
            Assert.Equal("极端族", schema[0].Name);
            Assert.True(schema[0].Parameters.ContainsKey(longParam));
        }

        [Fact]
        public async Task Concurrent_Bulk_Import_Should_Be_Consistent()
        {
            var repo = new InMemoryFamilyRepository();
            var service = new SimpleFamilySearchService(repo);
            int total = 200;
            var tasks = new List<Task>();
            for (int t = 0; t < 4; t++)
            {
                int offset = t * (total / 4);
                tasks.Add(Task.Run(async () =>
                {
                    for (int i = offset; i < offset + (total / 4); i++)
                    {
                        var fam = new FamilyMetadata($"CON{i:D4}", $"并发族{i}", "并发", new[]{"并发", $"标签{i%5}"}, new Dictionary<string, Parameter>{{"P", new("P", "number", "mm", true, "P", i)}}, $"desc{i}", null, null, DateTime.Now);
                        await repo.SaveOrUpdateFamilyAsync(fam);
                    }
                }));
            }
            await Task.WhenAll(tasks);
            var all = (await service.SearchByNaturalLanguageAsync("并发")).ToList();
            Assert.Equal(total, all.Count);
            for (int i = 0; i < total; i++)
            {
                Assert.Contains(all, f => f.Id == $"CON{i:D4}");
            }
        }

        [Fact]
        public async Task Batch_Delete_Should_Remove_Only_Targets()
        {
            var repo = new InMemoryFamilyRepository();
            var service = new SimpleFamilySearchService(repo);
            var ids = Enumerable.Range(1, 20).Select(i => $"DEL{i:D2}").ToList();
            foreach (var id in ids)
            {
                var fam = new FamilyMetadata(id, $"删除族{id}", "批量", new[]{"批量"}, new Dictionary<string, Parameter>(), "desc", null, null, DateTime.Now);
                await repo.SaveOrUpdateFamilyAsync(fam);
            }
            // 批量删除前10个
            foreach (var id in ids.Take(10))
                await repo.DeleteFamilyAsync(id);
            var all = (await service.SearchByNaturalLanguageAsync("删除族")).ToList();
            Assert.Equal(10, all.Count);
            foreach (var id in ids.Take(10))
                Assert.DoesNotContain(all, f => f.Id == id);
            foreach (var id in ids.Skip(10))
                Assert.Contains(all, f => f.Id == id);
        }

        [Fact]
        public async Task Dynamic_Parameter_And_Tag_Change_Should_Reflect_In_Retrieval_And_Schema()
        {
            var repo = new InMemoryFamilyRepository();
            var service = new SimpleFamilySearchService(repo);
            // 初始导入两个族
            var fam1 = new FamilyMetadata("M001", "变更族1", "结构", new[]{"初始"}, new Dictionary<string, Parameter>{{"A", new("A", "number", "mm", true, "A", 1)}}, "desc1", null, null, DateTime.Now);
            var fam2 = new FamilyMetadata("M002", "变更族2", "建筑", new[]{"初始"}, new Dictionary<string, Parameter>{{"B", new("B", "number", "mm", true, "B", 2)}}, "desc2", null, null, DateTime.Now);
            await repo.SaveOrUpdateFamilyAsync(fam1);
            await repo.SaveOrUpdateFamilyAsync(fam2);
            // 动态添加参数和标签到fam1
            var fam1_updated = new FamilyMetadata("M001", "变更族1", "结构", new[]{"初始", "新增标签"}, new Dictionary<string, Parameter>{{"A", new("A", "number", "mm", true, "A", 1)}, {"C", new("C", "string", null, false, "新参数", "X")}}, "desc1", null, null, DateTime.Now);
            await repo.SaveOrUpdateFamilyAsync(fam1_updated);
            // 检索应包含新标签和新参数
            var found = await repo.GetFamilyByIdAsync("M001");
            Assert.Contains("新增标签", found.Tags ?? new List<string>());
            Assert.True(found.Parameters.ContainsKey("C"));
            // Schema导出应包含新参数
            var schema = (await repo.GetAllFamiliesAsync()).Select(f => new { f.Id, f.Parameters }).ToList();
            var s1 = schema.First(s => s.Id == "M001");
            Assert.True(s1.Parameters.ContainsKey("C"));
            // 动态删除参数和标签
            var fam1_removed = new FamilyMetadata("M001", "变更族1", "结构", new[]{"初始"}, new Dictionary<string, Parameter>{{"A", new("A", "number", "mm", true, "A", 1)}}, "desc1", null, null, DateTime.Now);
            await repo.SaveOrUpdateFamilyAsync(fam1_removed);
            var found2 = await repo.GetFamilyByIdAsync("M001");
            Assert.DoesNotContain("新增标签", found2.Tags ?? new List<string>());
            Assert.False(found2.Parameters.ContainsKey("C"));
            // fam2不受影响
            var found3 = await repo.GetFamilyByIdAsync("M002");
            Assert.Contains("B", found3.Parameters.Keys);
            Assert.Contains("初始", found3.Tags ?? new List<string>());
        }

        [Fact]
        public async Task SaveOrUpdateFamilyAsync_Should_Not_Leave_Dirty_State_On_Exception()
        {
            // 模拟仓储层异常
            var repo = new InMemoryFamilyRepositoryWithExceptionOnSave();
            var fam = new FamilyMetadata("EX001", "异常族", "结构", new[]{"异常"}, new Dictionary<string, Parameter>(), "desc", null, null, DateTime.Now);
            await Assert.ThrowsAsync<InvalidOperationException>(() => repo.SaveOrUpdateFamilyAsync(fam));
            // 检查未写入
            var found = await repo.GetFamilyByIdAsync("EX001");
            Assert.Null(found);
        }

        class InMemoryFamilyRepositoryWithExceptionOnSave : InMemoryFamilyRepository
        {
            public override Task SaveOrUpdateFamilyAsync(FamilyMetadata metadata)
            {
                throw new InvalidOperationException("模拟保存失败");
            }
        }

        [Fact]
        public async Task DeleteFamilyAsync_Should_Be_Idempotent()
        {
            var repo = new InMemoryFamilyRepository();
            var fam = new FamilyMetadata("IDEMP001", "幂等族", "结构", new[]{"幂等"}, new Dictionary<string, Parameter>(), "desc", null, null, DateTime.Now);
            await repo.SaveOrUpdateFamilyAsync(fam);
            // 连续多次删除
            await repo.DeleteFamilyAsync("IDEMP001");
            await repo.DeleteFamilyAsync("IDEMP001");
            await repo.DeleteFamilyAsync("IDEMP001");
            var found = await repo.GetFamilyByIdAsync("IDEMP001");
            Assert.Null(found);
        }

        [Fact]
        public async Task SaveOrUpdateFamilyAsync_Should_Be_Idempotent_For_Same_Data()
        {
            var repo = new InMemoryFamilyRepository();
            var fam = new FamilyMetadata("IDEMP002", "幂等族2", "结构", new[]{"幂等"}, new Dictionary<string, Parameter>(), "desc", null, null, DateTime.Now);
            await repo.SaveOrUpdateFamilyAsync(fam);
            await repo.SaveOrUpdateFamilyAsync(fam);
            await repo.SaveOrUpdateFamilyAsync(fam);
            var found = await repo.GetFamilyByIdAsync("IDEMP002");
            Assert.NotNull(found);
            Assert.Equal("幂等族2", found?.Name);
        }

        [Fact(Skip = "性能基准测试，人工分析输出")]
        public async Task Performance_Bulk_Retrieval_And_Schema_Export_Benchmark()
        {
            var repo = new InMemoryFamilyRepository();
            var service = new SimpleFamilySearchService(repo);
            int total = 10000;
            for (int i = 0; i < total; i++)
            {
                var fam = new FamilyMetadata($"PERF{i:D5}", $"性能族{i}", "性能", new[]{"性能", $"标签{i%20}"}, new Dictionary<string, Parameter>{{"P", new("P", "number", "mm", true, "P", i)}}, $"desc{i}", null, null, DateTime.Now);
                await repo.SaveOrUpdateFamilyAsync(fam);
            }
            // 全量检索性能
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var all = (await service.SearchByNaturalLanguageAsync("性能")).ToList();
            sw.Stop();
            Console.WriteLine($"[性能基准] 全量检索{total}条族耗时: {sw.ElapsedMilliseconds}ms");
            Assert.Equal(total, all.Count);
            // 分页检索性能
            sw.Restart();
            var page = (await service.SearchByNaturalLanguageAsync("性能", 500)).ToList();
            sw.Stop();
            Console.WriteLine($"[性能基准] 分页检索500条族耗时: {sw.ElapsedMilliseconds}ms");
            Assert.Equal(500, page.Count);
            // 标签检索性能
            sw.Restart();
            var tag10 = (await service.SearchByTagsAsync(new[]{"标签10"})).ToList();
            sw.Stop();
            Console.WriteLine($"[性能基准] 标签检索(标签10)耗时: {sw.ElapsedMilliseconds}ms, 数量: {tag10.Count}");
            // Schema导出性能
            sw.Restart();
            var families = await repo.GetAllFamiliesAsync();
            var schema = families.Select(f => new
            {
                f.Name,
                Parameters = f.Parameters.ToDictionary(p => p.Key, p => new { p.Value.Type, p.Value.Unit, p.Value.Required, p.Value.Description })
            }).ToList();
            sw.Stop();
            Console.WriteLine($"[性能基准] Schema导出{total}条族耗时: {sw.ElapsedMilliseconds}ms");
            Assert.Equal(total, schema.Count);
        }

        class SecureFamilyRepository : InMemoryFamilyRepository
        {
            private readonly UserRole _role;
            private readonly string _userId;
            public SecureFamilyRepository(UserRole role, string userId = "user1") { _role = role; _userId = userId; }
            public override Task SaveOrUpdateFamilyAsync(FamilyMetadata metadata)
            {
                if (_role == UserRole.ReadOnly) throw new UnauthorizedAccessException("只读用户不能写入");
                if (_role == UserRole.User && metadata.CreatedBy != _userId) throw new UnauthorizedAccessException("普通用户不能修改他人数据");
                return base.SaveOrUpdateFamilyAsync(metadata);
            }
            public override Task DeleteFamilyAsync(string familyId)
            {
                if (_role == UserRole.ReadOnly) throw new UnauthorizedAccessException("只读用户不能删除");
                // 假设只能删除自己创建的族
                if (_role == UserRole.User)
                {
                    var fam = this.GetFamilyByIdAsync(familyId).Result;
                    if (fam != null && fam.CreatedBy != _userId) throw new UnauthorizedAccessException("普通用户不能删除他人数据");
                }
                return base.DeleteFamilyAsync(familyId);
            }
        }

        [Fact]
        public async Task ReadOnly_User_Should_Be_Denied_Write_Operations()
        {
            var repo = new SecureFamilyRepository(UserRole.ReadOnly);
            var fam = new FamilyMetadata("SEC001", "安全族", "结构", new[]{"安全"}, new Dictionary<string, Parameter>(), "desc", null, null, DateTime.Now);
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => repo.SaveOrUpdateFamilyAsync(fam));
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => repo.DeleteFamilyAsync("SEC001"));
        }

        [Fact]
        public async Task Normal_User_Cannot_Modify_Or_Delete_Others_Data()
        {
            var repo = new SecureFamilyRepository(UserRole.User, userId: "userA");
            var fam = new FamilyMetadata("SEC002", "安全族2", "结构", new[]{"安全"}, new Dictionary<string, Parameter>(), "desc", null, "userB", DateTime.Now);
            // 由userB创建，userA不能修改/删除
            await ((InMemoryFamilyRepository)repo).SaveOrUpdateFamilyAsync(fam); // 用父类实现直接写入
            var famUpdate = new FamilyMetadata("SEC002", "安全族2-改", "结构", new[]{"安全"}, new Dictionary<string, Parameter>(), "desc", null, "userA", DateTime.Now);
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => repo.SaveOrUpdateFamilyAsync(famUpdate));
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => repo.DeleteFamilyAsync("SEC002"));
        }

        [Fact]
        public async Task Admin_User_Can_Do_All_Operations()
        {
            var repo = new SecureFamilyRepository(UserRole.Admin);
            var fam = new FamilyMetadata("SEC003", "安全族3", "结构", new[]{"安全"}, new Dictionary<string, Parameter>(), "desc", null, "admin", DateTime.Now);
            await repo.SaveOrUpdateFamilyAsync(fam);
            var found = await repo.GetFamilyByIdAsync("SEC003");
            Assert.NotNull(found);
            await repo.DeleteFamilyAsync("SEC003");
            var found2 = await repo.GetFamilyByIdAsync("SEC003");
            Assert.Null(found2);
        }
    }
} 