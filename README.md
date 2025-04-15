# RevitMCP

## 项目简介
RevitMCP（Model Context Protocol for Revit）致力于为Revit引入基于MCP协议的自然语言交互能力，极大提升BIM建模与协作效率。项目采用领域驱动设计（DDD）、CQRS模式和分层架构，支持多目标框架，具备高可扩展性和易维护性。

---

## 系统架构与设计

### 架构分层
- **RevitMCP.Plugin**（.NET 8）：Revit插件端，负责UI、命令发送、结果展示、与Server通信。
- **RevitMCP.Server**（.NET 9）：MCP服务器端，负责自然语言解析、命令分解、Revit API操作、与LLM客户端对接。
- **RevitMCP.Shared**（netstandard2.0）：共享DTO、接口、通信协议。
- **docs/**：详细设计与专题文档。

### 技术栈
- .NET 8（Plugin端，兼容Revit 2025）
- .NET 9（Server端，兼容MCP SDK）
- MCP官方C# SDK
- Revit API 2025
- xUnit、Moq、FluentAssertions（测试）
- GitHub Actions（CI/CD）

### 通信机制
- Plugin与Server通过MCP协议跨进程通信（Stdio/IPC，JSON消息）
- Shared层定义所有跨进程消息、DTO、接口，保证解耦
- Server预留与LLM客户端（如Claude Desktop）对接接口

---

## 主要功能
- 自然语言查询与建模（如"创建10米×8米房间"）
- 多轮对话与参数补全
- 自动生成墙体、门窗、楼板等建筑模型
- 端到端跨进程通信与反馈
- 高度可扩展的DDD分层架构
- 完善的单元、集成、系统测试

---

## MVP演示流程
1. 用户通过Ribbon界面启动MCP服务。
2. 在LLM客户端输入自然语言指令（如"创建一个10米×8米的房间"）。
3. 插件与Server通信，Server解析指令并调用Revit API自动建模。
4. 用户可多轮对话添加门窗、楼板等，实时看到模型生成。
5. 插件端反馈操作结果。

---

## 开发进度与计划

### 已完成
- 项目分层与目录结构搭建
- Shared层通信协议、DTO、接口实现
- Plugin/Server基础跨进程通信（Ping-Pong演示）
- MVP端到端集成测试（自动化）
- CI/CD自动化（GitHub Actions）
- 文档结构整理与.gitignore优化

### 进行中
- 自然语言解析与基础建模命令实现
- 多轮对话与参数补全
- MVP演示脚本与系统测试

### 计划中
- 高级功能（族库管理、变更管理、工程量计算等）
- 多版本Revit支持、性能优化、安全增强
- 安装包制作与配置指导
- 与项目管理系统集成接口预留

详细开发计划与模块进度见[开发计划.md](开发计划.md)与[REVITMCP_MODULES_CHECKLIST.md](REVITMCP_MODULES_CHECKLIST.md)。

---

## 目录结构
```
/
├── .github/workflows/         # CI/CD配置
├── README.md                  # 项目简介与说明
├── 开发计划.md                # 迭代目标与进度
├── 技术文档.md                # 技术选型与架构
├── REVITMCP_MODULES_CHECKLIST.md  # 模块清单与状态
├── RevitMCP_架构与项目结构.md      # 详细架构设计
├── docs/                      # 详细设计与专题文档
├── References/                # 本地DLL引用（不纳入git）
├── RevitMCP.Plugin/           # 插件端代码
├── RevitMCP.Server/           # 服务器端代码
├── RevitMCP.Shared/           # 共享库代码
├── RevitMCP.IntegrationTests/ # 集成测试
└── ...
```

---

## 依赖说明
- 需本地安装.NET 8、.NET 9、Revit 2025
- RevitAPI.dll、RevitAPIUI.dll请在本地Revit安装目录下获取并配置引用（不纳入git）
- 详细依赖与配置见[技术文档.md](技术文档.md)

---

## 贡献方式
欢迎issue、PR和建议！
- 参与开发请阅读[CONTRIBUTING.md](CONTRIBUTING.md)
- 遵守[CODE_OF_CONDUCT.md](CODE_OF_CONDUCT.md)
- 任何问题可在GitHub仓库提交issue

---

## 许可证
MIT License
