# RevitMCP 架构设计与项目结构

本文档概述了RevitMCP项目的架构设计、项目结构和设计原则。

## 1. 系统概览

RevitMCP基于Model Context Protocol (MCP)构建，为大型语言模型(LLM)提供了与Autodesk Revit交互的标准化方式。系统由以下几个关键组件组成：

```plaintext
┌─────────────────┐      ┌─────────────────┐      ┌─────────────────┐
│                 │      │                 │      │                 │
│  LLM客户端      │◄────►│  RevitMCP       │◄────►│  Revit          │
│  (Claude等)     │      │  服务器         │      │  应用程序       │
│                 │      │                 │      │                 │
└─────────────────┘      └─────────────────┘      └─────────────────┘
```

### 1.1 关键组件

1. **LLM客户端**
   - Claude Desktop或其他兼容MCP的客户端
   - 处理用户交互和自然语言处理
   - 使用MCP协议与RevitMCP服务器通信

2. **RevitMCP服务器**
   - 实现MCP规范的核心组件
   - 将自然语言请求转换为Revit API操作
   - 管理LLM与Revit之间的上下文和状态
   - 向LLM客户端提供工具和资源

3. **Revit集成**
   - 连接到Revit API
   - 在Revit模型上执行命令和查询
   - 处理Revit特定的数据结构和操作

## 2. 项目结构

### 2.1 顶级目录

```plaintext
RevitMCP/
├── MCPOfficial/         # MCP官方文档、规范和C# SDK
├── RevitMCP/            # 核心实现
├── Examples/            # 示例项目和使用案例
├── Documentation/       # 综合文档
└── Tests/               # 测试项目
```

### 2.2 MCPOfficial

包含MCP官方文档、规范和C# SDK的副本，用于参考和开发。

```plaintext
MCPOfficial/
├── Documentation/       # MCP官方文档
├── Specification/       # MCP协议规范
└── SDK/                 # MCP C# SDK
```

### 2.3 RevitMCP

包含RevitMCP服务器和客户端的核心实现。

```plaintext
RevitMCP/
├── Presentation/           # UI层/表示层
│   ├── MCP/               # MCP服务器接口
│   ├── API/               # API接口
│   └── Prompts/            # 提示模板
├── Application/            # 应用层
│   ├── Commands/           # 命令处理器
│   ├── Queries/            # 查询处理器
│   ├── DTOs/               # 数据传输对象
│   ├── Services/           # 应用服务
│   └── EventHandlers/      # 事件处理器
├── Domain/                 # 领域层
│   ├── Models/             # 领域实体和值对象
│   │   ├── Elements/       # Revit元素模型
│   │   ├── Parameters/     # 参数模型
│   │   └── Views/          # 视图模型
│   ├── Services/           # 领域服务
│   ├── Repositories/       # 仓储接口
│   └── Events/             # 领域事件
└── Infrastructure/         # 基础设施层
    ├── RevitAPI/           # Revit API集成
    ├── Persistence/        # 持久化实现
    ├── MCP/                # MCP协议实现
    ├── NLP/                # 自然语言处理
    └── ExternalServices/    # 外部服务集成
```

### 2.4 Examples

包含示例项目和使用案例，展示如何使用RevitMCP。

```plaintext
Examples/
├── BasicQueries/        # 基本查询示例
├── ModelModification/   # 模型修改示例
├── ViewManagement/      # 视图管理示例
└── WorkflowAutomation/  # 工作流自动化示例
```

### 2.5 Documentation

包含项目的综合文档。

```plaintext
Documentation/
├── API/                 # API参考文档
├── Guides/              # 用户和开发者指南
├── Tutorials/           # 教程和示例
└── Images/              # 文档中使用的图像
```

### 2.6 Tests

包含项目的测试代码。

```plaintext
Tests/
├── UnitTests/           # 单元测试
├── IntegrationTests/    # 集成测试
└── TestData/            # 测试数据和模型
```

### 2.7 根目录文件

- `README.md` - 项目概述和入门信息
- `ARCHITECTURE.md` - 架构设计文档
- `API_DOCUMENTATION.md` - API文档
- `INSTALLATION.md` - 安装指南
- `USAGE.md` - 使用指南
- `CONTRIBUTING.md` - 贡献指南
- `LICENSE.md` - 许可证信息
- `CODE_OF_CONDUCT.md` - 社区行为准则
- `TEST_STRATEGY.md` - 测试策略文档
- `RevitMCP.sln` - Visual Studio解决方案文件

## 3. 技术架构

### 3.1 MCP实现

RevitMCP实现了Model Context Protocol规范，包括：

1. **工具(Tools)**
   - 可以在Revit模型上执行的定义操作
   - 每个工具都有特定的目的和参数模式
   - 示例：QueryElements, ModifyParameter, CreateView等

2. **资源(Resources)**
   - 向LLM公开的Revit数据和内容
   - 模型信息、元素属性、视图数据等
   - 结构化以便LLM易于使用

3. **提示(Prompts)**
   - 常见Revit操作的可重用模板
   - 特定任务的预定义工作流
   - 可针对不同用例进行自定义

### 3.2 数据流

1. **用户输入**
   - 用户向LLM客户端提供自然语言输入
   - LLM处理并理解请求

2. **工具选择**
   - LLM基于请求选择适当的RevitMCP工具
   - 从自然语言输入中提取参数

3. **工具执行**
   - RevitMCP服务器接收工具请求
   - 服务器将请求转换为Revit API调用
   - 在Revit模型上执行操作

4. **响应生成**
   - Revit操作的结果被格式化并返回
   - LLM基于结果生成自然语言响应
   - 响应呈现给用户

## 4. 技术实现策略

### 4.1 .NET版本兼容性解决方案

项目面临的一个技术挑战是Revit 2025最高支持.NET 8，而MCP官方SDK需要.NET 9。经过分析，我们确认MCP本身的架构设计已经为解决这个问题提供了完美的方案。

#### 4.1.1 架构解决方案

MCP服务本身就采用插件-MCP服务器-LLM客户端的架构，已支持跨进程通信，我们将利用这一特性实现版本兼容性：

```plaintext
┌─────────────────┐      ┌─────────────────┐      ┌─────────────────┐
│                 │      │                 │      │                 │
│  Revit 插件     │◄────►│  RevitMCP       │◄────►│  MCP 客户端     │
│  (.NET 8)       │      │  服务器         │      │  (Claude等)     │
│                 │      │  (.NET 9)       │      │                 │
└─────────────────┘      └─────────────────┘      └─────────────────┘
       │                        ▲
       │                        │
       ▼                        │
┌─────────────────┐             │
│                 │             │
│  Revit API      │             │
│  (.NET 8)       │             │
│                 │             │
└─────────────────┘             │
                                │
                      使用MCP原生传输机制
                      (Stdio或HTTP/SSE)
```

#### 4.1.2 具体实现方案

1. **Revit插件组件**：
   - 使用.NET 8开发，确保与Revit 2025兼容
   - 负责启动RevitMCP服务器进程
   - 使用MCP的`StdioClientTransport`与服务器通信
   - 处理Revit API调用

2. **RevitMCP服务器**：
   - 使用.NET 9开发，集成MCP SDK
   - 实现为独立进程
   - 使用MCP的`StdioServerTransport`接收来自Revit插件的请求
   - 使用MCP的标准协议与LLM客户端通信

3. **通信实现**：
   - 利用MCP SDK中的`StdioClientTransport`和`StdioServerTransport`类
   - 使用MCP的标准化消息格式和协议
   - 无需自行实现复杂的IPC机制

#### 4.1.3 多版本Revit支持

这种架构设计也为支持多版本的Revit提供了灵活性：

- 可以为不同版本的Revit创建特定的插件适配器
- 保持RevitMCP服务器的稳定性和一致性
- 实现功能探测和降级机制，适应不同版本的功能差异

这种方案充分利用了MCP协议的设计理念，无需引入额外的复杂性即可解决.NET版本兼容性问题。

## 5. 测试策略

### 5.1 测试级别

RevitMCP项目采用多层次的测试策略，确保软件质量和可靠性：

1. **单元测试**
   - 测试各个组件的独立功能
   - 使用模拟(Mock)隔离外部依赖
   - 覆盖核心业务逻辑和算法
   - 快速执行，作为CI/CD流程的一部分

2. **集成测试**
   - 测试组件之间的交互
   - 验证层与层之间的接口
   - 测试与Revit API的集成
   - 测试与MCP协议的集成

3. **系统测试**
   - 测试整个系统的功能
   - 验证端到端工作流
   - 测试性能和负载情况
   - 模拟真实用户场景

4. **验收测试**
   - 确保系统满足用户需求
   - 基于用户故事和用例
   - 可能包括手动测试和自动化测试
   - 验证用户体验和可用性

### 5.2 测试框架和工具

1. **单元测试框架**
   - xUnit：主要单元测试框架
   - Moq：用于创建模拟对象
   - FluentAssertions：提供更易读的断言

2. **集成测试工具**
   - TestHost：用于集成测试
   - WireMock.NET：模拟外部服务
   - Docker：提供隔离的测试环境

3. **系统测试工具**
   - Selenium/Playwright：UI自动化测试
   - JMeter/k6：性能和负载测试
   - Postman/RestSharp：API测试

4. **测试数据管理**
   - 使用示例Revit模型进行测试
   - 数据生成器创建测试数据
   - 版本控制测试数据

### 5.3 特殊测试考虑

#### 5.3.1 Revit API测试

由于Revit API的特殊性，需要特别考虑：

- **沙箱测试环境**：使用专门的测试环境运行Revit API测试
- **事务管理**：正确处理Revit事务，确保测试不会修改生产模型
- **模拟Revit API**：在可能的情况下，创建Revit API的模拟版本用于单元测试
- **测试自动化**：使用Revit API的批处理能力自动化测试

#### 5.3.2 跨进程通信测试

测试跨进程通信的策略：

- **端到端测试**：验证Revit插件与RevitMCP服务器之间的通信
- **协议一致性测试**：确保消息格式和协议实现正确
- **错误处理测试**：验证通信错误和异常情况的处理
- **性能测试**：测量通信延迟和吞吐量

#### 5.3.3 自然语言处理测试

测试与LLM交互的策略：

- **提示模板测试**：验证提示模板产生预期的结果
- **意图识别测试**：测试系统正确理解用户意图的能力
- **边缘情况测试**：测试模糊或不完整的用户输入
- **回归测试**：确保LLM集成的稳定性

### 5.4 测试自动化与CI/CD

1. **持续集成**
   - 每次提交自动运行单元测试
   - 定期运行集成测试和系统测试
   - 代码覆盖率分析和报告

2. **测试环境**
   - 开发环境：开发人员本地测试
   - 集成环境：自动化测试和集成测试
   - 预生产环境：性能测试和用户验收测试

3. **测试数据管理**
   - 使用版本控制管理测试数据
   - 自动生成测试数据
   - 数据清理和重置机制

## 6. 设计原则与模式

### 6.1 核心设计原则

#### 6.1.1 关注点分离 (Separation of Concerns)
- 将系统划分为不同的模块，每个模块负责特定的功能
- 明确区分MCP协议处理、Revit API交互和业务逻辑
- 使用分层架构隔离不同责任的代码

#### 6.1.2 SOLID原则
- **单一职责原则 (SRP)**: 每个类只有一个变更的理由
- **开放/封闭原则 (OCP)**: 对扩展开放，对修改封闭
- **里氏替换原则 (LSP)**: 子类应该能够替换其父类
- **接口隔离原则 (ISP)**: 客户端不应依赖它不使用的接口
- **依赖倒置原则 (DIP)**: 依赖抽象而非具体实现

#### 6.1.3 安全第一
- 严格验证所有输入数据
- 实施适当的权限控制
- 防止潜在的有害操作
- 安全处理异常和错误

#### 6.1.4 上下文理解
- 在交互过程中保持上下文
- 理解BIM上下文和术语
- 弥合自然语言和技术概念之间的差距

#### 6.1.5 可测试性设计
- 编写易于单元测试的代码
- 使用依赖注入便于模拟和测试
- 避免静态方法和全局状态

#### 6.1.6 可扩展性
- 模块化设计，便于添加新工具
- 可针对不同Revit工作流进行定制
- 适应不同的LLM客户端

#### 6.1.7 性能考虑
- 优化Revit API调用
- 高效处理大型模型数据
- 实施适当的缓存机制
- 异步处理长时间运行的操作

### 6.2 设计模式

#### 6.2.1 依赖注入模式
- 使用依赖注入容器管理服务生命周期
- 通过构造函数注入依赖
- 降低组件间的耦合度

```csharp
// 示例
public class RevitElementService
{
    private readonly IRevitConnection _revitConnection;

    public RevitElementService(IRevitConnection revitConnection)
    {
        _revitConnection = revitConnection;
    }

    // 服务方法...
}
```

#### 6.2.2 适配器模式
- 为Revit API创建适配器，使其符合项目需求
- 简化与Revit API的交互
- 隔离Revit API的变化

```csharp
public class RevitElementAdapter : IElementAdapter
{
    public ElementInfo AdaptElement(Autodesk.Revit.DB.Element revitElement)
    {
        // 将Revit元素转换为我们的模型
        return new ElementInfo
        {
            Id = revitElement.Id.IntegerValue,
            Name = revitElement.Name,
            // 其他属性...
        };
    }
}
```

#### 6.2.3 命令模式
- 将Revit操作封装为命令对象
- 支持操作的撤销/重做
- 便于日志记录和事务管理

```csharp
public class ModifyParameterCommand : IRevitCommand
{
    private readonly ElementId _elementId;
    private readonly string _parameterName;
    private readonly object _newValue;
    private object _oldValue;

    // 构造函数和Execute方法...

    public void Undo()
    {
        // 恢复参数的原始值
    }
}
```

#### 6.2.4 工厂模式
- 使用工厂创建复杂对象
- 集中管理对象创建逻辑
- 支持对象池和缓存

```csharp
public class RevitElementFactory
{
    public IElement CreateWall(XYZ startPoint, XYZ endPoint, ElementId wallTypeId)
    {
        // 创建墙元素的逻辑
    }

    public IElement CreateDoor(XYZ location, ElementId doorTypeId, ElementId hostWallId)
    {
        // 创建门元素的逻辑
    }
}
```

#### 6.2.5 观察者模式
- 实现事件驱动架构
- 通知系统中的变化
- 支持松耦合的组件通信

```csharp
public class ModelChangeNotifier
{
    public event EventHandler<ElementChangedEventArgs> ElementChanged;

    protected virtual void OnElementChanged(ElementChangedEventArgs e)
    {
        ElementChanged?.Invoke(this, e);
    }

    // 其他方法...
}
```

#### 6.2.6 策略模式
- 封装不同的算法和处理策略
- 支持运行时切换处理逻辑
- 使代码更具可扩展性

```csharp
public interface IElementFilterStrategy
{
    bool ShouldInclude(IElement element);
}

public class CategoryFilterStrategy : IElementFilterStrategy
{
    private readonly string _categoryName;

    public CategoryFilterStrategy(string categoryName)
    {
        _categoryName = categoryName;
    }

    public bool ShouldInclude(IElement element)
    {
        return element.Category == _categoryName;
    }
}
```

#### 6.2.7 仓储模式
- 抽象数据访问逻辑
- 提供统一的数据访问接口
- 便于切换数据源或实现缓存

```csharp
public interface IElementRepository
{
    Task<IElement> GetByIdAsync(int elementId);
    Task<IEnumerable<IElement>> FindByCategoryAsync(string category);
    // 其他方法...
}
```

### 6.3 架构模式

#### 6.3.1 领域驱动设计(DDD)分层架构

RevitMCP采用领域驱动设计(Domain-Driven Design, DDD)的分层架构，包括以下层次：

- **UI层/表示层 (Presentation)**
  - 负责用户交互和信息展示
  - 包含MCP服务器接口和自然语言处理
  - 将用户请求转换为应用层命令
  - 将应用层结果转换为用户友好的响应

- **应用层 (Application)**
  - 协调领域对象完成用户用例
  - 实现工作流程和业务流程
  - 包含命令处理器、查询处理器和应用服务
  - 不包含业务规则，只协调领域对象

- **领域层 (Domain)**
  - 架构的核心，包含业务模型和规则
  - 定义Revit元素、参数、视图等领域概念
  - 实现领域服务和领域事件
  - 包含业务实体、值对象、聚合根和仓储接口

- **基础设施层 (Infrastructure)**
  - 提供技术实现细节
  - 实现与Revit API的交互
  - 实现仓储接口和持久化
  - 集成外部服务和第三方库

- **跨层关注点**
  - 日志记录、异常处理、安全性
  - 跨越所有层的通用功能

#### 6.3.2 中介者模式

- 使用中介者协调复杂的组件交互
- 减少组件之间的直接依赖
- 集中管理工作流和状态

#### 6.3.3 管道-过滤器架构

- 将自然语言处理实现为一系列过滤器
- 每个过滤器负责特定的转换或处理步骤
- 支持灵活的处理流程配置

### 6.4 代码组织原则

#### 6.4.1 按功能组织代码

- 将相关功能组织在同一模块中
- 使用命名空间反映功能结构
- 保持相关代码的物理位置接近

#### 6.4.2 一致的命名约定

- 使用清晰、描述性的命名
- 遵循C#命名约定
- 保持命名风格一致

#### 6.4.3 文档和注释

- 为公共API提供XML文档
- 解释复杂算法和业务规则
- 记录设计决策和权衡

#### 6.4.4 异常处理策略

- 定义明确的异常层次结构
- 在适当的级别处理异常
- 提供有用的错误消息和上下文

## 7. 集成点

### 7.1 Revit API集成

RevitMCP通过官方Autodesk Revit API连接到Revit，允许它：

- 查询模型元素和属性
- 修改参数和元素属性
- 创建和操作视图
- 执行Revit命令和操作
- 访问和修改项目设置

### 7.2 MCP客户端集成

RevitMCP通过标准MCP协议与MCP客户端集成：

- 实现MCP服务器规范
- 为LLM客户端提供标准化接口
- 支持各种传输机制(stdio, HTTP等)
- 遵循MCP版本控制和兼容性指南

## 8. 未来架构考虑

1. **多模型支持**
   - 同时处理多个Revit模型
   - 跨模型操作和查询

2. **协作工作流**
   - 支持多用户与同一模型交互
   - 不同学科之间的协调

3. **扩展BIM生态系统**
   - 与其他BIM工具和平台集成
   - 连接到外部数据源和服务

4. **高级AI能力**
   - 为特定BIM任务整合专门的AI模型
   - 从用户交互中学习以改进建议
