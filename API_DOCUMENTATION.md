# RevitMCP API文档

本文档提供了RevitMCP API的全面文档，该API通过Model Context Protocol (MCP)实现与Autodesk Revit的自然语言交互。

## 目录

- [概述](#概述)
- [MCP服务器API](#mcp服务器api)
- [Revit集成API](#revit集成api)
- [工具参考](#工具参考)
- [资源参考](#资源参考)
- [错误处理](#错误处理)
- [示例](#示例)

## 概述

RevitMCP API由两个主要组件组成：

1. **MCP服务器API** - 实现Model Context Protocol规范
2. **Revit集成API** - 连接并与Revit API交互

这些组件协同工作，提供自然语言请求与Revit操作之间的无缝接口。

## MCP服务器API

MCP服务器API实现了Model Context Protocol规范，为LLM客户端提供了与Revit交互的标准化方式。

### 服务器初始化

```csharp
using ModelContextProtocol.Server;
using RevitMCP.Server;

// 创建并配置RevitMCP服务器
var server = new RevitMcpServer(new RevitMcpServerOptions
{
    Name = "RevitMCP",
    Version = "1.0.0",
    Description = "用于Revit交互的MCP服务器"
});

// 启动服务器
await server.StartAsync();
```

### 服务器配置

`RevitMcpServerOptions`类提供了RevitMCP服务器的配置选项：

| 属性 | 类型 | 描述 |
|----------|------|-------------|
| Name | string | MCP服务器名称 |
| Version | string | MCP服务器版本 |
| Description | string | MCP服务器描述 |
| RevitConnectionSettings | RevitConnectionSettings | 连接到Revit的设置 |
| LogLevel | LogLevel | 服务器的日志级别 |

## Revit集成API

Revit集成API连接并与Revit API交互，提供MCP服务器与Revit之间的桥梁。

### 连接到Revit

```csharp
using RevitMCP.Integration;

// 连接到活动的Revit实例
var revitConnection = new RevitConnection();
await revitConnection.ConnectAsync();

// 或连接到特定的Revit实例
var revitConnection = new RevitConnection(new RevitConnectionSettings
{
    ProcessId = 12345,
    Timeout = TimeSpan.FromSeconds(30)
});
await revitConnection.ConnectAsync();
```

### 执行Revit命令

```csharp
// 执行Revit命令
await revitConnection.ExecuteCommandAsync(new RevitCommand
{
    CommandId = "RevitCommandId.CreateWall",
    Parameters = new Dictionary<string, object>
    {
        { "startPoint", new XYZ(0, 0, 0) },
        { "endPoint", new XYZ(10, 0, 0) },
        { "wallType", "基本墙" }
    }
});
```

### 查询Revit元素

```csharp
// 按类别查询元素
var walls = await revitConnection.QueryElementsAsync(new ElementQuery
{
    Category = "墙",
    Filter = "宽度 > 200"
});

// 按ID查询元素
var element = await revitConnection.GetElementByIdAsync(123456);

// 获取元素属性
var properties = await revitConnection.GetElementPropertiesAsync(element);
```

## 工具参考

RevitMCP提供了一组工具，LLM客户端可以使用这些工具与Revit交互。每个工具都有特定的目的和参数模式。

### 元素查询工具

#### `queryElements`

基于指定条件查询Revit模型中的元素。

**参数：**
- `category` (string)：要查询的元素类别（例如，"墙"，"门"）
- `filter` (string, 可选)：要应用的过滤表达式
- `properties` (array, 可选)：要检索的特定属性

**返回：** 符合条件的元素数组

#### `getElementById`

通过ID检索特定元素。

**参数：**
- `elementId` (number)：要检索的元素ID

**返回：** 具有指定ID的元素

### 元素修改工具

#### `modifyParameter`

修改一个或多个元素的参数值。

**参数：**
- `elementIds` (array)：要修改的元素ID
- `parameterName` (string)：要修改的参数名称
- `value` (any)：参数的新值

**返回：** 修改操作的结果

#### `createElement`

在Revit模型中创建新元素。

**参数：**
- `category` (string)：要创建的元素类别
- `properties` (object)：新元素的属性
- `location` (object)：元素的位置信息

**返回：** 创建的元素信息

### 视图管理工具

#### `createView`

在Revit模型中创建新视图。

**参数：**
- `viewType` (string)：要创建的视图类型（例如，"FloorPlan"，"Section"）
- `name` (string)：新视图的名称
- `settings` (object, 可选)：额外的视图设置

**返回：** 创建的视图信息

#### `listViews`

列出当前Revit模型中的所有视图。

**参数：**
- `viewType` (string, 可选)：按视图类型过滤

**返回：** 符合条件的视图数组

## 资源参考

RevitMCP公开了各种资源，为LLM客户端提供有关Revit模型的信息。

### 模型信息

提供有关当前Revit模型的一般信息。

**示例：**
```json
{
  "name": "Sample_Project.rvt",
  "path": "C:\\Projects\\Sample_Project.rvt",
  "version": "2025",
  "units": "公制",
  "lastSaved": "2023-06-15T14:30:00Z",
  "author": "张三"
}
```

### 元素目录

提供当前模型中可用元素类别和类型的目录。

**示例：**
```json
{
  "categories": [
    {
      "name": "墙",
      "elementCount": 245,
      "types": ["基本墙", "外墙", "内墙"]
    },
    {
      "name": "门",
      "elementCount": 56,
      "types": ["单扇门", "双扇门", "推拉门"]
    }
  ]
}
```

## 错误处理

RevitMCP使用标准化的错误处理方法提供清晰、可操作的错误消息。

### 错误类型

| 错误代码 | 描述 | 可能原因 |
|------------|-------------|----------------|
| `REVIT_CONNECTION_ERROR` | 连接到Revit时出错 | Revit未运行，权限不足 |
| `ELEMENT_NOT_FOUND` | 未找到元素 | 无效的元素ID，元素已删除 |
| `PARAMETER_NOT_FOUND` | 未找到参数 | 无效的参数名称，参数不适用于元素 |
| `INVALID_OPERATION` | 无效操作 | 元素类型不支持操作 |
| `TRANSACTION_FAILED` | 事务失败 | 违反Revit约束，模型已锁定 |

### 错误响应格式

```json
{
  "error": {
    "code": "ELEMENT_NOT_FOUND",
    "message": "在当前模型中未找到ID为123456的元素",
    "details": {
      "elementId": 123456,
      "modelName": "Sample_Project.rvt"
    }
  }
}
```

## 示例

### 示例1：查询墙元素

```csharp
// MCP工具实现
[McpServerTool]
public async Task<IEnumerable<ElementInfo>> QueryWalls(string filterExpression = null)
{
    var query = new ElementQuery
    {
        Category = "墙",
        Filter = filterExpression
    };
    
    var elements = await _revitConnection.QueryElementsAsync(query);
    return elements.Select(e => new ElementInfo
    {
        Id = e.Id,
        Name = e.Name,
        Category = e.Category,
        Properties = e.GetProperties()
    });
}
```

### 示例2：创建新视图

```csharp
// MCP工具实现
[McpServerTool]
public async Task<ViewInfo> CreateFloorPlanView(string viewName, string level)
{
    var viewSettings = new ViewSettings
    {
        Type = "FloorPlan",
        Name = viewName,
        AssociatedLevel = level
    };
    
    var view = await _revitConnection.CreateViewAsync(viewSettings);
    return new ViewInfo
    {
        Id = view.Id,
        Name = view.Name,
        Type = view.ViewType,
        Scale = view.Scale
    };
}
```

### 示例3：修改元素参数

```csharp
// MCP工具实现
[McpServerTool]
public async Task<ModificationResult> ChangeWallType(int elementId, string newWallType)
{
    var element = await _revitConnection.GetElementByIdAsync(elementId);
    if (element == null)
    {
        throw new ElementNotFoundException(elementId);
    }
    
    var result = await _revitConnection.ModifyParameterAsync(element, "Type", newWallType);
    return new ModificationResult
    {
        Success = result.Success,
        Message = result.Message,
        ElementId = elementId,
        Parameter = "Type",
        NewValue = newWallType
    };
}
```
