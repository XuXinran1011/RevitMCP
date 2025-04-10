# RevitMCP - Revit自然语言交互

RevitMCP是一个基于Model Context Protocol (MCP)的实现，旨在使设计师和建造师能够通过自然语言与Autodesk Revit进行交互。

## 项目概述

RevitMCP通过将大型语言模型(LLM)的强大能力与Revit的专业功能相结合，创建了一个直观的界面，使用户能够：

- 使用自然语言查询Revit模型信息
- 通过对话式界面执行Revit操作
- 提取和分析BIM数据
- 自动化复杂的工作流程
- 通过直观的沟通连接设计意图与施工执行

## 项目结构

- **/MCPOfficial** - 包含MCP官方文档、规范和C# SDK
- **/RevitMCP** - RevitMCP服务器和客户端的核心实现
- **/Examples** - 示例项目和使用案例
- **/Documentation** - 综合文档和指南

## 系统要求

- Windows 10或更高版本
- Autodesk Revit 2025
- .NET 8 / .NET 9
- [Claude Desktop](https://claude.ai/desktop)或其他兼容MCP的客户端

## 快速开始

请参阅[安装指南](INSTALLATION.md)获取详细的设置说明。

### 基本步骤

1. 安装RevitMCP包
2. 配置您的MCP客户端（推荐使用Claude Desktop）
3. 连接到您的Revit实例
4. 开始使用自然语言与您的Revit模型交互

## 主要功能

- **自然语言查询**：使用普通语言询问有关Revit模型的问题
- **上下文理解**：系统理解BIM模型的上下文
- **跨学科沟通**：弥合设计师和建造师之间的沟通鸿沟
- **工作流自动化**：通过对话自动化重复性任务
- **数据提取与分析**：通过简单的提示提取和分析BIM数据

## 文档

- [架构概述](ARCHITECTURE.md) - 包含系统架构、设计原则与模式
- [API文档](API_DOCUMENTATION.md) - 详细的API参考文档
- [使用指南](USAGE.md) - 使用示例和最佳实践
- [贡献指南](CONTRIBUTING.md) - 如何参与项目开发

## 设计原则

RevitMCP遵循以下核心设计原则：

- **关注点分离** - 每个模块负责特定的功能
- **SOLID原则** - 遵循单一职责、开放/封闭等原则
- **安全第一** - 严格验证输入并防止潜在有害操作
- **可测试性** - 编写易于测试的代码
- **可扩展性** - 模块化设计便于扩展

我们采用了多种设计模式，包括依赖注入、适配器、命令、工厂、观察者等。详细信息请参阅[架构文档](ARCHITECTURE.md#设计原则与模式)。

## 开发状态

RevitMCP目前处于积极开发阶段。我们欢迎社区贡献和反馈，以帮助改进这个工具。

## 许可证

本项目采用MIT许可证 - 详情请参阅[LICENSE](LICENSE.md)文件。

## 致谢

- [Model Context Protocol](https://modelcontextprotocol.io/) - 本项目的基础
- Autodesk Revit API - 实现与Revit程序化交互
- Anthropic的Claude - 提供自然语言理解能力
