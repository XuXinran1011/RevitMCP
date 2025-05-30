# 贡献指南（CONTRIBUTING）

感谢你对RevitMCP项目的关注！我们欢迎任何形式的贡献，包括代码、文档、测试、建议和反馈。

## 如何参与
1. **Issue**：如发现Bug、需求或建议，请先在GitHub仓库提交Issue。
2. **Fork & PR**：Fork本仓库，创建新分支进行开发，完成后提交Pull Request（PR）。
3. **讨论**：如有架构、设计等重大变更建议，请先在Issue或Discussions中讨论。

## 开发流程
- 建议每个功能/修复使用独立分支，命名如`feature/xxx`、`bugfix/xxx`。
- 保持PR粒度小、描述清晰，便于代码审查。
- 每次提交请关联相关Issue（如有）。

## 分支管理
- `main`：主分支，保持可用、稳定。
- `dev`：开发分支，日常开发合并至此。
- 其他功能/修复分支：开发完成后合并到`dev`，经测试后再合并到`main`。

## 代码规范
- 遵循C#标准编码风格，变量/方法/类命名清晰。
- 公共接口、方法使用XML注释，说明用途、参数、返回值、异常。
- 复杂逻辑、设计决策处补充"为什么"注释。
- 代码变更时同步更新相关注释和文档。

## 测试要求
- 所有新功能/修复需配套单元测试或集成测试。
- 测试覆盖率不足的模块请优先补全。
- PR需通过CI自动化测试。

## 文档要求
- 重要变更需同步更新`README.md`、`开发计划.md`、`技术文档.md`等相关文档。
- 新增/优化模块请补充或完善`docs/`下专题文档。

## 行为准则
- 遵守[CODE_OF_CONDUCT.md](CODE_OF_CONDUCT.md)，尊重他人，积极协作。

## 联系方式
- 如有疑问或合作意向，可通过GitHub Issue或Discussions联系维护者。
