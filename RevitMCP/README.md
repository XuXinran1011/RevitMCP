# RevitMCP 项目结构

本文件夹包含RevitMCP项目的核心实现代码。

## 项目结构

- **Presentation**: UI层/表示层
  - **MCP**: MCP服务器接口
  - **API**: API接口
  - **Prompts**: 提示模板

- **Application**: 应用层
  - **Commands**: 命令处理器
  - **Queries**: 查询处理器
  - **DTOs**: 数据传输对象
  - **Services**: 应用服务
  - **EventHandlers**: 事件处理器

- **Domain**: 领域层
  - **Models**: 领域实体和值对象
    - **Elements**: Revit元素模型
    - **Parameters**: 参数模型
    - **Views**: 视图模型
  - **Services**: 领域服务
  - **Repositories**: 仓储接口
  - **Events**: 领域事件

- **Infrastructure**: 基础设施层
  - **RevitAPI**: Revit API集成
  - **Persistence**: 持久化实现
  - **MCP**: MCP协议实现
  - **NLP**: 自然语言处理
  - **ExternalServices**: 外部服务集成

## 开发指南

请参考项目根目录下的ARCHITECTURE.md文件，了解项目的架构设计和开发指南。
