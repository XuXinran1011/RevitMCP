# RevitMCP 项目结构说明

本文档说明了RevitMCP项目的结构和组织方式。

## 项目概述

RevitMCP项目采用领域驱动设计(DDD)的分层架构，包括：

1. **UI层/表示层 (Presentation)**
2. **应用层 (Application)**
3. **领域层 (Domain)**
4. **基础设施层 (Infrastructure)**

项目分为三个主要组件：

1. **RevitMCP.Plugin** (.NET 8) - Revit插件项目
2. **RevitMCP.Server** (.NET 9) - MCP服务器项目
3. **RevitMCP.Shared** (netstandard2.0) - 共享库项目

## 项目结构

### RevitMCP.Plugin

```
RevitMCP.Plugin/
├── Presentation/ - UI层
│   └── UI/ - Revit UI相关代码
│       └── RevitMCPApp.cs - Revit外部应用类
├── Application/ - 应用层
│   ├── Commands/ - 命令处理器
│   │   ├── StartMCPCommand.cs - 启动MCP服务器命令
│   │   └── StopMCPCommand.cs - 停止MCP服务器命令
│   └── Services/ - 应用服务
├── Domain/ - 领域层
│   ├── Models/ - 领域模型
│   │   └── RevitElement.cs - Revit元素领域模型
│   └── Services/ - 领域服务
│       └── RevitElementService.cs - Revit元素服务
└── Infrastructure/ - 基础设施层
    ├── RevitAPI/ - Revit API适配器
    │   └── RevitAPIAdapter.cs - Revit API适配器
    └── Communication/ - 通信相关
        └── MCPServerManager.cs - MCP服务器管理器
```

### RevitMCP.Server

```
RevitMCP.Server/
├── Presentation/ - 表示层
│   └── MCP/ - MCP服务器接口
│       └── MCPServer.cs - MCP服务器类
├── Application/ - 应用层
│   ├── Commands/ - 命令处理器
│   │   └── GetElementCommand.cs - 获取元素命令
│   ├── Queries/ - 查询处理器
│   │   └── GetElementsByCategoryQuery.cs - 按类别获取元素查询
│   └── Services/ - 应用服务
│       ├── MCPToolService.cs - MCP工具服务
│       └── TestProgram.cs - 测试程序
├── Domain/ - 领域层
│   ├── Models/ - 领域模型
│   └── Services/ - 领域服务
└── Infrastructure/ - 基础设施层
    ├── MCP/ - MCP协议实现
    │   └── MCPToolHandler.cs - MCP工具处理器
    └── NLP/ - 自然语言处理
```

### RevitMCP.Shared

```
RevitMCP.Shared/
├── Models/ - 共享模型
│   └── RevitElementInfo.cs - Revit元素信息模型
├── Interfaces/ - 共享接口
│   └── IRevitElement.cs - Revit元素接口
├── Communication/ - 通信相关
│   └── MCPMessage.cs - MCP消息类
└── DTOs/ - 数据传输对象
    └── ElementDTO.cs - 元素数据传输对象
```

## 架构说明

### 领域驱动设计(DDD)分层架构

1. **UI层/表示层 (Presentation)**
   - 负责用户交互和信息展示
   - 包含MCP服务器接口和自然语言处理
   - 将用户请求转换为应用层命令
   - 将应用层结果转换为用户友好的响应

2. **应用层 (Application)**
   - 协调领域对象完成用户用例
   - 实现工作流程和业务流程
   - 包含命令处理器、查询处理器和应用服务
   - 不包含业务规则，只协调领域对象

3. **领域层 (Domain)**
   - 架构的核心，包含业务模型和规则
   - 定义Revit元素、参数、视图等领域概念
   - 实现领域服务和领域事件
   - 包含业务实体、值对象、聚合根和仓储接口

4. **基础设施层 (Infrastructure)**
   - 提供技术实现细节
   - 实现与Revit API的交互
   - 实现仓储接口和持久化
   - 集成外部服务和第三方库

### 跨进程通信

RevitMCP项目使用跨进程通信来解决.NET版本兼容性问题：

- **RevitMCP.Plugin** (.NET 8) - 与Revit API交互
- **RevitMCP.Server** (.NET 9) - 实现MCP协议
- 两个组件通过标准输入/输出进行通信

这种架构允许Revit插件在.NET 8环境中运行，同时MCP服务器可以利用.NET 9的功能。
