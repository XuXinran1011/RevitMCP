# RevitMCP设计指南

> 本文件用于说明项目的设计原则、架构决策、接口设计和扩展性。

## 1. 系统架构概览

RevitMCP是基于Model Context Protocol (MCP)的实现，旨在使设计师和建造师能够通过自然语言与Autodesk Revit进行交互。系统采用领域驱动设计(DDD)的分层架构，由以下几个关键组件组成：

```
┌─────────────────┐      ┌─────────────────┐      ┌─────────────────┐
│                 │      │                 │      │                 │
│  LLM客户端      │◄────►│  RevitMCP       │◄────►│  Revit          │
│  (Claude等)     │      │  服务器         │      │  应用程序       │
│                 │      │                 │      │                 │
└─────────────────┘      └─────────────────┘      └─────────────────┘
```

### 1.1 主要组件

1. **LLM客户端**：处理用户交互和自然语言处理，使用MCP协议与RevitMCP服务器通信
2. **RevitMCP服务器**：实现MCP规范的核心组件，将自然语言请求转换为Revit API操作
3. **Revit集成**：连接到Revit API，在Revit模型上执行命令和查询

### 1.2 技术架构

系统使用跨进程通信解决.NET版本兼容性挑战：

```
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

## 2. 项目结构与组织

RevitMCP项目包含三个主要组件，每个组件遵循领域驱动设计的分层架构：

1. **RevitMCP.Plugin**（.NET 8）：Revit插件项目
2. **RevitMCP.Server**（.NET 9）：MCP服务器项目
3. **RevitMCP.Shared**（netstandard2.0）：共享库项目

## 3. 领域模型与模块组织

### 3.1 核心领域模型

系统采用轻量级数据提供模式，核心领域模型包括：

#### 3.1.1 元素核心标识模型

```csharp
public class ElementCoreIdentifier
{
    public string RevitElementId { get; set; }       // Revit内部元素ID
    public string GlobalUniqueId { get; set; }       // 全局唯一标识符
    public string ElementCategory { get; set; }      // 元素类别
    public string ElementType { get; set; }          // 元素类型
    public string FamilyName { get; set; }           // 族名称
    public string ElementName { get; set; }          // 元素名称
}
```

#### 3.1.2 元素位置模型

```csharp
public class ElementLocationInfo
{
    public string Level { get; set; }                // 所在楼层
    public string Zone { get; set; }                 // 区域代码
    public BoundingBox BoundingBox { get; set; }     // 包围盒
    public double? Rotation { get; set; }            // 旋转角度
}
```

#### 3.1.3 元素引用和状态模型

```csharp
public class ElementReferenceInfo
{
    public string ExternalSystemId { get; set; }     // 外部系统ID
    public string StatusCode { get; set; }           // 状态代码
    public DateTime LastModified { get; set; }       // 最后修改时间
    public string LastModifiedBy { get; set; }       // 最后修改用户
}
```

#### 3.1.4 元素关系模型

```csharp
public class ElementBasicRelations
{
    public string HostElementId { get; set; }        // 宿主元素ID
    public List<string> ConnectedElementIds { get; set; } // 直接连接的元素ID列表
    public string ContainerElementId { get; set; }   // 容器元素ID
    public string SystemId { get; set; }             // 系统关系
}
```

### 3.2 模块组织

RevitMCP系统按照DDD原则组织为以下模块：

#### 3.2.1 表示层 (Presentation)

**功能**：处理用户界面和交互，实现MCP协议接口，处理自然语言输入输出

**主要组件**：
- **MCP服务器接口**：处理与LLM客户端的通信
- **API接口**：提供RESTful API访问
- **提示模板**：管理自然语言交互的提示模板

**设计原则应用**：
- **单一职责原则(SRP)**：每个接口类仅负责特定类型的交互
- **接口隔离原则(ISP)**：不同类型的客户端面对专用的接口
- **开闭原则(OCP)**：通过提示模板和抽象接口，支持扩展而不修改现有代码

#### 3.2.2 应用层 (Application)

**功能**：协调领域对象完成用例，处理工作流和业务流程

**主要组件**：
- **命令处理器**：将MCP工具调用转换为领域操作
- **查询处理器**：处理模型查询请求
- **应用服务**：实现业务用例

**设计原则应用**：
- **命令查询责任分离(CQRS)**：明确区分命令和查询操作
- **迪米特法则(LoD)**：应用服务只与直接相关的组件交互
- **依赖倒置原则(DIP)**：依赖抽象接口，而非具体实现

#### 3.2.3 领域层 (Domain)

**功能**：包含核心业务逻辑，定义领域模型和规则

**主要组件**：
- **领域模型**：核心业务实体和值对象
- **领域服务**：实现复杂业务逻辑
- **仓储接口**：定义数据访问抽象
- **领域事件**：定义业务事件

**设计原则应用**：
- **丰富的领域模型**：实体包含行为，不仅是数据容器
- **值对象不变性**：确保值对象的不可变性
- **聚合边界**：明确定义聚合根和一致性边界

#### 3.2.4 基础设施层 (Infrastructure)

**功能**：提供技术实现，集成外部系统，处理持久化

**主要组件**：
- **Revit API适配器**：与Revit API的交互
- **数据仓储实现**：实现领域仓储接口
- **MCP协议实现**：实现MCP通信
- **外部服务集成**：与外部系统的集成

**设计原则应用**：
- **适配器模式**：通过适配器隔离外部API依赖
- **仓储模式**：隐藏数据访问细节
- **依赖注入**：运行时提供依赖实现

## 4. 功能模块详细设计

### 4.1 自然语言查询模块

**主要功能**：
- 模型元素查询
- 参数和数据提取
- 视图和文档查询

**核心组件**：

1. **查询解析器** (应用层)
```csharp
public interface IQueryParser
{
    QueryRequest ParseNaturalLanguageQuery(string naturalLanguageQuery);
}
```

2. **元素查询服务** (领域层)
```csharp
public interface IElementQueryService
{
    IEnumerable<ElementCoreIdentifier> FindElementsByCategory(string category, QueryFilter filter);
    IEnumerable<ElementCoreIdentifier> FindElementsByParameter(string parameterName, object value, QueryFilter filter);
    ElementDetails GetElementDetails(string elementId);
}
```

3. **查询过滤器** (领域层)
```csharp
public class QueryFilter
{
    public string Level { get; set; }
    public string Zone { get; set; }
    public Dictionary<string, object> ParameterFilters { get; set; }
    public BoundingBox SpatialFilter { get; set; }
}
```

### 4.2 模型修改模块

**主要功能**：
- 元素参数修改
- 简单元素创建
- 基本元素关系管理

**核心组件**：

1. **命令解析器** (应用层)
```csharp
public interface ICommandParser
{
    ModelCommand ParseNaturalLanguageCommand(string naturalLanguageCommand);
}
```

2. **元素修改服务** (领域层)
```csharp
public interface IElementModificationService
{
    void ModifyElementParameter(string elementId, string parameterName, object value);
    void ModifyElementsParameters(IEnumerable<string> elementIds, string parameterName, object value);
    string CreateElement(ElementCreationRequest request);
    void DeleteElement(string elementId);
}
```

3. **事务管理器** (基础设施层)
```csharp
public interface ITransactionManager
{
    void ExecuteInTransaction(Action action, string transactionName);
    T ExecuteInTransaction<T>(Func<T> func, string transactionName);
}
```

### 4.3 族库管理模块

**主要功能**：
- 族库索引与搜索
- 族参数查询与管理
- 基本族库组织

**核心组件**：

1. **族库索引器** (应用层)
```csharp
public interface IFamilyLibraryIndexer
{
    void IndexLibrary(string libraryPath);
    void UpdateIndex();
    SearchResult SearchFamilies(FamilySearchCriteria criteria);
}
```

2. **族管理服务** (领域层)
```csharp
public interface IFamilyManagementService
{
    FamilyDetails GetFamilyDetails(string familyId);
    void ModifyFamilyParameter(string familyId, string parameterName, object value);
    string LoadFamilyToProject(string familyPath);
}
```

3. **族搜索条件** (领域层)
```csharp
public class FamilySearchCriteria
{
    public string CategoryFilter { get; set; }
    public string NameFilter { get; set; }
    public Dictionary<string, object> ParameterFilters { get; set; }
    public string[] Tags { get; set; }
}
```

### 4.4 API接口模块

**主要功能**：
- 元素核心信息API
- 元素更新API
- 映射管理API
- 同步API

**核心组件**：

1. **元素控制器** (表示层)
```csharp
public class ElementController
{
    public ElementCoreIdentifier GetElementCore(string guid);
    public ElementLocationInfo GetElementLocation(string guid);
    public ElementBasicRelations GetElementRelations(string guid);
    public void UpdateElementStatus(string guid, string statusCode);
    public List<ElementCoreIdentifier> GetFilteredElements(ElementFilterParams filterParams);
}
```

2. **映射控制器** (表示层)
```csharp
public class MappingController
{
    public void CreateMapping(MappingRequest request);
    public MappingInfo GetMappingByExternalId(string externalId);
    public void CreateMappingBatch(BatchMappingRequest request);
    public void DeleteMapping(string guid);
}
```

3. **同步控制器** (表示层)
```csharp
public class SyncController
{
    public void StartSync(SyncRequest request);
    public SyncStatus GetSyncStatus();
}
```

## 5. 跨模块通信与集成

### 5.1 依赖注入框架

系统使用依赖注入容器管理组件依赖：

```csharp
public static class DependencyConfig
{
    public static void ConfigureServices(IServiceCollection services)
    {
        // 注册表示层服务
        services.AddScoped<IMCPToolHandler, MCPToolHandler>();
        
        // 注册应用层服务
        services.AddScoped<IQueryParser, NaturalLanguageQueryParser>();
        services.AddScoped<ICommandParser, NaturalLanguageCommandParser>();
        
        // 注册领域服务
        services.AddScoped<IElementQueryService, ElementQueryService>();
        services.AddScoped<IElementModificationService, ElementModificationService>();
        services.AddScoped<IFamilyManagementService, FamilyManagementService>();
        
        // 注册基础设施服务
        services.AddScoped<IRevitApiAdapter, RevitApiAdapter>();
        services.AddScoped<ITransactionManager, RevitTransactionManager>();
        services.AddScoped<IElementRepository, RevitElementRepository>();
    }
}
```

### 5.2 事件驱动通信

系统使用领域事件实现松耦合的跨模块通信：

```csharp
// 定义领域事件
public class ElementModifiedEvent : IDomainEvent
{
    public string ElementId { get; }
    public string ParameterName { get; }
    public object NewValue { get; }
    
    public ElementModifiedEvent(string elementId, string parameterName, object newValue)
    {
        ElementId = elementId;
        ParameterName = parameterName;
        NewValue = newValue;
    }
}

// 事件处理器
public class ElementModificationEventHandler : IEventHandler<ElementModifiedEvent>
{
    private readonly IExternalSystemAdapter _externalSystemAdapter;
    
    public ElementModificationEventHandler(IExternalSystemAdapter externalSystemAdapter)
    {
        _externalSystemAdapter = externalSystemAdapter;
    }
    
    public void Handle(ElementModifiedEvent @event)
    {
        // 通知外部系统元素已修改
        _externalSystemAdapter.NotifyElementModified(
            @event.ElementId, 
            @event.ParameterName, 
            @event.NewValue);
    }
}
```

### 5.3 适配器模式

系统使用适配器模式隔离外部依赖：

```csharp
// Revit API适配器接口
public interface IRevitApiAdapter
{
    Element GetElementById(string elementId);
    void SetParameterValue(Element element, string parameterName, object value);
    Element CreateWall(WallCreationData wallData);
    IEnumerable<Element> FindElementsByCategory(string category);
}

// Revit API适配器实现
public class RevitApiAdapter : IRevitApiAdapter
{
    private readonly Document _document;
    
    public RevitApiAdapter(Document document)
    {
        _document = document;
    }
    
    public Element GetElementById(string elementId)
    {
        ElementId id = new ElementId(long.Parse(elementId));
        return _document.GetElement(id);
    }
    
    public void SetParameterValue(Element element, string parameterName, object value)
    {
        Parameter param = element.LookupParameter(parameterName);
        if (param != null)
        {
            // 根据参数类型设置值
            if (param.StorageType == StorageType.Double)
            {
                param.Set((double)Convert.ChangeType(value, typeof(double)));
            }
            else if (param.StorageType == StorageType.Integer)
            {
                param.Set((int)Convert.ChangeType(value, typeof(int)));
            }
            else if (param.StorageType == StorageType.String)
            {
                param.Set(value.ToString());
            }
            // 处理其他类型...
        }
    }
    
    // 其他方法实现...
}
```

## 6. 设计原则实践

### 6.1 领域驱动设计 (DDD) 实践

1. **限界上下文**：
   - RevitMCP核心上下文
   - 族库管理上下文
   - 工程量计算上下文
   
2. **聚合根**：
   - `RevitElement`：表示Revit元素
   - `FamilyType`：表示族类型
   - `Project`：表示Revit项目

3. **值对象**：
   - `ElementLocation`：表示元素位置
   - `BoundingBox`：表示包围盒
   - `ParameterValue`：表示参数值

4. **领域服务**：
   - `ElementRelationshipService`：处理元素间关系
   - `ElementValidationService`：验证元素操作

### 6.2 SOLID原则实践

1. **单一职责原则 (SRP)**：
   - 每个类只负责一个功能领域
   - 例如：`ElementQueryService`只负责查询，`ElementModificationService`只负责修改

2. **开闭原则 (OCP)**：
   - 通过接口和抽象类扩展功能
   - 例如：新增查询类型时只需实现`IQueryStrategy`接口

3. **里氏替换原则 (LSP)**：
   - 子类可替换父类使用
   - 例如：所有`Element`的子类都遵循相同的契约

4. **接口隔离原则 (ISP)**：
   - 针对不同功能提供专用接口
   - 例如：`IReadOnlyElementRepository`和`IElementRepository`分离读写职责

5. **依赖倒置原则 (DIP)**：
   - 高层模块不依赖低层模块，都依赖于抽象
   - 例如：领域服务依赖仓储接口，而非具体实现

### 6.3 迪米特法则实践

1. **最少知识原则**：
   - 组件只与直接相关的其他组件通信
   - 例如：`ElementController`只与`ElementService`通信，不直接访问仓储

2. **中介者模式**：
   - 使用`ApplicationService`作为中介者，协调多个领域服务

3. **门面模式**：
   - 为复杂子系统提供统一接口
   - 例如：`RevitApiFacade`简化对Revit API的访问

### 6.4 合成复用原则实践

1. **优先使用组合而非继承**：
   - 使用组合实现功能复用
   - 例如：`ElementQueryService`组合使用多个查询策略

2. **策略模式**：
   - 将不同算法封装为策略类
   - 例如：不同类型的元素查询逻辑封装为不同的`QueryStrategy`

3. **装饰器模式**：
   - 动态扩展对象功能
   - 例如：使用`CachingElementRepository`装饰基本仓储实现缓存

## 7. 数据流与接口规范

### 7.1 API接口规范

1. **RESTful API设计**：
   - 使用标准HTTP方法（GET, POST, PATCH, DELETE）
   - 使用URI表示资源
   - 使用HTTP状态码表示操作结果

2. **API端点**：
```
GET /api/elements/{guid}/core           # 获取元素核心信息
GET /api/elements/{guid}/location       # 获取元素位置信息
GET /api/elements/{guid}/relations      # 获取元素关系信息
GET /api/elements/{guid}/status         # 获取元素状态信息
GET /api/elements/filtered              # 按条件筛选元素

PATCH /api/elements/{guid}/status       # 更新元素状态
PATCH /api/elements/{guid}/reference    # 更新外部引用
POST /api/elements/batch-update         # 批量更新元素

POST /api/mappings                      # 创建映射关系
GET /api/mappings/{externalId}          # 通过外部ID查询映射
POST /api/mappings/batch                # 批量创建映射
DELETE /api/mappings/{guid}             # 删除映射关系

POST /api/sync                          # 启动同步
GET /api/sync/status                    # 获取同步状态
```

### 7.2 数据交换格式

标准化的JSON数据结构用于API响应：

```json
// 元素基本信息响应
{
  "coreIdentifier": {
    "revitElementId": "123456",
    "globalUniqueId": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
    "elementCategory": "墙",
    "elementType": "基本墙",
    "familyName": "基本墙",
    "elementName": "内墙-100mm"
  },
  "locationInfo": {
    "level": "一层",
    "zone": "A区",
    "boundingBox": {
      "min": {"x": 0.0, "y": 0.0, "z": 0.0},
      "max": {"x": 5.0, "y": 3.0, "z": 3.0}
    }
  },
  "referenceInfo": {
    "externalSystemId": "EXT-12345",
    "statusCode": "IN_PROGRESS",
    "lastModified": "2023-08-15T10:30:00Z",
    "lastModifiedBy": "user123"
  }
}
```

## 8. 功能扩展机制

系统设计了灵活的扩展机制，支持功能的持续增强：

### 8.1 插件架构

使用插件架构支持功能扩展：

```csharp
public interface IRevitMcpPlugin
{
    string Name { get; }
    string Version { get; }
    void Initialize(IServiceCollection services);
    IEnumerable<IMcpTool> ProvideMcpTools();
}
```

### 8.2 工具注册机制

支持动态注册MCP工具：

```csharp
public interface IMcpToolRegistry
{
    void RegisterTool(IMcpTool tool);
    void UnregisterTool(string toolName);
    IMcpTool GetTool(string toolName);
    IEnumerable<IMcpTool> GetAllTools();
}
```

### 8.3 自定义数据提供者

支持扩展数据源：

```csharp
public interface ICustomDataProvider
{
    string ProviderName { get; }
    bool CanProvideData(string dataType);
    object GetData(string dataType, Dictionary<string, object> parameters);
}
```

## 9. 开发与测试指南

### 9.1 开发环境设置

1. **必要软件**：
   - Visual Studio 2022或更高版本
   - .NET 8 SDK（用于RevitMCP.Plugin）
   - .NET 9 SDK（用于RevitMCP.Server）
   - Autodesk Revit 2025

2. **项目配置**：
   - 设置多目标框架项目
   - 配置适当的引用路径
   - 设置调试配置

### 9.2 测试策略

1. **单元测试**：
   - 使用xUnit测试框架
   - 使用Moq进行模拟
   - 为每个领域服务编写测试

2. **集成测试**：
   - 测试组件间协作
   - 使用测试数据库
   - 模拟Revit API

3. **端到端测试**：
   - 使用实际Revit环境
   - 自动化UI测试
   - 测试完整工作流

## 10. 总结

RevitMCP项目采用严格的领域驱动设计方法，遵循SOLID原则、迪米特法则和合成复用原则，创建了一个模块化、可扩展的系统架构。通过轻量级数据提供模式，系统能够有效地将Revit BIM数据与外部系统集成，同时保持高性能和低耦合。

系统的分层架构确保了关注点分离，使不同团队能够在不同模块上并行工作。通过严格定义的接口和抽象，系统可以灵活适应未来的需求变化，同时保持代码的可测试性和可维护性。

这份设计指南为开发团队提供了清晰的架构蓝图、模块组织和接口定义，指导开发工作按照最佳实践和既定原则进行。 