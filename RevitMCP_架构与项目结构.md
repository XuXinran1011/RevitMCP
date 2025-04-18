# RevitMCP 架构与项目结构设计文档

## 目录
- [1. 系统概述](#1-系统概述)
- [2. 架构设计与核心原则](#2-架构设计与核心原则)
- [3. 项目组织结构](#3-项目组织结构)
- [4. 核心功能模块设计](#4-核心功能模块设计)
- [5. 测试策略](#5-测试策略)
- [6. 安全与性能设计](#6-安全与性能设计)
- [7. 实现路线图](#7-实现路线图)
- [8. 结论](#8-结论)

---

## 用户反馈体系架构设计

### 架构分层
- **表示层（Plugin/UI）**：负责收集和展示用户反馈，包括操作结果、错误提示、进度状态等。
- **应用层（Server）**：负责处理反馈请求、生成反馈内容、推送反馈事件。
- **基础设施层**：负责日志记录、反馈数据持久化、异常上报等。

### 关键模块
- **FeedbackService（Server应用层）**：生成标准化反馈消息，提供API供Plugin/UI查询或订阅反馈。
- **FeedbackMessage（Shared层DTO）**：统一反馈消息结构，包含类型、内容、时间、关联操作ID等。
- **FeedbackPanel（Plugin/UI）**：展示反馈信息，支持历史查询、错误详情、用户建议入口等。
- **FeedbackLogger（基础设施层）**：将反馈事件写入日志，便于后期分析和追溯。

---

## 日志与监控体系架构设计

### 架构分层
- **基础设施层**：实现统一日志组件和监控采集器，支持多级别日志、结构化输出、远程上报。
- **应用层**：埋点关键业务事件、性能指标、异常等。
- **监控接口**：Server暴露健康检查、状态查询、性能指标API，便于外部监控系统集成。

### 关键模块
- **Logger（基础设施层）**：多级别（INFO/WARN/ERROR/DEBUG）、结构化日志、文件/远程双写。
- **MonitoringService（Server应用层）**：采集业务指标（命令执行数、成功率、响应时延等），提供健康检查API（/health）、指标API（/metrics）。
- **LogViewer（Plugin/UI）**：日志查询、过滤、导出等功能，便于用户和开发者排查问题。
- **AlertManager（可选）**：关键异常、服务异常自动报警（如邮件、Webhook等）。

---

## 多人协作机制说明

多人协作的并发与一致性问题交由Revit原生协作机制（如Revit Worksharing、Revit Server、BIM 360等）处理，RevitMCP只需对接Revit本地API，无需自行实现排队或同步机制。

---

## 1. 系统概述

RevitMCP (Model Context Protocol for Revit) 是一个利用大型语言模型(LLM)为Revit提供自然语言交互能力的系统。本文档详细说明了系统的架构设计、技术实现策略和项目组织结构。

### 1.1 项目背景与目标

RevitMCP旨在通过自然语言交互简化Revit的使用体验，实现以下目标：

- 建立稳定、高效的MCP服务器，连接Revit与LLM客户端
- 使用户能够通过自然语言查询和操作Revit模型
- 提高设计师和建造师在BIM环境中的工作效率
- 降低Revit学习曲线，使更多人能够有效利用BIM技术

### 1.2 系统架构总览

系统由以下三个关键组件组成：

```plaintext
┌─────────────────┐      ┌─────────────────┐      ┌─────────────────┐
│                 │      │                 │      │                 │
│  LLM客户端      │◄────►│  RevitMCP       │◄────►│  Revit          │
│  (Claude等)     │      │  服务器         │      │  应用程序       │
│                 │      │                 │      │                 │
└─────────────────┘      └─────────────────┘      └─────────────────┘
```

1. **LLM客户端**
   - Claude Desktop或其他兼容MCP的客户端
   - 处理用户交互和自然语言处理
   - 使用MCP协议与RevitMCP服务器通信

2. **RevitMCP服务器**
   - 实现MCP规范的核心组件
   - 将自然语言请求转换为Revit API操作
   - 管理LLM与Revit之间的上下文和状态

3. **Revit集成**
   - 连接到Revit API
   - 在Revit模型上执行命令和查询
   - 处理Revit特定的数据结构和操作

## 2. 架构设计与核心原则

### 2.1 领域驱动设计 (DDD) 架构

RevitMCP采用DDD的分层架构，构建了清晰的责任边界和领域模型：

1. **表示层 (Presentation)**
   - 负责用户界面和交互
   - 实现MCP协议接口
   - 处理自然语言输入输出
   - 不包含业务逻辑，仅负责信息展示和用户输入处理

2. **应用层 (Application)**
   - 协调领域对象完成用例
   - 处理工作流和业务流程
   - 不包含业务规则，只是编排领域服务和实体
   - 定义命令和查询处理器，实现用户意图

3. **领域层 (Domain)**
   - 包含核心业务逻辑和规则
   - 定义领域模型、实体和值对象
   - 实现领域服务，封装无法归属于单一实体的业务规则
   - 定义仓储接口，但不实现

4. **基础设施层 (Infrastructure)**
   - 提供技术实现，支持上层业务逻辑
   - 实现领域层定义的仓储接口
   - 提供外部系统集成
   - 处理持久化、消息传递等技术细节

### 2.2 架构设计原则

系统设计严格遵循以下原则，确保代码质量和可维护性：

#### 2.2.1 SOLID原则

- **单一职责原则(SRP)** - 每个类只负责一个功能领域
  ```csharp
  // 良好实践：每个服务专注于单一职责
  public class FamilySearchService { /* 仅处理族库搜索 */ }
  public class FamilyMetadataService { /* 仅处理族元数据 */ }
  ```

- **开闭原则(OCP)** - 对扩展开放，对修改封闭
  ```csharp
  // 通过接口和策略模式实现开闭原则
  public interface IQuantityCalculator { 
      double Calculate(Element element);
  }
  
  // 新增计算方法时，只需添加新类，无需修改现有代码
  public class ParameterBasedCalculator : IQuantityCalculator { /*...*/ }
  public class GeometryBasedCalculator : IQuantityCalculator { /*...*/ }
  ```

- **里氏替换原则(LSP)** - 子类可替换父类而不影响程序正确性
  ```csharp
  // 确保子类完全实现父类/接口契约
  public abstract class ElementProcessor {
      public abstract void Process(Element element);
  }
  
  public class WallProcessor : ElementProcessor {
      public override void Process(Element element) { /*...*/ }
  }
  ```

- **接口隔离原则(ISP)** - 客户端不应依赖不使用的接口
  ```csharp
  // 将大接口分解为小接口，确保精确依赖
  public interface IElementReader { 
      Element GetElement(string id);
  }
  
  public interface IElementWriter {
      void SaveElement(Element element);
  }
  ```

- **依赖倒置原则(DIP)** - 高层模块不应依赖低层模块，都应依赖抽象
  ```csharp
  // 通过依赖注入实现控制反转
  public class ModelQueryService {
      private readonly IElementRepository _repository;
      
      public ModelQueryService(IElementRepository repository) {
          _repository = repository;
      }
  }
  ```

#### 2.2.2 迪米特法则(LoD)

- 对象应只与直接朋友通信，减少对象间耦合
  ```csharp
  // 不良实践
  public class ModelManager {
      public void UpdateElement(string elementId, string paramName, object value) {
          var repository = new ElementRepository(); // 直接实例化依赖
          var element = repository.GetElement(elementId);
          element.SetParameter(paramName, value); // 直接操作内部细节
          repository.SaveElement(element);
      }
  }
  
  // 良好实践
  public class ModelManager {
      private readonly IElementService _elementService;
      
      public ModelManager(IElementService elementService) {
          _elementService = elementService;
      }
      
      public void UpdateElement(string elementId, string paramName, object value) {
          _elementService.UpdateParameter(elementId, paramName, value);
      }
  }
  ```

#### 2.2.3 合成复用原则(CRP)

- 优先使用对象组合而非继承实现代码复用
  ```csharp
  // 不良实践：通过继承复用功能
  public class AdvancedElementRepository : BaseElementRepository {
      // 通过继承复用BaseElementRepository的功能
  }
  
  // 良好实践：通过组合复用功能
  public class ElementRepository : IElementRepository {
      private readonly IRevitApiAdapter _revitApi;
      private readonly IElementMapper _mapper;
      
      public ElementRepository(
          IRevitApiAdapter revitApi,
          IElementMapper mapper) {
          _revitApi = revitApi;
          _mapper = mapper;
      }
      
      // 通过组合使用依赖的功能
  }
  ```

### 2.3 技术架构设计

项目面临的技术挑战之一是Revit 2025最高支持.NET 8，而MCP官方SDK需要.NET 9。为解决此问题，我们利用MCP原生的跨进程通信能力：

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

1. **Revit插件组件** (.NET 8)
   - 使用.NET 8开发，确保与Revit 2025兼容
   - 负责启动RevitMCP服务器进程
   - 使用MCP的`StdioClientTransport`与服务器通信
   - 处理Revit API调用

2. **RevitMCP服务器** (.NET 9)
   - 使用.NET 9开发，集成MCP SDK
   - 实现为独立进程
   - 使用MCP的`StdioServerTransport`接收来自Revit插件的请求
   - 使用MCP的标准协议与LLM客户端通信 

## 3. 项目组织结构

RevitMCP项目采用模块化结构，清晰划分不同组件的职责：

### 3.1 解决方案结构

项目包含三个主要组件：

1. **RevitMCP.Plugin** (.NET 8) - Revit插件项目
2. **RevitMCP.Server** (.NET 9) - MCP服务器项目
3. **RevitMCP.Shared** (netstandard2.0) - 共享库项目

每个组件按照DDD架构原则进行内部组织，确保清晰的职责分离和依赖关系管理。

### 3.2 代码目录组织

#### 3.2.1 RevitMCP.Plugin

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

#### 3.2.2 RevitMCP.Server

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
│       └── MCPToolService.cs - MCP工具服务
├── Domain/ - 领域层
│   ├── Models/ - 领域模型
│   └── Services/ - 领域服务
└── Infrastructure/ - 基础设施层
    ├── MCP/ - MCP协议实现
    │   └── MCPToolHandler.cs - MCP工具处理器
    └── NLP/ - 自然语言处理
```

#### 3.2.3 RevitMCP.Shared

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

### 3.3 命名约定

为保证代码一致性和可读性，项目采用以下命名约定：

1. **文件命名**
   - 源代码文件：PascalCase（如 `RevitElement.cs`）
   - 配置文件：小写（如 `config.json`）
   - 文档文件：大写（如 `README.md`）

2. **目录命名**
   - 源代码目录：PascalCase（如 `Infrastructure/`）
   - 资源目录：小写（如 `resources/`）
   - 测试目录：PascalCase（如 `UnitTests/`）

3. **命名空间组织**
   - 遵循目录结构
   - 使用公司/项目前缀
   - 反映功能层次

4. **类型命名**
   - 接口：以"I"为前缀（如 `IElementRepository`）
   - 抽象类：以"Base"或"Abstract"为前缀（如 `BaseElementService`）
   - 服务类：以"Service"为后缀（如 `FamilySearchService`）
   - 仓储类：以"Repository"为后缀（如 `ElementRepository`）

## 4. 核心功能模块设计

系统包含多个功能模块，每个模块都采用DDD架构进行设计，确保职责清晰和关注点分离。

### 4.1 自然语言查询模块

该模块负责处理用户通过自然语言提出的查询请求，并从Revit模型中提取相关信息。

#### 4.1.1 领域模型

```csharp
// 查询请求领域模型
public class ModelQuery
{
    public string QueryText { get; }
    public QueryType Type { get; }
    public IDictionary<string, object> Parameters { get; }
    public string Context { get; }
    
    // 值对象模式，不可变
    public ModelQuery(string queryText, QueryType type, 
        IDictionary<string, object> parameters, string context)
    {
        QueryText = queryText;
        Type = type;
        Parameters = new ReadOnlyDictionary<string, object>(parameters);
        Context = context;
    }
}

// 查询类型枚举
public enum QueryType
{
    ElementSelection,
    PropertyExtraction,
    Calculation,
    Relationship,
    Documentation
}
```

#### 4.1.2 领域服务

```csharp
// 查询服务接口
public interface IModelQueryService
{
    Task<QueryResult> ExecuteQueryAsync(ModelQuery query);
    Task<IEnumerable<Element>> FindElementsAsync(string naturalLanguageQuery);
    Task<IDictionary<string, object>> ExtractPropertiesAsync(
        IEnumerable<Element> elements, string[] propertyNames);
}

// 查询结果值对象
public class QueryResult
{
    public bool Success { get; }
    public string ResultText { get; }
    public object Data { get; }
    public IEnumerable<string> Messages { get; }
    public Exception Error { get; }
    
    // 成功结果工厂方法
    public static QueryResult CreateSuccess(string resultText, object data = null,
        IEnumerable<string> messages = null)
    {
        return new QueryResult(true, resultText, data, messages);
    }
    
    // 失败结果工厂方法
    public static QueryResult CreateFailure(Exception error, 
        IEnumerable<string> messages = null)
    {
        return new QueryResult(false, null, null, messages, error);
    }
    
    // 私有构造函数强制使用工厂方法
    private QueryResult(bool success, string resultText, object data,
        IEnumerable<string> messages = null, Exception error = null)
    {
        Success = success;
        ResultText = resultText;
        Data = data;
        Messages = messages ?? new List<string>();
        Error = error;
    }
}
```

#### 4.1.3 应用服务

```csharp
// 查询命令处理器
public class ModelQueryHandler
{
    private readonly IModelQueryService _queryService;
    private readonly IQueryParser _queryParser;
    
    public ModelQueryHandler(
        IModelQueryService queryService,
        IQueryParser queryParser)
    {
        _queryService = queryService;
        _queryParser = queryParser;
    }
    
    public async Task<QueryResult> HandleQueryAsync(string naturalLanguageQuery)
    {
        try
        {
            // 解析查询
            var modelQuery = _queryParser.ParseQuery(naturalLanguageQuery);
            
            // 执行查询
            return await _queryService.ExecuteQueryAsync(modelQuery);
        }
        catch (Exception ex)
        {
            return QueryResult.CreateFailure(ex);
        }
    }
}
```

#### 4.1.4 基础设施实现

```csharp
// 查询解析器实现
public class NaturalLanguageQueryParser : IQueryParser
{
    private readonly ILLMService _llmService;
    
    public NaturalLanguageQueryParser(ILLMService llmService)
    {
        _llmService = llmService;
    }
    
    public ModelQuery ParseQuery(string naturalLanguageQuery)
    {
        // 使用LLM服务解析查询意图和参数
        var parseResult = _llmService.ParseQueryIntent(naturalLanguageQuery);
        
        // 转换为领域模型
        return new ModelQuery(
            naturalLanguageQuery,
            parseResult.QueryType,
            parseResult.Parameters,
            parseResult.Context
        );
    }
}
```

### 4.2 模型修改模块

该模块负责处理用户通过自然语言指令对Revit模型进行的修改操作。

#### 4.2.1 领域模型

```csharp
// 模型修改命令
public class ModelModificationCommand
{
    public string CommandText { get; }
    public ModificationType Type { get; }
    public IReadOnlyDictionary<string, object> Parameters { get; }
    public IEnumerable<string> TargetElementIds { get; }
    
    // 值对象模式，不可变
    public ModelModificationCommand(
        string commandText, 
        ModificationType type,
        IDictionary<string, object> parameters,
        IEnumerable<string> targetElementIds)
    {
        CommandText = commandText;
        Type = type;
        Parameters = new ReadOnlyDictionary<string, object>(parameters);
        TargetElementIds = targetElementIds?.ToList() ?? new List<string>();
    }
}

// 修改类型枚举
public enum ModificationType
{
    ParameterChange,
    ElementCreation,
    ElementDeletion,
    ElementRelocation,
    RelationshipChange
}

// 修改结果值对象
public class ModificationResult
{
    public bool Success { get; }
    public string ResultMessage { get; }
    public IEnumerable<string> ModifiedElementIds { get; }
    public Exception Error { get; }
    
    // 成功结果工厂方法
    public static ModificationResult CreateSuccess(
        string message, 
        IEnumerable<string> modifiedElementIds)
    {
        return new ModificationResult(true, message, modifiedElementIds);
    }
    
    // 失败结果工厂方法
    public static ModificationResult CreateFailure(
        string message, 
        Exception error = null)
    {
        return new ModificationResult(false, message, null, error);
    }
    
    // 私有构造函数强制使用工厂方法
    private ModificationResult(
        bool success, 
        string resultMessage,
        IEnumerable<string> modifiedElementIds = null,
        Exception error = null)
    {
        Success = success;
        ResultMessage = resultMessage;
        ModifiedElementIds = modifiedElementIds ?? new List<string>();
        Error = error;
    }
}
```

#### 4.2.2 领域服务

```csharp
// 模型修改服务接口
public interface IModelModificationService
{
    Task<ModificationResult> ExecuteModificationAsync(
        ModelModificationCommand command);
    
    Task<ModificationResult> ModifyParametersAsync(
        IEnumerable<string> elementIds,
        IDictionary<string, object> parameterChanges);
    
    Task<ModificationResult> CreateElementsAsync(
        string elementType,
        IDictionary<string, object> properties,
        IEnumerable<LocationInfo> locations);
}

// 事务管理器接口
public interface ITransactionManager
{
    T Execute<T>(Func<T> action, string transactionName);
    void Execute(Action action, string transactionName);
}
```

#### 4.2.3 应用服务

```csharp
// 修改命令处理器
public class ModelModificationHandler
{
    private readonly IModelModificationService _modificationService;
    private readonly ICommandParser _commandParser;
    private readonly IValidationService _validationService;
    
    public ModelModificationHandler(
        IModelModificationService modificationService,
        ICommandParser commandParser,
        IValidationService validationService)
    {
        _modificationService = modificationService;
        _commandParser = commandParser;
        _validationService = validationService;
    }
    
    public async Task<ModificationResult> HandleCommandAsync(
        string naturalLanguageCommand)
    {
        try
        {
            // 解析命令
            var command = _commandParser.ParseCommand(naturalLanguageCommand);
            
            // 验证命令
            var validationResult = _validationService.ValidateCommand(command);
            if (!validationResult.IsValid)
            {
                return ModificationResult.CreateFailure(
                    "命令验证失败: " + string.Join(", ", validationResult.Errors));
            }
            
            // 执行修改
            return await _modificationService.ExecuteModificationAsync(command);
        }
        catch (Exception ex)
        {
            return ModificationResult.CreateFailure(
                "命令执行失败", ex);
        }
    }
}
```

### 4.3 族库管理模块

该模块负责管理、索引和搜索Revit族库，使用户能通过自然语言方便地查找和使用族。

#### 4.3.1 领域模型

```csharp
// 族库领域模型
public interface IFamilyLibrary
{
    string Name { get; }
    LibraryType Type { get; }
    IEnumerable<FamilyMetadata> GetFamilies();
    void AddFamily(FamilyMetadata family);
    void RemoveFamily(string familyId);
}

// 族元数据模型
public class FamilyMetadata
{
    public string Id { get; }
    public string Name { get; }
    public string Category { get; }
    public IEnumerable<string> Tags { get; }
    public IDictionary<string, Parameter> Parameters { get; }
    public string Description { get; }
    public string PreviewImagePath { get; }
    public string CreatedBy { get; }
    public DateTime LastModified { get; }
    
    // 值对象模式，通过构造函数确保完整性
    public FamilyMetadata(
        string id, string name, string category, 
        IEnumerable<string> tags, IDictionary<string, Parameter> parameters,
        string description, string previewImagePath,
        string createdBy, DateTime lastModified)
    {
        // 验证必填字段
        if (string.IsNullOrEmpty(id)) throw new ArgumentException("Id不能为空");
        if (string.IsNullOrEmpty(name)) throw new ArgumentException("Name不能为空");
        if (string.IsNullOrEmpty(category)) throw new ArgumentException("Category不能为空");
        
        Id = id;
        Name = name;
        Category = category;
        Tags = tags?.ToList() ?? new List<string>();
        Parameters = parameters ?? new Dictionary<string, Parameter>();
        Description = description;
        PreviewImagePath = previewImagePath;
        CreatedBy = createdBy;
        LastModified = lastModified;
    }
}
```

#### 4.3.2 领域服务接口

```csharp
// 族库仓储接口(领域层定义，基础设施层实现)
public interface IFamilyRepository
{
    Task<IEnumerable<IFamilyLibrary>> GetAllLibrariesAsync();
    Task<FamilyMetadata> GetFamilyByIdAsync(string familyId);
    Task<IEnumerable<FamilyMetadata>> SearchFamiliesAsync(SearchCriteria criteria);
    Task UpdateFamilyMetadataAsync(string familyId, FamilyMetadata metadata);
}

// 族库搜索服务
public interface IFamilySearchService
{
    Task<SearchResult<FamilyMetadata>> SearchByNaturalLanguageAsync(
        string query, int maxResults = 20);
    
    Task<SearchResult<FamilyMetadata>> SearchByTagsAsync(
        IEnumerable<string> tags, int maxResults = 20);
    
    Task<SearchResult<FamilyMetadata>> SearchByCriteriaAsync(
        SearchCriteria criteria, int maxResults = 20);
}
```

#### 4.3.3 应用服务

```csharp
// 族库查询处理器
public class FamilySearchQueryHandler
{
    private readonly IFamilySearchService _searchService;
    private readonly IQueryParser _queryParser;
    
    public FamilySearchQueryHandler(
        IFamilySearchService searchService,
        IQueryParser queryParser)
    {
        _searchService = searchService;
        _queryParser = queryParser;
    }
    
    public async Task<SearchResult<FamilyMetadata>> HandleSearchQueryAsync(
        string naturalLanguageQuery)
    {
        // 解析查询
        var searchCriteria = _queryParser.ParseFamilySearchQuery(naturalLanguageQuery);
        
        // 执行搜索
        return await _searchService.SearchByCriteriaAsync(searchCriteria);
    }
}
```

#### 4.3.4 基础设施实现

```csharp
// 本地族库实现
public class LocalFamilyLibrary : IFamilyLibrary
{
    private readonly string _libraryPath;
    private readonly IDictionary<string, FamilyMetadata> _familiesCache;
    private readonly IFileSystem _fileSystem;
    private readonly IMetadataParser _metadataParser;
    
    public string Name { get; }
    public LibraryType Type => LibraryType.Local;
    
    public LocalFamilyLibrary(
        string name,
        string libraryPath,
        IFileSystem fileSystem,
        IMetadataParser metadataParser)
    {
        Name = name;
        _libraryPath = libraryPath;
        _fileSystem = fileSystem;
        _metadataParser = metadataParser;
        _familiesCache = new Dictionary<string, FamilyMetadata>();
        
        // 初始化缓存
        Initialize();
    }
    
    private void Initialize()
    {
        foreach (var familyFile in _fileSystem.GetFiles(_libraryPath, "*.rfa"))
        {
            var metadataFile = Path.ChangeExtension(familyFile, ".json");
            if (_fileSystem.FileExists(metadataFile))
            {
                var metadata = _metadataParser.ParseMetadata(metadataFile);
                _familiesCache[metadata.Id] = metadata;
            }
        }
    }
    
    public IEnumerable<FamilyMetadata> GetFamilies()
    {
        return _familiesCache.Values;
    }
    
    public void AddFamily(FamilyMetadata family)
    {
        _familiesCache[family.Id] = family;
        // 保存元数据
        var metadataFile = Path.Combine(_libraryPath, $"{family.Id}.json");
        _metadataParser.SaveMetadata(family, metadataFile);
    }
    
    public void RemoveFamily(string familyId)
    {
        if (_familiesCache.ContainsKey(familyId))
        {
            _familiesCache.Remove(familyId);
            // 删除元数据文件
            var metadataFile = Path.Combine(_libraryPath, $"{familyId}.json");
            if (_fileSystem.FileExists(metadataFile))
            {
                _fileSystem.DeleteFile(metadataFile);
            }
        }
    }
}
```

### 4.4 命令字典/LLM Schema自动导出模块

该模块负责自动导出和同步命令字典/LLM Schema，确保LLM与Server端参数表一致，支持自然语言驱动的智能建模。

#### 4.4.1 领域模型

```csharp
// 命令Schema定义
public class CommandSchema
{
    public string ElementType { get; }
    public string FamilyName { get; }
    public IReadOnlyDictionary<string, ParameterDefinition> Parameters { get; }
    public DateTime LastUpdated { get; }

    public CommandSchema(string elementType, string familyName, IDictionary<string, ParameterDefinition> parameters, DateTime lastUpdated)
    {
        ElementType = elementType;
        FamilyName = familyName;
        Parameters = new ReadOnlyDictionary<string, ParameterDefinition>(parameters);
        LastUpdated = lastUpdated;
    }
}

// 参数定义
public class ParameterDefinition
{
    public string Name { get; }
    public string Type { get; }
    public string Unit { get; }
    public bool Required { get; }
    public string Description { get; }

    public ParameterDefinition(string name, string type, string unit, bool required, string description)
    {
        Name = name;
        Type = type;
        Unit = unit;
        Required = required;
        Description = description;
    }
}
```

#### 4.4.2 领域服务接口

```csharp
// 命令Schema导出服务接口
public interface ICommandSchemaExportService
{
    Task<IEnumerable<CommandSchema>> GetAllSchemasAsync();
    Task ExportSchemasAsync(string outputPath, SchemaExportFormat format);
}

public enum SchemaExportFormat
{
    Json,
    Markdown
}
```

#### 4.4.3 应用服务

```csharp
// 命令Schema导出处理器
public class CommandSchemaExportHandler
{
    private readonly ICommandSchemaExportService _exportService;

    public CommandSchemaExportHandler(ICommandSchemaExportService exportService)
    {
        _exportService = exportService;
    }

    public async Task ExportAllSchemasAsync(string outputPath, SchemaExportFormat format)
    {
        await _exportService.ExportSchemasAsync(outputPath, format);
    }
}
```

#### 4.4.4 基础设施实现

```csharp
// JSON格式Schema导出实现
public class JsonCommandSchemaExporter : ICommandSchemaExportService
{
    private readonly IFamilyRepository _familyRepository;

    public JsonCommandSchemaExporter(IFamilyRepository familyRepository)
    {
        _familyRepository = familyRepository;
    }

    public async Task<IEnumerable<CommandSchema>> GetAllSchemasAsync()
    {
        var families = await _familyRepository.GetAllLibrariesAsync();
        var schemas = new List<CommandSchema>();
        foreach (var lib in families)
        {
            foreach (var family in lib.GetFamilies())
            {
                var paramDefs = family.Parameters.ToDictionary(
                    p => p.Key,
                    p => new ParameterDefinition(p.Key, p.Value.Type, p.Value.Unit, p.Value.Required, p.Value.Description));
                schemas.Add(new CommandSchema(family.Category, family.Name, paramDefs, family.LastModified));
            }
        }
        return schemas;
    }

    public async Task ExportSchemasAsync(string outputPath, SchemaExportFormat format)
    {
        var schemas = await GetAllSchemasAsync();
        if (format == SchemaExportFormat.Json)
        {
            var json = JsonConvert.SerializeObject(schemas, Formatting.Indented);
            File.WriteAllText(outputPath, json);
        }
        // 可扩展Markdown等格式
    }
}
```

#### 4.4.5 典型导出格式

```json
{
  "Wall": {
    "Parameters": {
      "Length": { "Type": "number", "Unit": "mm", "Required": true, "Description": "墙体长度" },
      "Height": { "Type": "number", "Unit": "mm", "Required": true, "Description": "墙体高度" },
      "Thickness": { "Type": "number", "Unit": "mm", "Required": false, "Description": "墙体厚度" }
    }
  },
  "Door": {
    "Parameters": {
      "Width": { "Type": "number", "Unit": "mm", "Required": true, "Description": "门宽度" },
      "Height": { "Type": "number", "Unit": "mm", "Required": true, "Description": "门高度" },
      "WallId": { "Type": "string", "Required": true, "Description": "所属墙ID" }
    }
  }
  // ...
}
```

#### 4.4.6 自动触发机制
- 每当族库发生变更（如新增/删除/修改族或参数），系统会自动扫描所有族及其参数，生成最新的命令字典/LLM Schema。
- 支持通过API、命令行或定时任务触发导出，保证LLM与Server端Schema实时同步。
- 该机制与FamilyMetadata等族参数表紧密集成，族库的任何变更都会实时反映到命令字典/LLM Schema中，确保LLM和所有下游系统获取到的参数定义始终与实际族库一致。

### 4.5 变更管理模块

该模块负责管理Revit模型中的变更，支持变更模型分离、可视化和追踪功能。

#### 4.5.1 领域模型

```csharp
// 变更范围定义
public class ChangeScope
{
    // 变更范围内的元素ID
    public IEnumerable<ElementId> ElementsInScope { get; }
    
    // 空间边界（可选）
    public BoundingBoxXYZ SpatialBoundary { get; }
    
    // 变更原因描述
    public string ChangeReason { get; }
    
    // 变更负责人
    public string ResponsiblePerson { get; }
    
    // 变更识别码
    public string ChangeIdentifier { get; }
    
    // 值对象模式，不可变
    public ChangeScope(
        IEnumerable<ElementId> elementsInScope,
        BoundingBoxXYZ spatialBoundary,
        string changeReason,
        string responsiblePerson,
        string changeIdentifier)
    {
        ElementsInScope = elementsInScope?.ToList() ?? new List<ElementId>();
        SpatialBoundary = spatialBoundary;
        ChangeReason = changeReason ?? string.Empty;
        ResponsiblePerson = responsiblePerson ?? string.Empty;
        ChangeIdentifier = changeIdentifier ?? Guid.NewGuid().ToString();
    }
}

// 分离选项
public class SeparationOptions
{
    // 是否包含关联元素
    public bool IncludeRelatedElements { get; }
    
    // 关联深度级别
    public int RelationDepthLevel { get; }
    
    // 是否包含共享族
    public bool IncludeSharedFamilies { get; }
    
    // 是否保留视图设置
    public bool PreserveViewSettings { get; }
    
    // 输出路径
    public string OutputPath { get; }
    
    // 命名规则
    public string NamingPattern { get; }
    
    // 使用Builder模式创建实例
    public class Builder
    {
        private bool _includeRelatedElements = true;
        private int _relationDepthLevel = 1;
        private bool _includeSharedFamilies = true;
        private bool _preserveViewSettings = false;
        private string _outputPath;
        private string _namingPattern = "{OriginalName}_{ChangeId}_{Date}";
        
        public Builder IncludeRelatedElements(bool include)
        {
            _includeRelatedElements = include;
            return this;
        }
        
        public Builder WithRelationDepthLevel(int level)
        {
            _relationDepthLevel = level > 0 ? level : 1;
            return this;
        }
        
        public Builder IncludeSharedFamilies(bool include)
        {
            _includeSharedFamilies = include;
            return this;
        }
        
        public Builder PreserveViewSettings(bool preserve)
        {
            _preserveViewSettings = preserve;
            return this;
        }
        
        public Builder WithOutputPath(string path)
        {
            _outputPath = path;
            return this;
        }
        
        public Builder WithNamingPattern(string pattern)
        {
            _namingPattern = !string.IsNullOrEmpty(pattern) 
                ? pattern 
                : "{OriginalName}_{ChangeId}_{Date}";
            return this;
        }
        
        public SeparationOptions Build()
        {
            if (string.IsNullOrEmpty(_outputPath))
            {
                throw new ArgumentException("输出路径不能为空");
            }
            
            return new SeparationOptions(
                _includeRelatedElements,
                _relationDepthLevel,
                _includeSharedFamilies,
                _preserveViewSettings,
                _outputPath,
                _namingPattern);
        }
    }
    
    // 私有构造函数，强制使用Builder
    private SeparationOptions(
        bool includeRelatedElements,
        int relationDepthLevel,
        bool includeSharedFamilies,
        bool preserveViewSettings,
        string outputPath,
        string namingPattern)
    {
        IncludeRelatedElements = includeRelatedElements;
        RelationDepthLevel = relationDepthLevel;
        IncludeSharedFamilies = includeSharedFamilies;
        PreserveViewSettings = preserveViewSettings;
        OutputPath = outputPath;
        NamingPattern = namingPattern;
    }
}

// 分离结果
public class SeparationResult
{
    // 原始模型
    public DocumentInfo OriginalModel { get; }
    
    // 变更范围模型
    public DocumentInfo ChangeScopeModel { get; }
    
    // 非变更范围模型
    public DocumentInfo NonChangeScopeModel { get; }
    
    // 处理的元素数量
    public int ProcessedElementCount { get; }
    
    // 变更元素数量
    public int ChangedElementCount { get; }
    
    // 分离时间
    public TimeSpan ProcessingTime { get; }
    
    // 日志信息
    public IEnumerable<LogEntry> Logs { get; }
    
    // 构造函数确保完整性
    public SeparationResult(
        DocumentInfo originalModel,
        DocumentInfo changeScopeModel,
        DocumentInfo nonChangeScopeModel,
        int processedElementCount,
        int changedElementCount,
        TimeSpan processingTime,
        IEnumerable<LogEntry> logs)
    {
        OriginalModel = originalModel ?? throw new ArgumentNullException(nameof(originalModel));
        ChangeScopeModel = changeScopeModel ?? throw new ArgumentNullException(nameof(changeScopeModel));
        NonChangeScopeModel = nonChangeScopeModel;
        ProcessedElementCount = processedElementCount;
        ChangedElementCount = changedElementCount;
        ProcessingTime = processingTime;
        Logs = logs?.ToList() ?? new List<LogEntry>();
    }
}
```

#### 4.5.2 领域服务

```csharp
// 变更模型分离服务接口
public interface IChangeModelSeparationService
{
    // 定义变更范围
    Task<ChangeScope> DefineChangeScopeAsync(
        DefineChangeScopeOptions options);
        
    // 分离变更模型
    Task<SeparationResult> SeparateChangeModelAsync(
        Document document,
        ChangeScope changeScope,
        SeparationOptions options,
        IProgress<ProgressInfo> progress = null);
        
    // 生成变更后模型
    Task<Document> GenerateChangedModelAsync(
        Document originalDocument,
        IEnumerable<ElementId> changedElementIds,
        GenerationOptions options,
        IProgress<ProgressInfo> progress = null);
        
    // 比较变更前后模型
    Task<ComparisonResult> CompareModelsAsync(
        Document beforeModel,
        Document afterModel,
        ComparisonOptions options);
}

// 元素依赖分析服务接口
public interface IElementDependencyAnalyzer
{
    // 分析元素依赖关系
    Task<ElementDependencies> AnalyzeDependenciesAsync(
        Document document, 
        ElementId elementId, 
        int depthLevel = 1);
    
    // 查找所有依赖元素（递归）
    Task<IEnumerable<ElementId>> FindAllDependenciesAsync(
        Document document,
        IEnumerable<ElementId> rootElementIds,
        int maxDepth = 1);
}
```

#### 4.5.3 应用服务

```csharp
// 变更模型分离命令处理器
public class ChangeSeparationCommandHandler
{
    private readonly IChangeModelSeparationService _separationService;
    private readonly IDocumentManager _documentManager;
    private readonly ICommandParser _commandParser;
    
    public ChangeSeparationCommandHandler(
        IChangeModelSeparationService separationService,
        IDocumentManager documentManager,
        ICommandParser commandParser)
    {
        _separationService = separationService;
        _documentManager = documentManager;
        _commandParser = commandParser;
    }
    
    public async Task<SeparationResult> HandleSeparationCommandAsync(
        string naturalLanguageCommand,
        IProgress<ProgressInfo> progress = null)
    {
        // 解析命令
        var command = _commandParser.ParseChangeSeparationCommand(naturalLanguageCommand);
        
        // 获取当前文档
        var document = _documentManager.GetCurrentDocument();
        if (document == null)
        {
            throw new InvalidOperationException("没有打开的文档");
        }
        
        // 定义变更范围
        var changeScope = await _separationService.DefineChangeScopeAsync(command.ScopeOptions);
        
        // 构建分离选项
        var separationOptions = new SeparationOptions.Builder()
            .IncludeRelatedElements(command.IncludeRelatedElements)
            .WithRelationDepthLevel(command.RelationDepthLevel)
            .IncludeSharedFamilies(command.IncludeSharedFamilies)
            .PreserveViewSettings(command.PreserveViewSettings)
            .WithOutputPath(command.OutputPath)
            .WithNamingPattern(command.NamingPattern)
            .Build();
        
        // 执行分离
        return await _separationService.SeparateChangeModelAsync(
            document, changeScope, separationOptions, progress);
    }
}
```

#### 4.5.4 基础设施实现

```csharp
// 变更模型分离服务实现
public class ChangeModelSeparationService : IChangeModelSeparationService
{
    private readonly IElementDependencyAnalyzer _dependencyAnalyzer;
    private readonly ITransactionManager _transactionManager;
    private readonly IRevitAPIAdapter _revitAPI;
    private readonly ILogger<ChangeModelSeparationService> _logger;
    
    public ChangeModelSeparationService(
        IElementDependencyAnalyzer dependencyAnalyzer,
        ITransactionManager transactionManager,
        IRevitAPIAdapter revitAPI,
        ILogger<ChangeModelSeparationService> logger)
    {
        _dependencyAnalyzer = dependencyAnalyzer;
        _transactionManager = transactionManager;
        _revitAPI = revitAPI;
        _logger = logger;
    }
    
    public async Task<ChangeScope> DefineChangeScopeAsync(DefineChangeScopeOptions options)
    {
        // 实现变更范围定义逻辑
        _logger.LogInformation("定义变更范围：{ChangeReason}", options.ChangeReason);
        
        // 示例实现
        var elementsInScope = await _revitAPI.FindElementsByFilterAsync(options.ElementFilter);
        
        return new ChangeScope(
            elementsInScope,
            options.SpatialBoundary,
            options.ChangeReason,
            options.ResponsiblePerson,
            options.ChangeIdentifier ?? Guid.NewGuid().ToString());
    }
    
    public async Task<SeparationResult> SeparateChangeModelAsync(
        Document document,
        ChangeScope changeScope,
        SeparationOptions options,
        IProgress<ProgressInfo> progress = null)
    {
        _logger.LogInformation("开始分离变更模型：{ChangeId}", changeScope.ChangeIdentifier);
        var stopwatch = Stopwatch.StartNew();
        var logs = new List<LogEntry>();
        
        try
        {
            // 报告开始进度
            progress?.Report(new ProgressInfo(0, "准备分离变更模型"));
            
            // 收集所有需要处理的元素
            var allElementsToProcess = new HashSet<ElementId>();
            
            // 添加变更范围内的元素
            foreach (var elementId in changeScope.ElementsInScope)
            {
                allElementsToProcess.Add(elementId);
            }
            
            // 如果需要包含关联元素，则分析依赖关系
            if (options.IncludeRelatedElements)
            {
                progress?.Report(new ProgressInfo(5, "分析元素依赖关系"));
                var dependencies = await _dependencyAnalyzer.FindAllDependenciesAsync(
                    document, changeScope.ElementsInScope, options.RelationDepthLevel);
                
                foreach (var dependencyId in dependencies)
                {
                    allElementsToProcess.Add(dependencyId);
                }
                
                logs.Add(new LogEntry(LogLevel.Information, 
                    $"找到 {dependencies.Count()} 个关联元素"));
            }
            
            // 报告进度
            progress?.Report(new ProgressInfo(20, "创建变更范围模型"));
            
            // 保存原始模型信息
            var originalModel = new DocumentInfo(
                document.Title,
                document.PathName,
                document.ActiveView?.Name);
            
            // 创建变更范围模型
            var changeScopeModelPath = Path.Combine(
                options.OutputPath,
                FormatFileName(options.NamingPattern, document, "ChangeScope", changeScope.ChangeIdentifier));
            
            var changeScopeModel = await CreateChangeModelAsync(
                document, allElementsToProcess, changeScopeModelPath, options);
            
            progress?.Report(new ProgressInfo(60, "创建非变更范围模型"));
            
            // 创建非变更范围模型
            DocumentInfo nonChangeScopeModel = null;
            if (allElementsToProcess.Count < document.Elements.Cast<Element>().Count())
            {
                var nonChangeScopeModelPath = Path.Combine(
                    options.OutputPath,
                    FormatFileName(options.NamingPattern, document, "NonChangeScope", changeScope.ChangeIdentifier));
                
                // 获取不在变更范围内的元素
                var nonChangeScopeElements = document.Elements
                    .Cast<Element>()
                    .Where(e => !allElementsToProcess.Contains(e.Id))
                    .Select(e => e.Id)
                    .ToList();
                
                nonChangeScopeModel = await CreateChangeModelAsync(
                    document, nonChangeScopeElements, nonChangeScopeModelPath, options);
            }
            
            progress?.Report(new ProgressInfo(100, "变更模型分离完成"));
            
            // 创建结果
            return new SeparationResult(
                originalModel,
                changeScopeModel,
                nonChangeScopeModel,
                allElementsToProcess.Count,
                changeScope.ElementsInScope.Count(),
                stopwatch.Elapsed,
                logs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "分离变更模型失败");
            logs.Add(new LogEntry(LogLevel.Error, $"错误：{ex.Message}"));
            throw;
        }
        finally
        {
            stopwatch.Stop();
        }
    }
    
    // 其它方法实现...
    
    // 辅助方法：格式化文件名
    private string FormatFileName(string pattern, Document document, string modelType, string changeId)
    {
        return pattern
            .Replace("{OriginalName}", Path.GetFileNameWithoutExtension(document.Title))
            .Replace("{ChangeId}", changeId)
            .Replace("{Date}", DateTime.Now.ToString("yyyyMMdd_HHmmss"))
            .Replace("{ModelType}", modelType)
            + ".rvt";
    }
    
    // 辅助方法：创建变更模型
    private async Task<DocumentInfo> CreateChangeModelAsync(
        Document sourceDocument,
        IEnumerable<ElementId> elementsToInclude,
        string outputPath,
        SeparationOptions options)
    {
        // 实现创建模型的逻辑...
        // 此处简化为记录日志
        _logger.LogInformation("创建模型：{OutputPath}", outputPath);
        
        // 模拟异步操作
        await Task.Delay(100);
        
        // 返回模拟的结果
        return new DocumentInfo(
            Path.GetFileName(outputPath),
            outputPath,
            "默认3D视图");
    }
}
```

## 5. 测试策略

RevitMCP项目采用全面的测试策略，确保代码质量和功能稳定性。

### 5.1 测试层次

1. **单元测试**
   - 针对独立组件的测试
   - 使用模拟（Mock）隔离依赖
   - 验证业务逻辑和规则

2. **集成测试**
   - 测试组件协作
   - 验证跨进程通信
   - 检查数据流

3. **系统测试**
   - 端到端测试
   - 性能测试
   - 安全测试

### 5.2 单元测试示例

```csharp
// 使用xUnit和Moq进行单元测试
public class ModelQueryHandlerTests
{
    [Fact]
    public async Task HandleQueryAsync_ValidQuery_ReturnsSuccessResult()
    {
        // Arrange
        var mockQueryService = new Mock<IModelQueryService>();
        var mockQueryParser = new Mock<IQueryParser>();
        
        var testQuery = new ModelQuery(
            "查找所有墙", 
            QueryType.ElementSelection,
            new Dictionary<string, object>(), 
            "当前文档");
            
        var expectedResult = QueryResult.CreateSuccess(
            "找到10个墙", 
            new List<ElementInfo>());
            
        mockQueryParser
            .Setup(p => p.ParseQuery(It.IsAny<string>()))
            .Returns(testQuery);
            
        mockQueryService
            .Setup(s => s.ExecuteQueryAsync(It.IsAny<ModelQuery>()))
            .ReturnsAsync(expectedResult);
            
        var handler = new ModelQueryHandler(
            mockQueryService.Object,
            mockQueryParser.Object);
            
        // Act
        var result = await handler.HandleQueryAsync("查找所有墙");
        
        // Assert
        Assert.True(result.Success);
        Assert.Equal("找到10个墙", result.ResultText);
        
        // 验证方法调用
        mockQueryParser.Verify(
            p => p.ParseQuery("查找所有墙"), 
            Times.Once);
            
        mockQueryService.Verify(
            s => s.ExecuteQueryAsync(testQuery), 
            Times.Once);
    }
    
    [Fact]
    public async Task HandleQueryAsync_ExceptionThrown_ReturnsFailureResult()
    {
        // Arrange
        var mockQueryService = new Mock<IModelQueryService>();
        var mockQueryParser = new Mock<IQueryParser>();
        
        mockQueryParser
            .Setup(p => p.ParseQuery(It.IsAny<string>()))
            .Throws(new FormatException("无效查询格式"));
            
        var handler = new ModelQueryHandler(
            mockQueryService.Object,
            mockQueryParser.Object);
            
        // Act
        var result = await handler.HandleQueryAsync("无效查询");
        
        // Assert
        Assert.False(result.Success);
        Assert.IsType<FormatException>(result.Error);
        Assert.Equal("无效查询格式", result.Error.Message);
    }
}
```

### 5.3 集成测试策略

为了测试Revit API交互，采用特殊的测试框架：

```csharp
public class RevitAPIMockFixture : IDisposable
{
    public Mock<IRevitAPIAdapter> RevitAPIMock { get; }
    
    public RevitAPIMockFixture()
    {
        RevitAPIMock = new Mock<IRevitAPIAdapter>();
        // 设置通用模拟行为
    }
    
    public void Dispose()
    {
        // 清理资源
    }
}

// 使用测试固件的集成测试
public class ModelModificationServiceIntegrationTests : IClassFixture<RevitAPIMockFixture>
{
    private readonly RevitAPIMockFixture _fixture;
    
    public ModelModificationServiceIntegrationTests(RevitAPIMockFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Fact]
    public async Task ModifyParametersAsync_ValidParameters_SuccessfullyApplied()
    {
        // Arrange
        var mockTransactionManager = new Mock<ITransactionManager>();
        mockTransactionManager
            .Setup(t => t.Execute(It.IsAny<Action>(), It.IsAny<string>()))
            .Callback<Action, string>((action, name) => action());
            
        var service = new ModelModificationService(
            _fixture.RevitAPIMock.Object,
            mockTransactionManager.Object);
            
        // 模拟元素查找和参数修改
        _fixture.RevitAPIMock
            .Setup(a => a.GetElementAsync(It.IsAny<string>()))
            .ReturnsAsync((string id) => new ElementInfo { Id = id, Name = "测试元素" });
            
        _fixture.RevitAPIMock
            .Setup(a => a.SetParameterValueAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>()))
            .ReturnsAsync(true);
            
        // Act
        var result = await service.ModifyParametersAsync(
            new[] { "element1", "element2" },
            new Dictionary<string, object> { { "Height", 2000 }, { "Width", 1000 } });
            
        // Assert
        Assert.True(result.Success);
        Assert.Equal(2, result.ModifiedElementIds.Count());
        
        // 验证参数修改
        _fixture.RevitAPIMock.Verify(
            a => a.SetParameterValueAsync("element1", "Height", 2000),
            Times.Once);
            
        _fixture.RevitAPIMock.Verify(
            a => a.SetParameterValueAsync("element1", "Width", 1000),
            Times.Once);
    }
}
```

## 6. 安全与性能设计

### 6.1 安全设计

系统的安全设计确保数据完整性和用户操作安全：

1. **输入验证**
   - 验证自然语言输入，防止恶意命令
   - 验证所有API参数，防止注入攻击
   - 使用白名单验证允许的操作

2. **权限控制**
   - 实现基于角色的访问控制
   - 限制敏感操作的执行
   - 记录所有关键操作

3. **事务管理**
   - 所有模型修改在事务中执行
   - 提供回滚机制
   - 确保操作的原子性

### 6.2 性能优化

系统的性能优化策略确保在处理大型模型时仍能保持良好性能：

1. **数据缓存**
   - 缓存常用元素和参数数据
   - 实现多级缓存策略
   - 智能缓存刷新机制

2. **异步处理**
   - 长时间运行的操作实现为异步
   - 提供进度反馈机制
   - 支持操作取消

3. **批量操作**
   - 合并多个操作减少API调用
   - 优化事务使用
   - 懒加载技术

4. **资源管理**
   - 内存使用优化
   - 资源池化
   - 及时释放未使用资源

## 7. 实现路线图

### 7.1 第一阶段：核心功能实现（3-6个月）

- 建立基础架构和MCP服务器
- 实现基本的自然语言查询功能
- 实现基础的模型修改功能
- 实现基本的族库管理功能
- 完成核心功能的测试和优化

### 7.2 第二阶段：扩展功能实现（6-9个月）

- 实现高级模型修改功能
- 实现变更管理功能
- 实现辅助读图功能
- 扩展族库管理功能
- 优化性能和用户体验

### 7.3 第三阶段：可选功能和集成（9-12个月）

- 实现基础工作流自动化功能
- 开发基础视图和文档管理功能
- 增强与外部系统的集成
- 完善文档和示例
- 进行全面测试和优化

### 7.4 未来扩展：工程量计算功能（12个月后）

- 基于预留的接口设计实现具体功能
- 实现多种计算方法支持
- 支持主要构件类型的工程量提取
- 实现基础的数据导出功能
- 提供简单的汇总和统计功能

## 8. 结论

RevitMCP项目通过领域驱动设计(DDD)架构和严格遵循设计原则，构建了一个高质量、可扩展的系统，使用户能够通过自然语言与Revit进行交互。系统的关键特性包括：

1. **清晰的分层架构**
   - 表示层、应用层、领域层和基础设施层职责清晰
   - 确保业务逻辑与技术实现分离
   - 便于测试和维护

2. **设计原则贯穿始终**
   - SOLID原则确保代码高内聚低耦合
   - 迪米特法则减少对象间依赖
   - 合成复用原则优先使用组合实现代码复用

3. **灵活的技术架构**
   - 通过跨进程通信解决.NET版本兼容性问题
   - 模块化设计便于功能扩展
   - 统一的命名规范和目录结构

4. **完善的测试策略**
   - 多层次测试确保代码质量
   - 模拟框架隔离外部依赖
   - 自动化测试提高开发效率

通过这些设计和实现，RevitMCP将为建筑设计和施工领域带来革命性的工作方式变革，显著提升用户的工作效率和体验。 