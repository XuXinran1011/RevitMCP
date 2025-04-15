# DATA_MODEL

> 本文件用于说明项目的数据结构、DTO设计和序列化方案。

# RevitMCP 数据模型设计

本文档详细说明RevitMCP采用的轻量级数据提供者模式的数据模型设计、参数定义和API规范。

## 1. 设计原则

RevitMCP作为轻量级数据提供者，遵循以下设计原则：

1. **最小必要原则**：仅在Revit模型中存储最基础、最必要的数据
2. **低耦合设计**：BIM模型与业务系统通过标准化API松耦合
3. **高性能优先**：优化数据访问性能，减少模型负担
4. **语义化标识**：提供清晰的语义化数据结构，便于集成
5. **扩展性保障**：设计支持后续功能扩展的数据结构

## 2. 核心数据模型

### 2.1 元素核心标识模型

```csharp
/// <summary>
/// 元素核心标识信息，用于跨系统元素识别
/// </summary>
public class ElementCoreIdentifier
{
    /// <summary>
    /// Revit内部元素ID
    /// </summary>
    public string RevitElementId { get; set; }
    
    /// <summary>
    /// 全局唯一标识符，可用于跨模型引用
    /// </summary>
    public string GlobalUniqueId { get; set; }
    
    /// <summary>
    /// 元素类别(如墙、门、窗等)
    /// </summary>
    public string ElementCategory { get; set; }
    
    /// <summary>
    /// 元素类型
    /// </summary>
    public string ElementType { get; set; }
    
    /// <summary>
    /// 族名称(适用于族实例)
    /// </summary>
    public string FamilyName { get; set; }
    
    /// <summary>
    /// 元素名称(如有)
    /// </summary>
    public string ElementName { get; set; }
}
```

### 2.2 元素位置模型

```csharp
/// <summary>
/// 元素空间位置信息
/// </summary>
public class ElementLocationInfo
{
    /// <summary>
    /// 所在楼层
    /// </summary>
    public string Level { get; set; }
    
    /// <summary>
    /// 区域代码，用于空间组织
    /// </summary>
    public string Zone { get; set; }
    
    /// <summary>
    /// 包围盒坐标
    /// </summary>
    public BoundingBox BoundingBox { get; set; }
    
    /// <summary>
    /// 旋转角度(如适用)
    /// </summary>
    public double? Rotation { get; set; }
}

/// <summary>
/// 三维包围盒
/// </summary>
public class BoundingBox
{
    public Point3D Min { get; set; }
    public Point3D Max { get; set; }
}

/// <summary>
/// 三维点坐标
/// </summary>
public class Point3D
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
}
```

### 2.3 元素引用和状态模型

```csharp
/// <summary>
/// 元素引用和状态信息
/// </summary>
public class ElementReferenceInfo
{
    /// <summary>
    /// 外部系统ID，用于关联外部系统数据
    /// </summary>
    public string ExternalSystemId { get; set; }
    
    /// <summary>
    /// 状态代码，使用简单编码表示元素状态
    /// </summary>
    public string StatusCode { get; set; }
    
    /// <summary>
    /// 最后修改时间
    /// </summary>
    public DateTime LastModified { get; set; }
    
    /// <summary>
    /// 最后修改用户
    /// </summary>
    public string LastModifiedBy { get; set; }
}
```

### 2.4 元素关系模型

```csharp
/// <summary>
/// 元素基础关系信息
/// </summary>
public class ElementBasicRelations
{
    /// <summary>
    /// 宿主元素ID (如适用)
    /// </summary>
    public string HostElementId { get; set; }
    
    /// <summary>
    /// 直接连接的元素ID列表
    /// </summary>
    public List<string> ConnectedElementIds { get; set; }
    
    /// <summary>
    /// 容器元素ID (如适用)
    /// </summary>
    public string ContainerElementId { get; set; }
    
    /// <summary>
    /// 系统关系 (如属于同一MEP系统)
    /// </summary>
    public string SystemId { get; set; }
}
```

## 3. 共享参数定义

### 3.1 基础共享参数

RevitMCP使用以下共享参数扩展Revit元素：

| 参数名称 | 类型 | 描述 | 示例值 |
|---------|------|------|--------|
| ELEMENT_GUID | 文本 | 全局唯一ID | "8a42fe3b-e225-4f62-9c3d-e78b45cec862" |
| ELEMENT_STATUS_CODE | 文本 | 状态代码 | "NEW", "IN_PROGRESS", "COMPLETED" |
| ELEMENT_EXT_REF | 文本 | 外部系统引用ID | "PRJ-1234" |
| ELEMENT_ZONE | 文本 | 区域代码 | "A区", "2层-北区" |
| ELEMENT_LAST_MODIFIED | 日期 | 最后修改日期 | "2023-08-15T10:30:00Z" |
| ELEMENT_LAST_MODIFIED_BY | 文本 | 最后修改用户 | "user123" |

### 3.2 状态代码定义

基础状态代码推荐值：

| 状态代码 | 描述 | 应用场景 |
|---------|------|---------|
| NEW | 新建状态 | 新添加到模型的元素 |
| PLANNED | 计划状态 | 已规划但未施工的元素 |
| IN_PROGRESS | 进行中 | 正在施工的元素 |
| COMPLETED | 已完成 | 已完成施工的元素 |
| INSPECTED | 已验收 | 已完成质量验收的元素 |
| ISSUE | 存在问题 | 存在质量或其他问题的元素 |
| REVISED | 已修改 | 相对原计划已修改的元素 |

外部系统可以根据业务需求扩展状态代码，建议遵循{业务代码}_{状态}的命名方式，例如：
- QA_PASS：质量验收通过
- MAT_ORDERED：材料已订购
- MAT_DELIVERED：材料已到货

## 4. API设计

RevitMCP提供RESTful API，支持元素数据访问和操作：

### 4.1 元素信息API

```
GET /api/elements/{guid}/core           # 获取元素核心信息
GET /api/elements/{guid}/location       # 获取元素位置信息
GET /api/elements/{guid}/relations      # 获取元素关系信息
GET /api/elements/{guid}/status         # 获取元素状态信息
GET /api/elements/filtered              # 按条件筛选元素
```

#### 筛选参数示例

```
GET /api/elements/filtered?category=墙&level=一层&statusCode=IN_PROGRESS
```

### 4.2 元素更新API

```
PATCH /api/elements/{guid}/status       # 更新元素状态
{
  "statusCode": "IN_PROGRESS",
  "updatedBy": "user123"
}

PATCH /api/elements/{guid}/reference    # 更新外部引用
{
  "externalSystemId": "EXT-12345"
}

POST /api/elements/batch-update         # 批量更新元素
{
  "elementGuids": ["guid1", "guid2", "guid3"],
  "updateData": {
    "statusCode": "COMPLETED"
  }
}
```

### 4.3 映射管理API

```
POST /api/mappings                      # 创建映射关系
{
  "revitGuid": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
  "externalId": "EXT-12345",
  "mappingType": "PROGRESS"
}

GET /api/mappings/{externalId}          # 通过外部ID查询映射

POST /api/mappings/batch                # 批量创建映射
{
  "mappings": [
    {
      "revitGuid": "guid1",
      "externalId": "ext1"
    },
    {
      "revitGuid": "guid2",
      "externalId": "ext2"
    }
  ]
}

DELETE /api/mappings/{guid}             # 删除映射关系
```

### 4.4 同步API

```
POST /api/sync                          # 启动同步
{
  "syncType": "INCREMENTAL",
  "lastSyncTime": "2023-08-15T10:30:00Z"
}

GET /api/sync/status                    # 获取同步状态
```

## 5. 数据交换格式

### 5.1 元素标准响应格式

```json
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

### 5.2 批量元素响应格式

```json
{
  "totalCount": 3,
  "elements": [
    {
      "coreIdentifier": {
        "revitElementId": "123456",
        "globalUniqueId": "guid1",
        "elementCategory": "墙",
        "elementType": "基本墙"
      },
      "referenceInfo": {
        "statusCode": "IN_PROGRESS"
      }
    },
    {
      "coreIdentifier": {
        "revitElementId": "123457",
        "globalUniqueId": "guid2",
        "elementCategory": "墙",
        "elementType": "基本墙"
      },
      "referenceInfo": {
        "statusCode": "COMPLETED"
      }
    },
    {
      "coreIdentifier": {
        "revitElementId": "123458",
        "globalUniqueId": "guid3",
        "elementCategory": "门",
        "elementType": "单扇门"
      },
      "referenceInfo": {
        "statusCode": "PLANNED"
      }
    }
  ]
}
```

### 5.3 关系数据格式

```json
{
  "elementGuid": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
  "relationships": {
    "hostElement": "host-guid-xxx",
    "connectedElements": [
      "connected-guid-1",
      "connected-guid-2"
    ],
    "containerElement": "container-guid-xxx",
    "systemId": "plumbing-system-1"
  }
}
```

## 6. 扩展机制

### 6.1 自定义属性扩展

外部系统可以通过API请求RevitMCP添加自定义共享参数：

```
POST /api/schema/parameters
{
  "parameterName": "CUSTOM_PARAMETER",
  "parameterType": "TEXT",
  "description": "自定义参数",
  "categories": ["墙", "门", "窗"]
}
```

### 6.2 状态码扩展

外部系统可以注册自定义状态码：

```
POST /api/schema/status-codes
{
  "statusCode": "CUSTOM_STATUS",
  "description": "自定义状态",
  "domain": "EXTERNAL_SYSTEM_NAME"
}
```

## 7. 数据同步与集成指南

### 7.1 集成外部系统的最佳实践

1. **初始映射建立**
   - 在项目开始时，建立元素与外部系统的初始映射
   - 使用批量API提高效率
   - 保存映射关系以备后用

2. **定期数据同步**
   - 定期从RevitMCP拉取变更数据
   - 只同步变更的元素，减少网络传输
   - 使用状态码跟踪元素生命周期

3. **事件驱动更新**
   - 当外部系统数据变更时，更新Revit元素状态
   - 当Revit模型变更时，通知外部系统

### 7.2 性能优化建议

1. **批量操作**
   - 使用批量API而不是单个元素API
   - 减少API调用次数
   - 减少事务数量

2. **增量同步**
   - 使用lastModified时间戳进行增量同步
   - 只传输和处理变更数据
   - 维护同步检查点

3. **缓存策略**
   - 缓存常用元素数据
   - 实现智能预加载
   - 定义合理的缓存失效策略

## 8. 案例示例

### 8.1 进度管理集成案例

```
# 步骤1: 为所有结构柱初始化基础信息
POST /api/elements/initialize?category=结构柱

# 步骤2: 获取特定区域的所有柱子
GET /api/elements/filtered?category=结构柱&zone=A区

# 步骤3: 在外部系统中创建进度任务记录
# (在外部项目管理系统中执行)

# 步骤4: 建立映射关系
POST /api/mappings/batch
{
  "mappings": [
    {"revitGuid": "column-guid-1", "externalId": "task-1"},
    {"revitGuid": "column-guid-2", "externalId": "task-2"}
    // ...更多映射
  ]
}

# 步骤5: 随着施工进度更新元素状态
PATCH /api/elements/column-guid-1/status
{
  "statusCode": "IN_PROGRESS",
  "updatedBy": "foreman-1"
}

# 步骤6: 完成后更新状态
PATCH /api/elements/column-guid-1/status
{
  "statusCode": "COMPLETED",
  "updatedBy": "foreman-1"
}
```

### 8.2 质量验收集成案例

```
# 步骤1: 查询需要验收的元素
GET /api/elements/filtered?statusCode=COMPLETED&category=结构柱

# 步骤2: 更新验收结果
PATCH /api/elements/column-guid-1/status
{
  "statusCode": "INSPECTED",
  "updatedBy": "inspector-1"
}

# 如有问题，则标记问题状态
PATCH /api/elements/column-guid-2/status
{
  "statusCode": "ISSUE",
  "updatedBy": "inspector-1"
}
```

## 9. 结语

RevitMCP轻量级数据提供者模式通过最小化Revit模型中的数据存储，实现了高性能、低耦合的BIM数据访问方案。这种设计允许专业的项目管理平台处理复杂的业务逻辑，同时保持与BIM模型的紧密集成。

通过合理使用共享参数、状态码和API接口，各种施工管理功能（如进度管理、质量验收、物资管理等）可以在外部系统中高效实现，同时保持与BIM模型的数据一致性。 