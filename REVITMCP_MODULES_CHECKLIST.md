# RevitMCP 待开发代码模块清单

> 本清单用于追踪项目所有模块的开发状态、优先级和依赖关系。每次开发、测试或完成一个模块后，请及时更新状态。建议开发者优先完成高优先级、依赖少的模块。详细开发顺序和依赖见下表。

---

本文档提供了RevitMCP项目的待开发模块清单，按照领域驱动设计(DDD)分层架构组织，并明确标识了各模块间的依赖关系和技术细节。RevitMCP项目通过跨进程通信解决了Revit 2025(.NET 8)与MCP SDK(.NET 9)的版本兼容性问题。

## 状态标记说明
- ⬜ 未开始：模块尚未开始开发
- 🏗️ 开发中：模块正在开发中
- ✅ 开发完成：模块代码开发已完成，等待测试
- 🧪 测试中：模块处于测试阶段
- ✓ 已完成：模块已完成开发和测试，可以部署

## RevitMCP.Plugin (Revit插件端 - .NET 8)

### Presentation - 表示层

| 文件路径 | 描述 | 优先级 | 依赖项 | 技术细节 | 状态 |
|---------|------|--------|-------|----------|------|
| `Presentation/UI/RevitMCPRibbon.cs` | Revit功能区UI | 高 | Application层命令 | Revit.UI命名空间、RibbonPanel、PushButton | ⬜ 未开始 |
| `Presentation/UI/RevitMCPDockablePanel.cs` | Revit可停靠面板 | 中 | UIUpdateService | Revit.UI.Docking、IDockablePaneProvider | ⬜ 未开始 |
| `Presentation/UI/SettingsDialog.cs` | 设置对话框 | 中 | SettingsService | WPF、INotifyPropertyChanged | ⬜ 未开始 |
| `Presentation/UI/ElementHighlighter.cs` | 元素高亮显示工具 | 中 | RevitAPIAdapter、UIUpdateService | Revit.DB.OverrideGraphicSettings | ⬜ 未开始 |
| `Presentation/UI/ProgressDialog.cs` | 进度显示对话框 | 低 | UIUpdateService | WPF、IProgress<T>接口 | ⬜ 未开始 |

### Application - 应用层

| 文件路径 | 描述 | 优先级 | 依赖项 | 技术细节 | 状态 |
|---------|------|--------|-------|----------|------|
| `Application/Commands/StartMCPCommand.cs` | 启动MCP服务器命令 | 高 | MCPServerManager | IExternalCommand接口、异步启动 | ⬜ 未开始 |
| `Application/Commands/StopMCPCommand.cs` | 停止MCP服务器命令 | 高 | MCPServerManager | IExternalCommand接口、优雅关闭 | ⬜ 未开始 |
| `Application/Commands/ConnectRevitCommand.cs` | 连接Revit命令 | 高 | ProcessCommunication | IExternalCommand接口、连接握手 | ⬜ 未开始 |
| `Application/Commands/DisconnectRevitCommand.cs` | 断开Revit连接命令 | 高 | ProcessCommunication | IExternalCommand接口、资源释放 | ⬜ 未开始 |
| `Application/Services/ServerManagerService.cs` | 服务器管理服务 | 高 | MCPServerManager、RevitLogger | 服务生命周期管理、状态检查 | ⬜ 未开始 |
| `Application/Services/SettingsService.cs` | 设置管理服务 | 中 | PluginDataStorage | JSON序列化、配置验证 | ⬜ 未开始 |
| `Application/Services/RevitElementService.cs` | Revit元素服务 | 高 | RevitAPIAdapter、RevitElementService | 选择集管理、过滤器应用 | ⬜ 未开始 |
| `Application/Services/RevitParameterService.cs` | Revit参数服务 | 高 | RevitParameter | 参数验证、单位转换 | ⬜ 未开始 |
| `Application/Services/UIUpdateService.cs` | UI更新服务 | 中 | RevitEventDispatcher | ExternalEvent、UI线程同步 | ⬜ 未开始 |
| `Application/Commands/GetElementCommand.cs` | 获取元素命令 | 高 | ElementService、IElementRepository | CQRS命令模式、验证机制、异步操作 | ⬜ 未开始 |
| `Application/Commands/ModifyElementCommand.cs` | 修改元素命令 | 高 | ElementService、IElementRepository | CQRS命令模式、事务管理、参数验证 | ⬜ 未开始 |
| `Application/Commands/CreateElementCommand.cs` | 创建元素命令 | 中 | ElementService、IElementRepository | CQRS命令模式、工厂模式、数据验证 | ⬜ 未开始 |
| `Application/Commands/DeleteElementCommand.cs` | 删除元素命令 | 中 | ElementService、IElementRepository | CQRS命令模式、级联删除、依赖检查 | ⬜ 未开始 |
| `Application/Queries/GetElementsByCategoryQuery.cs` | 按类别获取元素查询 | 高 | QueryService、IElementRepository | CQRS查询模式、过滤器模式、排序支持 | ⬜ 未开始 |
| `Application/Queries/GetElementsByParameterQuery.cs` | 按参数获取元素查询 | 高 | QueryService、IElementRepository | CQRS查询模式、条件构建、动态参数 | ⬜ 未开始 |
| `Application/Queries/GetElementsByLocationQuery.cs` | 按位置获取元素查询 | 中 | QueryService、GeometryService | CQRS查询模式、空间索引、碰撞检测 | ⬜ 未开始 |
| `Application/Services/MCPToolService.cs` | MCP工具服务 | 高 | 命令和查询处理器 | 命令路由、参数映射、结果格式化 | ⬜ 未开始 |
| `Application/Services/NLPInterpretationService.cs` | 自然语言解释服务 | 高 | IntentRecognizer、ParameterExtractor | NLP处理、意图识别、上下文管理 | ⬜ 未开始 |
| `Application/Services/ResponseFormattingService.cs` | 响应格式化服务 | 高 | 无 | 模板渲染、多语言支持、结构化输出 | ⬜ 未开始 |
| `Application/Services/QueryValidationService.cs` | 查询验证服务 | 中 | 无 | 验证规则、错误消息、链式验证 | ⬜ 未开始 |
| `Application/DTOs/ElementDTO.cs` | 元素数据传输对象 | 高 | 无 | 序列化注解、版本控制、字段映射 | ⬜ 未开始 |
| `Application/DTOs/ParameterDTO.cs` | 参数数据传输对象 | 高 | 无 | 序列化注解、类型安全、单位转换 | ⬜ 未开始 |
| `Application/Mappers/ElementMapper.cs` | 元素映射器 | 高 | Element、ElementDTO | AutoMapper、自定义转换器、深度映射 | ⬜ 未开始 |
| `Application/Mappers/ParameterMapper.cs` | 参数映射器 | 高 | Parameter、ParameterDTO | AutoMapper、单位换算、类型转换 | ⬜ 未开始 |
| `Application/Commands/QuantityTakeoff/CalculateQuantitiesCommand.cs` | 计算工程量命令 | 低 | IQuantityTakeoffService | CQRS命令模式、计算选项、结果处理 | ⬜ 未开始 |
| `Application/Commands/QuantityTakeoff/ExportQuantitiesCommand.cs` | 导出工程量命令 | 低 | IQuantityExporter | CQRS命令模式、导出选项、文件处理 | ⬜ 未开始 |
| `Application/Queries/QuantityTakeoff/GetQuantityCalculationStatusQuery.cs` | 获取计算状态查询 | 低 | IQuantityTakeoffService | CQRS查询模式、状态跟踪、进度报告 | ⬜ 未开始 |
| `Application/Services/QuantityTakeoff/QuantityTakeoffApplicationService.cs` | 工程量应用服务 | 低 | IQuantityTakeoffService | 服务协调、命令处理、结果转换 | ⬜ 未开始 |
| `Application/DTOs/QuantityTakeoff/QuantityResultDTO.cs` | 工程量结果DTO | 低 | 无 | 数据传输对象、序列化支持、树形结构 | ⬜ 未开始 |
| `Application/DTOs/QuantityTakeoff/CalculationOptionsDTO.cs` | 计算选项DTO | 低 | 无 | 数据传输对象、选项配置、默认值 | ⬜ 未开始 |
| `Application/Mappers/QuantityTakeoff/QuantityResultMapper.cs` | 工程量结果映射器 | 低 | QuantityResult、QuantityResultDTO | 数据映射、单位转换、格式化规则 | ⬜ 未开始 |
| `Application/Mappers/QuantityTakeoff/CalculationOptionsMapper.cs` | 计算选项映射器 | 低 | CalculationOptions、CalculationOptionsDTO | 选项映射、默认值处理、验证规则 | ⬜ 未开始 |

### Domain - 领域层

| 文件路径 | 描述 | 优先级 | 依赖项 | 技术细节 | 状态 |
|---------|------|--------|-------|----------|------|
| `Domain/Models/Element.cs` | 元素领域模型 | 高 | 无 | 实体基类、ID生成、不可变属性 | ⬜ 未开始 |
| `Domain/Models/Wall.cs` | 墙体领域模型 | 高 | Element | 继承Element、墙体特有属性和行为 | ⬜ 未开始 |
| `Domain/Models/Floor.cs` | 楼板领域模型 | 高 | Element | 继承Element、楼板特有属性和行为 | ⬜ 未开始 |
| `Domain/Models/Room.cs` | 房间领域模型 | 高 | Element | 继承Element、房间特有属性和边界计算 | ⬜ 未开始 |
| `Domain/Models/Parameter.cs` | 参数领域模型 | 高 | 无 | 值对象模式、类型安全、单位支持 | ⬜ 未开始 |
| `Domain/Models/Category.cs` | 类别领域模型 | 高 | 无 | 枚举封装、类别层次结构、过滤功能 | ⬜ 未开始 |
| `Domain/Models/Location.cs` | 位置值对象 | 中 | 无 | 值对象模式、不可变性、坐标操作 | ⬜ 未开始 |
| `Domain/Models/Dimension.cs` | 尺寸值对象 | 中 | 无 | 值对象模式、单位转换、精度控制 | ⬜ 未开始 |
| `Domain/Models/ModelQuery.cs` | 查询模型 | 高 | 无 | 值对象模式、查询参数封装、不可变性 | ⬜ 未开始 |
| `Domain/Models/QueryType.cs` | 查询类型枚举 | 高 | 无 | 枚举定义、查询分类、扩展方法 | ⬜ 未开始 |
| `Domain/Models/ChangeScope.cs` | 变更范围模型 | 中 | Element | 值对象模式、元素集合、空间边界 | ⬜ 未开始 |
| `Domain/Models/SeparationOptions.cs` | 模型分离选项 | 中 | 无 | Builder模式、配置参数、验证规则 | ⬜ 未开始 |
| `Domain/Models/SeparationResult.cs` | 分离结果模型 | 中 | 无 | 结果封装、统计信息、日志记录 | ⬜ 未开始 |
| `Domain/Models/FamilyMetadata.cs` | 族元数据模型 | 中 | Parameter | 值对象模式、元数据管理、标签支持 | ⬜ 未开始 |
| `Domain/Models/QuantityTakeoff/QuantityResult.cs` | 工程量计算结果 | 低 | 无 | 值对象模式、工程量数据封装、单位支持 | ⬜ 未开始 |
| `Domain/Models/QuantityTakeoff/CalculationOptions.cs` | 计算选项 | 低 | 无 | 选项参数、计算规则配置、格式选择 | ⬜ 未开始 |
| `Domain/Models/QuantityTakeoff/CalculationRule.cs` | 计算规则 | 低 | 无 | 规则引擎、计算方法标识、规则优先级 | ⬜ 未开始 |
| `Domain/Models/QuantityTakeoff/ExportFormat.cs` | 导出格式 | 低 | 无 | 枚举定义、格式参数、扩展支持 | ⬜ 未开始 |
| `Domain/Services/ElementService.cs` | 元素服务 | 高 | Element、IElementRepository | 领域服务模式、业务规则、事务协调 | ⬜ 未开始 |
| `Domain/Services/QueryService.cs` | 查询服务 | 高 | Element、IElementRepository | 查询构建、规范模式、过滤器链 | ⬜ 未开始 |
| `Domain/Services/GeometryService.cs` | 几何服务 | 中 | Element、Location | 几何算法、空间计算、碰撞检测 | ⬜ 未开始 |
| `Domain/Services/FamilySearchService.cs` | 族库搜索服务 | 中 | FamilyMetadata、IFamilyRepository | 搜索算法、相关性排序、过滤条件 | ⬜ 未开始 |
| `Domain/Services/ModelModificationService.cs` | 模型修改服务 | 高 | Element、IElementRepository | 修改策略、业务规则校验、事件发布 | ⬜ 未开始 |
| `Domain/Services/ChangeModelSeparationService.cs` | 变更模型分离服务 | 中 | ChangeScope、IElementDependencyAnalyzer | 模型分离算法、依赖分析、进度报告 | ⬜ 未开始 |
| `Domain/Services/QuantityTakeoff/IQuantityTakeoffService.cs` | 工程量计算服务接口 | 低 | Element、CalculationOptions | 工程量计算、结果生成、数据导出 | ⬜ 未开始 |
| `Domain/Services/QuantityTakeoff/ParameterCalculator.cs` | 参数计算器 | 低 | Element、Parameter | 参数提取、公式应用、单位换算 | ⬜ 未开始 |
| `Domain/Services/QuantityTakeoff/GeometryCalculator.cs` | 几何计算器 | 低 | Element、GeometryService | 几何分析、体积面积计算、尺寸提取 | ⬜ 未开始 |
| `Domain/Services/QuantityTakeoff/SolidOperationCalculator.cs` | 实体运算计算器 | 低 | Element、GeometryService | 布尔运算、复杂几何处理、开洞计算 | ⬜ 未开始 |
| `Domain/Services/QuantityTakeoff/MeshAnalysisCalculator.cs` | 网格分析计算器 | 低 | Element、GeometryService | 网格转换、区域计算、密度控制 | ⬜ 未开始 |
| `Domain/Services/QuantityTakeoff/HybridCalculator.cs` | 混合计算器 | 低 | 各种计算器服务 | 策略模式、智能选择、优化调度 | ⬜ 未开始 |
| `Domain/Services/QuantityTakeoff/CalculationStrategyService.cs` | 计算策略服务 | 低 | 各种计算器 | 策略选择、计算路径决策、性能优化 | ⬜ 未开始 |
| `Domain/Interfaces/IElementRepository.cs` | 元素仓储接口 | 高 | Element | 仓储模式、查询规范、CRUD操作 | ⬜ 未开始 |
| `Domain/Interfaces/IParameterRepository.cs` | 参数仓储接口 | 高 | Parameter | 仓储模式、参数查询、批量操作 | ⬜ 未开始 |
| `Domain/Interfaces/IFamilyRepository.cs` | 族库仓储接口 | 中 | FamilyMetadata | 仓储模式、族库管理、元数据操作 | ⬜ 未开始 |
| `Domain/Interfaces/IElementDependencyAnalyzer.cs` | 元素依赖分析接口 | 中 | Element | 依赖分析、关系深度控制、递归查找 | ⬜ 未开始 |
| `Domain/Interfaces/QuantityTakeoff/IQuantityExporter.cs` | 工程量导出接口 | 低 | QuantityResult | 导出格式、模板应用、批量处理 | ⬜ 未开始 |
| `Domain/Interfaces/QuantityTakeoff/ICalculationRuleProvider.cs` | 计算规则提供者接口 | 低 | CalculationRule | 规则加载、验证、优先级管理 | ⬜ 未开始 |
| `Domain/Interfaces/QuantityTakeoff/IQuantityCalculator.cs` | 工程量计算器接口 | 低 | Element | 计算方法抽象、结果生成、错误处理 | ⬜ 未开始 |
| `Domain/Events/ElementCreatedEvent.cs` | 元素创建事件 | 中 | Element | 领域事件模式、事件数据、创建上下文 | ⬜ 未开始 |
| `Domain/Events/ElementModifiedEvent.cs` | 元素修改事件 | 中 | Element | 领域事件模式、变更跟踪、差异数据 | ⬜ 未开始 |
| `Domain/Events/ModelSeparatedEvent.cs` | 模型分离事件 | 中 | SeparationResult | 领域事件模式、分离结果、操作时间戳 | ⬜ 未开始 |
| `Domain/Events/QuantityTakeoff/QuantityCalculationCompletedEvent.cs` | 工程量计算完成事件 | 低 | QuantityResult | 领域事件模式、计算结果、处理时间 | ⬜ 未开始 |

### Infrastructure - 基础设施层

| 文件路径 | 描述 | 优先级 | 依赖项 | 技术细节 | 状态 |
|---------|------|--------|-------|----------|------|
| `Infrastructure/RevitAPI/RevitAPIAdapter.cs` | Revit API适配器 | 高 | 领域模型、服务 | 适配器模式、Revit API调用、.NET 8兼容 | ⬜ 未开始 |
| `Infrastructure/RevitAPI/RevitDocumentManager.cs` | Revit文档管理器 | 高 | RevitAPIAdapter | 文档访问封装、线程安全、事件处理 | ⬜ 未开始 |
| `Infrastructure/RevitAPI/RevitTransactionManager.cs` | Revit事务管理器 | 高 | RevitAPIAdapter | 事务管理模式、回滚机制、异常处理 | ⬜ 未开始 |
| `Infrastructure/Communication/MCPServerManager.cs` | MCP服务器管理器 | 高 | ProcessCommunication | 进程管理、.NET 9启动、生命周期管理 | ⬜ 未开始 |
| `Infrastructure/Communication/ProcessCommunication.cs` | 进程间通信 | 高 | RevitAPIAdapter | Stdio通信、跨.NET版本消息传递、序列化 | ✓ 已完成 |
| `Infrastructure/Communication/RevitEventDispatcher.cs` | Revit事件调度器 | 中 | RevitAPIAdapter | ExternalEvent、异步调用、UI线程同步 | ⬜ 未开始 |
| `Infrastructure/Logging/RevitLogger.cs` | Revit日志记录器 | 中 | 无 | Serilog集成、文件日志、结构化日志 | ⬜ 未开始 |
| `Infrastructure/Configuration/RevitPluginConfig.cs` | Revit插件配置 | 中 | 无 | 配置绑定、默认值、验证规则 | ⬜ 未开始 |
| `Infrastructure/Storage/PluginDataStorage.cs` | 插件数据存储 | 低 | 无 | 本地文件存储、加密选项、版本控制 | ⬜ 未开始 |
| `Infrastructure/QuantityTakeoff/QuantityTakeoffService.cs` | 工程量计算服务实现 | 低 | IQuantityTakeoffService | 实现领域接口、计算逻辑、异步处理 | ⬜ 未开始 |
| `Infrastructure/QuantityTakeoff/ExcelExporter.cs` | Excel导出器 | 低 | IQuantityExporter | Excel文件生成、模板应用、公式支持 | ⬜ 未开始 |
| `Infrastructure/QuantityTakeoff/CsvExporter.cs` | CSV导出器 | 低 | IQuantityExporter | CSV文件生成、字段格式化、编码处理 | ⬜ 未开始 |
| `Infrastructure/QuantityTakeoff/DefaultCalculationRuleProvider.cs` | 默认计算规则提供者 | 低 | ICalculationRuleProvider | 规则加载、配置读取、规则缓存 | ⬜ 未开始 |
| `Infrastructure/QuantityTakeoff/QuantityCalculationCache.cs` | 工程量计算缓存 | 低 | 无 | 缓存机制、失效策略、内存优化 | ⬜ 未开始 |

## RevitMCP.Server (MCP服务器端 - .NET 9)

### Presentation - 表示层

| 文件路径 | 描述 | 优先级 | 依赖项 | 技术细节 | 状态 |
|---------|------|--------|-------|----------|------|
| `Presentation/MCP/MCPServer.cs` | MCP服务器类 | 高 | MCPToolHandler | MCP SDK、工具注册 | ⬜ 未开始 |
| `Presentation/MCP/Tools/ElementQueryTool.cs` | 元素查询工具 | 高 | MCPToolService、ElementQueryCommand | MCP Tool规范、异步处理 | ⬜ 未开始 |
| `Presentation/MCP/Tools/ElementModificationTool.cs` | 元素修改工具 | 高 | MCPToolService、ElementModificationCommand | MCP Tool规范、事务处理 | ⬜ 未开始 |
| `Presentation/MCP/Tools/ParameterQueryTool.cs` | 参数查询工具 | 高 | MCPToolService、ParameterQueryCommand | MCP Tool规范、参数筛选 | ⬜ 未开始 |
| `Presentation/MCP/Tools/GeometryAnalysisTool.cs` | 几何分析工具 | 中 | MCPToolService、GeometryService | MCP Tool规范、几何计算 | ⬜ 未开始 |
| `Presentation/MCP/Tools/ViewManagementTool.cs` | 视图管理工具 | 中 | MCPToolService、ViewService | MCP Tool规范、视图创建 | ⬜ 未开始 |
| `Presentation/MCP/Tools/DocumentInfoTool.cs` | 文档信息工具 | 中 | MCPToolService、ProjectService | MCP Tool规范、项目信息 | ⬜ 未开始 |
| `Presentation/MCP/Tools/ElementCreationTool.cs` | 元素创建工具 | 中 | MCPToolService、ElementCreationCommand | MCP Tool规范、参数验证 | ⬜ 未开始 |
| `Presentation/MCP/Tools/FamilyManagementTool.cs` | 族管理工具 | 低 | MCPToolService、FamilyService | MCP Tool规范、族加载 | ⬜ 未开始 |
| `Presentation/MCP/Tools/QuantityTakeoff/QuantityCalculationTool.cs` | 工程量计算工具 | 低 | MCPToolService、QuantityTakeoffService | MCP Tool规范、计算配置 | ⬜ 未开始 |
| `Presentation/MCP/Tools/QuantityTakeoff/QuantityExportTool.cs` | 工程量导出工具 | 低 | MCPToolService、QuantityExportService | MCP Tool规范、导出选项 | ⬜ 未开始 |
| `Presentation/Web/WebServer.cs` | Web服务器（可选） | 低 | 应用层服务 | ASP.NET Core、API端点 | ⬜ 未开始 |
| `Presentation/Web/Controllers/ApiController.cs` | API控制器（可选） | 低 | 应用层服务 | REST API、JWT认证 | ⬜ 未开始 |

### Infrastructure - 基础设施层

| 文件路径 | 描述 | 优先级 | 依赖项 | 技术细节 | 状态 |
|---------|------|--------|-------|----------|------|
| `Infrastructure/MCP/MCPToolHandler.cs` | MCP工具处理器 | 高 | MCPToolService | MCP SDK集成、工具分发 | ⬜ 未开始 |
| `Infrastructure/MCP/MCPResponseFormatter.cs` | MCP响应格式化器 | 高 | ResponseFormattingService | MCP响应格式、JSON | ⬜ 未开始 |
| `Infrastructure/MCP/MCPContextManager.cs` | MCP上下文管理器 | 高 | ContextTracker | 会话管理、状态保持 | ⬜ 未开始 |
| `Infrastructure/NLP/IntentRecognizer.cs` | 意图识别器 | 高 | 无 | 规则引擎、正则表达式 | ⬜ 未开始 |
| `Infrastructure/NLP/ParameterExtractor.cs` | 参数提取器 | 高 | 无 | 文本解析、命名实体 | ⬜ 未开始 |
| `Infrastructure/NLP/ContextTracker.cs` | 上下文跟踪器 | 中 | 无 | 状态机、会话数据 | ⬜ 未开始 |
| `Infrastructure/Repositories/RevitElementRepository.cs` | Revit元素仓储实现 | 高 | IElementRepository、RevitPluginCommunicator | 仓储实现、查询翻译 | ⬜ 未开始 |
| `Infrastructure/Repositories/RevitParameterRepository.cs` | Revit参数仓储实现 | 高 | IParameterRepository、RevitPluginCommunicator | 仓储实现、缓存 | ⬜ 未开始 |
| `Infrastructure/Communication/RevitPluginCommunicator.cs` | Revit插件通信器 | 高 | 无 | IPC通信、序列化 | ⬜ 未开始 |
| `Infrastructure/Communication/MCPClientManager.cs` | MCP客户端管理器 | 高 | 无 | MCP SDK客户端、连接池 | ⬜ 未开始 |
| `Infrastructure/Caching/ElementCache.cs` | 元素缓存 | 中 | 无 | 缓存策略、失效控制 | ⬜ 未开始 |
| `Infrastructure/Caching/GeometryCache.cs` | 几何缓存 | 中 | 无 | 几何计算缓存、哈希 | ⬜ 未开始 |
| `Infrastructure/Logging/ServerLogger.cs` | 服务器日志记录器 | 中 | 无 | Serilog集成、结构化日志 | ⬜ 未开始 |
| `Infrastructure/Configuration/ServerConfig.cs` | 服务器配置 | 中 | 无 | 选项模式、配置验证 | ⬜ 未开始 |
| `Infrastructure/QuantityTakeoff/ServerQuantityTakeoffService.cs` | 服务端工程量计算服务 | 低 | IQuantityTakeoffService、RevitPluginCommunicator | 服务实现、跨进程通信 | ⬜ 未开始 |
| `Infrastructure/QuantityTakeoff/QuantityCalculationJobManager.cs` | 工程量计算作业管理器 | 低 | 无 | 队列管理、后台任务、进度追踪 | ⬜ 未开始 |
| `Infrastructure/QuantityTakeoff/QuantityResultSerializer.cs` | 工程量结果序列化器 | 低 | 无 | 序列化优化、大数据处理、压缩支持 | ⬜ 未开始 |
| `Infrastructure/QuantityTakeoff/ExportTemplateManager.cs` | 导出模板管理器 | 低 | 无 | 模板加载、自定义模板支持、资源管理 | ⬜ 未开始 |

## RevitMCP.Shared (共享库 - netstandard2.0)

### Models - 共享模型

| 文件路径 | 描述 | 优先级 | 依赖项 | 技术细节 | 状态 |
|---------|------|--------|-------|----------|------|
| `Models/RevitElementInfo.cs` | Revit元素信息模型 | 高 | 无 | 轻量级DTO、序列化友好 | ⬜ 未开始 |
| `Models/RevitParameterInfo.cs` | Revit参数信息模型 | 高 | 无 | 值类型包装、不可变性 | ⬜ 未开始 |
| `Models/RevitCategoryInfo.cs` | Revit类别信息模型 | 高 | 无 | 枚举映射、类别层次 | ⬜ 未开始 |
| `Models/RevitProjectInfo.cs` | Revit项目信息模型 | 中 | 无 | 轻量级项目元数据 | ⬜ 未开始 |
| `Models/GeometryInfo.cs` | 几何信息模型 | 中 | 无 | 坐标系统、几何数据 | ⬜ 未开始 |
| `Models/ResponseMessage.cs` | 通用响应消息模型 | 高 | 无 | 跨进程通信响应格式 | ⬜ 未开始 |
| `Models/QueryMessage.cs` | 通用查询消息模型 | 高 | 无 | 跨进程通信查询格式 | ⬜ 未开始 |
| `Models/QuantityTakeoff/QuantityInfo.cs` | 工程量信息模型 | 低 | 无 | 轻量级工程量数据、序列化优化 | ⬜ 未开始 |
| `Models/QuantityTakeoff/CalculationMethodInfo.cs` | 计算方法信息 | 低 | 无 | 计算方法元数据、枚举映射 | ⬜ 未开始 |
| `Models/QuantityTakeoff/QuantityUnit.cs` | 工程量单位模型 | 低 | 无 | 单位定义、转换因子、格式化规则 | ⬜ 未开始 |

### Interfaces - 共享接口

| 文件路径 | 描述 | 优先级 | 依赖项 | 技术细节 | 状态 |
|---------|------|--------|-------|----------|------|
| `Interfaces/IRevitElement.cs` | Revit元素接口 | 高 | 无 | 跨项目契约、抽象能力 | ⬜ 未开始 |
| `Interfaces/IRevitParameter.cs` | Revit参数接口 | 高 | 无 | 参数操作抽象、类型安全 | ⬜ 未开始 |
| `Interfaces/IRevitCategory.cs` | Revit类别接口 | 高 | 无 | 类别层次结构抽象 | ⬜ 未开始 |
| `Interfaces/IGeometryProvider.cs` | 几何提供者接口 | 中 | 无 | 几何数据访问抽象 | ⬜ 未开始 |
| `Interfaces/Server/IMCPServerCommunication.cs` | MCP服务器通信接口 | 高 | 无 | 跨进程通信抽象 | ⬜ 未开始 |
| `Interfaces/Server/IElementRepository.cs` | 元素仓储抽象接口 | 高 | IRevitElement | 领域仓储接口、隔离实现 | ⬜ 未开始 |
| `Interfaces/Server/IParameterRepository.cs` | 参数仓储抽象接口 | 高 | IRevitParameter | 领域仓储接口、隔离实现 | ⬜ 未开始 |
| `Interfaces/Plugin/IRevitAPIAdapter.cs` | Revit API适配器接口 | 高 | 无 | Revit API封装抽象 | ⬜ 未开始 |
| `Interfaces/QuantityTakeoff/ISharedQuantityCalculator.cs` | 共享工程量计算器接口 | 低 | QuantityInfo | 跨项目计算抽象、版本兼容 | ⬜ 未开始 |
| `Interfaces/QuantityTakeoff/IQuantityExportProvider.cs` | 工程量导出提供者接口 | 低 | QuantityInfo | 导出抽象、格式支持、模板接口 | ⬜ 未开始 |

### Communication - 通信相关

| 文件路径 | 描述 | 优先级 | 依赖项 | 技术细节 | 状态 |
|---------|------|--------|-------|----------|------|
| `Communication/MCPMessage.cs` | MCP消息类 | 高 | 无 | 消息结构、序列化注解 | ⬜ 未开始 |
| `Communication/RevitCommandMessage.cs` | Revit命令消息 | 高 | 无 | 命令模式、序列化 | ⬜ 未开始 |
| `Communication/RevitQueryMessage.cs` | Revit查询消息 | 高 | 无 | 查询模式、序列化 | ⬜ 未开始 |
| `Communication/MessageEnvelope.cs` | 消息信封 | 中 | 无 | 包装消息、元数据 | ⬜ 未开始 |
| `Communication/MessageSerializer.cs` | 消息序列化器 | 中 | 无 | 高性能序列化、版本兼容 | ⬜ 未开始 |
| `Communication/IPCProtocol.cs` | 进程间通信协议 | 高 | 无 | 协议定义、消息格式 | ⬜ 未开始 |
| `Communication/QuantityTakeoff/QuantityCalculationMessage.cs` | 工程量计算消息 | 低 | 无 | 跨进程计算请求、参数编码 | ⬜ 未开始 |
| `Communication/QuantityTakeoff/QuantityExportMessage.cs` | 工程量导出消息 | 低 | 无 | 导出请求格式、选项序列化 | ⬜ 未开始 |

### DTOs - 数据传输对象

| 文件路径 | 描述 | 优先级 | 依赖项 | 技术细节 | 状态 |
|---------|------|--------|-------|----------|------|
| `DTOs/ElementDTO.cs` | 元素数据传输对象 | 高 | 无 | 序列化优化、版本兼容 | ⬜ 未开始 |
| `DTOs/ParameterDTO.cs` | 参数数据传输对象 | 高 | 无 | 多类型参数表示、类型安全 | ⬜ 未开始 |
| `DTOs/CategoryDTO.cs` | 类别数据传输对象 | 高 | 无 | 层次结构表示、简化映射 | ⬜ 未开始 |
| `DTOs/ProjectDTO.cs` | 项目数据传输对象 | 中 | 无 | 项目元数据、简化表示 | ⬜ 未开始 |
| `DTOs/GeometryDTO.cs` | 几何数据传输对象 | 中 | 无 | 轻量级几何表示、坐标转换 | ⬜ 未开始 |
| `DTOs/QuantityTakeoff/QuantityResultDTO.cs` | 工程量结果DTO | 低 | 无 | 标准化结果格式、跨项目兼容 | ⬜ 未开始 |
| `DTOs/QuantityTakeoff/CalculationOptionsDTO.cs` | 计算选项DTO | 低 | 无 | 选项参数封装、默认值处理 | ⬜ 未开始 |
| `DTOs/QuantityTakeoff/QuantitySummaryDTO.cs` | 工程量汇总DTO | 低 | 无 | 汇总数据表示、类别分组支持 | ⬜ 未开始 |

### Utilities - 工具类

| 文件路径 | 描述 | 优先级 | 依赖项 | 技术细节 | 状态 |
|---------|------|--------|-------|----------|------|
| `Utilities/UnitConverter.cs` | 单位转换工具 | 高 | 无 | 单位系统转换、精度控制 | ⬜ 未开始 |
| `Utilities/ParameterTypeConverter.cs` | 参数类型转换器 | 高 | 无 | 类型安全转换、错误处理 | ⬜ 未开始 |
| `Utilities/RevitIdUtility.cs` | Revit ID工具 | 中 | 无 | ID处理、格式化 | ⬜ 未开始 |
| `Utilities/ValidationHelper.cs` | 验证助手 | 中 | 无 | 参数验证、契约断言 | ⬜ 未开始 |
| `Utilities/GeometryMath.cs` | 几何数学工具 | 中 | 无 | 几何计算、精度控制 | ⬜ 未开始 |
| `Utilities/SerializationHelper.cs` | 序列化辅助工具 | 高 | 无 | JSON/二进制序列化、版本控制 | ⬜ 未开始 |
| `Utilities/QuantityTakeoff/QuantityMathUtility.cs` | 工程量数学工具 | 低 | 无 | 面积体积计算、单位转换 | ⬜ 未开始 |
| `Utilities/QuantityTakeoff/FormulaEvaluator.cs` | 公式计算器 | 低 | 无 | 工程量公式解析、计算引擎 | ⬜ 未开始 |

## 通用测试项目 - RevitMCP.Tests

### Shared层测试

| 文件路径 | 描述 | 优先级 | 依赖项 | 技术细节 | 状态 |
|---------|------|--------|-------|----------|------|
| `Shared/Models/RevitElementInfoTests.cs` | 元素信息模型测试 | 高 | RevitElementInfo | xUnit、对象属性验证 | ⬜ 未开始 |
| `Shared/Models/ResponseMessageTests.cs` | 响应消息模型测试 | 高 | ResponseMessage | xUnit、序列化测试 | ⬜ 未开始 |
| `Shared/Communication/MCPMessageTests.cs` | MCP消息测试 | 高 | MCPMessage | xUnit、消息结构测试 | ⬜ 未开始 |
| `Shared/Utilities/SerializationHelperTests.cs` | 序列化工具测试 | 高 | SerializationHelper | xUnit、序列化兼容性测试 | ⬜ 未开始 |
| `Shared/QuantityTakeoff/QuantityInfoTests.cs` | 工程量信息模型测试 | 低 | QuantityInfo | xUnit、数据完整性测试 | ⬜ 未开始 |
| `Shared/QuantityTakeoff/QuantityUnitTests.cs` | 工程量单位测试 | 低 | QuantityUnit | xUnit、单位转换测试 | ⬜ 未开始 |
| `Shared/QuantityTakeoff/FormulaEvaluatorTests.cs` | 公式计算器测试 | 低 | FormulaEvaluator | xUnit、公式解析测试 | ⬜ 未开始 |

### 模拟实现

| 文件路径 | 描述 | 优先级 | 依赖项 | 技术细节 | 状态 |
|---------|------|--------|-------|----------|------|
| `Mocks/MockMCPServerCommunication.cs` | 模拟MCP服务器通信 | 高 | IMCPServerCommunication | Moq、预定义响应 | ⬜ 未开始 |
| `Mocks/MockElementRepository.cs` | 模拟元素仓储 | 高 | IElementRepository | 内存仓储、测试数据 | ⬜ 未开始 |
| `Mocks/MockParameterRepository.cs` | 模拟参数仓储 | 高 | IParameterRepository | 内存仓储、测试数据 | ⬜ 未开始 |
| `Mocks/MockRevitAPIAdapter.cs` | 模拟Revit API适配器 | 高 | IRevitAPIAdapter | 模拟Revit API行为 | ⬜ 未开始 |
| `Mocks/QuantityTakeoff/MockQuantityCalculator.cs` | 模拟工程量计算器 | 低 | IQuantityCalculator | 模拟计算行为、测试数据 | ⬜ 未开始 |
| `Mocks/QuantityTakeoff/MockQuantityExporter.cs` | 模拟工程量导出器 | 低 | IQuantityExporter | 模拟导出行为、测试数据 | ⬜ 未开始 |

### Plugin层测试

| 文件路径 | 描述 | 优先级 | 依赖项 | 技术细节 | 状态 |
|---------|------|--------|-------|----------|------|
| `Plugin/Domain/Models/RevitElementTests.cs` | 元素模型测试 | 高 | RevitElement | xUnit、实体验证 | ⬜ 未开始 |
| `Plugin/Domain/Services/RevitElementServiceTests.cs` | 元素服务测试 | 高 | RevitElementService、MockElementRepository | xUnit、服务验证 | ⬜ 未开始 |
| `Plugin/Application/Commands/StartMCPCommandTests.cs` | 启动MCP命令测试 | 中 | StartMCPCommand、MockMCPServerCommunication | xUnit、命令执行验证 | ⬜ 未开始 |
| `Plugin/Infrastructure/Communication/ProcessCommunicationTests.cs` | 进程通信测试 | 高 | ProcessCommunication、MockServerProcess | xUnit、IPC通信测试 | ⬜ 未开始 |
| `Plugin/Domain/Services/QuantityTakeoff/ParameterCalculatorTests.cs` | 参数计算器测试 | 低 | ParameterCalculator、MockElementRepository | xUnit、计算逻辑测试 | ⬜ 未开始 |
| `Plugin/Domain/Services/QuantityTakeoff/GeometryCalculatorTests.cs` | 几何计算器测试 | 低 | GeometryCalculator、MockElementRepository | xUnit、几何计算测试 | ⬜ 未开始 |
| `Plugin/Domain/Services/QuantityTakeoff/HybridCalculatorTests.cs` | 混合计算器测试 | 低 | HybridCalculator、MockQuantityCalculator | xUnit、策略选择测试 | ⬜ 未开始 |
| `Plugin/Application/Commands/QuantityTakeoff/CalculateQuantitiesCommandTests.cs` | 计算工程量命令测试 | 低 | CalculateQuantitiesCommand、MockQuantityCalculator | xUnit、命令处理测试 | ⬜ 未开始 |

### 集成测试

| 文件路径 | 描述 | 优先级 | 依赖项 | 技术细节 | 状态 |
|---------|------|--------|-------|----------|------|
| `Integration/Plugin/RevitElementRepositoryIntegrationTests.cs` | 元素仓储集成测试 | 高 | RevitElementRepository、MockRevitAPIAdapter | xUnit、仓储与模拟适配器集成 | ⬜ 未开始 |
| `Integration/Communication/IPCCommunicationTests.cs` | IPC通信集成测试 | 高 | ProcessCommunication、IPCProtocol | xUnit、实际进程间通信测试 | ⬜ 未开始 |
| `Integration/Plugin/RevitApiAdapterIntegrationTests.cs` | Revit API适配器集成测试 | 中 | RevitAPIAdapter、RevitTestFramework | RevitTestFramework、API调用验证 | ⬜ 未开始 |
| `Integration/QuantityTakeoff/QuantityCalculationIntegrationTests.cs` | 工程量计算集成测试 | 低 | QuantityTakeoffService、MockElementRepository | xUnit、跨层计算测试 | ⬜ 未开始 |
| `Integration/QuantityTakeoff/ExportIntegrationTests.cs` | 工程量导出集成测试 | 低 | QuantityExporter、MockFileSystem | xUnit、文件生成测试 | ⬜ 未开始 |

### 性能测试

| 文件路径 | 描述 | 优先级 | 依赖项 | 技术细节 | 状态 |
|---------|------|--------|-------|----------|------|
| `Performance/Communication/IPCPerformanceTests.cs` | IPC通信性能测试 | 中 | ProcessCommunication | BenchmarkDotNet、吞吐量测试 | ⬜ 未开始 |
| `Performance/Serialization/MessageSerializationBenchmarks.cs` | 消息序列化性能测试 | 中 | MessageSerializer | BenchmarkDotNet、序列化性能 | ⬜ 未开始 |
| `Performance/QuantityTakeoff/CalculationPerformanceTests.cs` | 工程量计算性能测试 | 低 | QuantityCalculator | BenchmarkDotNet、大模型计算测试 | ⬜ 未开始 |
| `Performance/QuantityTakeoff/ExportPerformanceTests.cs` | 工程量导出性能测试 | 低 | QuantityExporter | BenchmarkDotNet、大数据导出测试 | ⬜ 未开始 |

### 测试工具和辅助类
| 文件路径 | 描述 | 优先级 | 依赖项 | 技术细节 | 状态 |
|---------|------|--------|-------|----------|------|
| `TestHelpers/TestData.cs` | 测试数据生成器 | 高 | 无 | 模拟数据、固定测试集 | ⬜ 未开始 |
| `TestHelpers/AssertionExtensions.cs` | 断言扩展方法 | 中 | FluentAssertions | 自定义断言、领域验证 | ⬜ 未开始 |
| `TestHelpers/TestProcessRunner.cs` | 测试进程运行器 | 高 | 无 | 进程启动、IPC测试辅助 | ⬜ 未开始 |
| `TestHelpers/QuantityTakeoff/TestQuantityModels.cs` | 工程量测试模型 | 低 | 无 | 工程量测试数据、模型生成 | ⬜ 未开始 |
| `TestHelpers/QuantityTakeoff/QuantityAssertions.cs` | 工程量断言助手 | 低 | FluentAssertions | 专用断言、误差容忍、单位转换 | ⬜ 未开始 |

```csharp
// 在测试项目中的模拟实现
public class MockMCPServerCommunication : IMCPServerCommunication
{
    public Task<ResponseMessage> SendQueryAsync(QueryMessage message)
    {
        // 返回预定义的测试响应
        return Task.FromResult(new ResponseMessage { ... });
    }
    
    public Task<bool> IsServerRunningAsync()
    {
        return Task.FromResult(true); // 始终假设服务器运行中
    }
}
```

### 优势

1. **解耦与可替换性**
   - 接口定义在共享层，任何实现都可以替换
   - 测试时无需实际运行Server进程

2. **更好的测试覆盖率**
   - 可以创建各种边缘情况的模拟实现
   - 测试异常处理和错误恢复逻辑

3. **符合DDD原则**
   - 遵循依赖倒置原则
   - 高层模块不依赖于低层模块的实现细节

4. **简化集成测试**
   - 集成测试可以通过实际IPC通信进行
   - 确保通信机制可靠且稳定

### 注意事项

1. **接口稳定性**
   - 共享层接口应尽可能稳定，避免频繁变更
   - 使用接口版本控制管理不兼容变更

2. **序列化兼容性**
   - 确保跨进程传输的数据模型有良好的序列化支持
   - 考虑使用协议缓冲区或类似技术提高效率

3. **Shared层模型设计**
   - 共享模型应为纯数据模型，不包含业务逻辑
   - 使用DTO模式传输数据，避免暴露领域模型

### 开发指南

#### 如何使用此清单

1. **初始开发规划**
   - 根据业务需求和技术依赖确定开发优先级
   - 按照架构层次组织开发顺序（通常从领域层开始）
   - 建议先完成所有"高"优先级模块
   - 使用依赖项列识别正确的开发顺序

2. **开发进度跟踪**
   - 使用状态标记跟踪每个模块的开发状态
   - 定期更新清单，反映当前开发进度
   - 组织架构审查，确保模块间依赖关系正确

3. **迭代开发计划**
   - 第一迭代：基础设施和核心领域模型
     - Domain/Models/Element.cs
     - Domain/Models/Parameter.cs
     - Domain/Interfaces/IElementRepository.cs
     - Infrastructure/RevitAPI/RevitAPIAdapter.cs
     - Infrastructure/Communication/ProcessCommunication.cs
     - Shared/Models/RevitElementInfo.cs
     - Shared/Interfaces/IRevitElement.cs
     - Shared/Communication/MCPMessage.cs
   - 第二迭代：基本查询和修改功能
     - Domain/Services/ElementService.cs
     - Application/Queries/GetElementsByCategoryQuery.cs
     - Application/Commands/GetElementCommand.cs
     - Presentation/MCP/Tools/ElementQueryTool.cs
     - Shared/DTOs/ElementDTO.cs
     - Shared/DTOs/ParameterDTO.cs
   - 第三迭代：高级功能和界面优化
   - 第四迭代：性能优化和错误处理完善

### 依赖关系

- **领域层不依赖任何其他层**
  - 模型类应是自包含的，只依赖其他领域模型
  - 领域服务可以使用领域模型和接口
  - 领域接口定义抽象依赖，实现由基础设施层提供

- **应用层依赖领域层**
  - 命令和查询使用领域服务协调业务逻辑
  - DTO用于数据传输，避免直接暴露领域模型
  - 应用服务负责协调工作流，不包含业务规则

- **基础设施层实现领域层接口**
  - 仓储实现对应的领域接口
  - 适配器将外部技术转换为领域概念
  - 提供领域服务所需的技术实现

- **表示层依赖应用层和领域层**
  - UI组件使用应用层命令和查询
  - MCP工具使用应用层服务
  - 避免直接访问基础设施层组件

- **共享层(RevitMCP.Shared)是跨项目共享的基础**
  - 共享模型和接口被Plugin和Server项目同时使用
  - 不包含业务逻辑，只提供数据结构和接口定义
  - 必须保持向后兼容性，避免破坏性更改
  - 应使用netstandard2.0确保最广泛的兼容性

### 架构一致性检查清单

- [ ] 所有领域模型保持独立，不依赖外部层
- [ ] 所有仓储接口定义在领域层，实现在基础设施层
- [ ] 应用层服务不包含业务规则，只协调领域对象
- [ ] 表示层不直接访问基础设施层
- [ ] 跨层通信使用接口或DTO，不直接传递领域对象
- [ ] 领域事件用于跨聚合根或模块的通信
- [ ] 共享层模型不包含业务逻辑，只有数据结构和接口定义
- [ ] Plugin和Server项目正确引用共享层而不是复制代码
- [ ] 共享层接口版本稳定，避免破坏性更改

### 技术栈参考

1. **核心技术**
   - .NET 8 (RevitMCP.Plugin)
   - .NET 9 (RevitMCP.Server)
   - .NET Standard 2.0 (RevitMCP.Shared)
   - Revit API 2025 \References\
   - MCP SDK

2. **架构框架**
   - CQRS模式分离命令和查询
   - 领域驱动设计 (DDD)
   - 依赖注入容器 (Microsoft.Extensions.DependencyInjection)

3. **测试框架**
   - xUnit (单元测试和集成测试)
   - Moq (模拟框架)
   - FluentAssertions (流畅断言)
   - RevitTestFramework (Revit特定测试)

4. **工具库**
   - AutoMapper (对象映射)
   - Serilog (结构化日志)
   - Newtonsoft.Json (JSON序列化)
   - System.Text.Json (高性能JSON)

### 通信层

- ✓ 共享通信接口设计 (`Shared/Interfaces/IMCPServerCommunication.cs`)
- ✓ 消息模型设计 (`Shared/Communication/MCPMessage.cs` 和派生类)
- ✓ 通信协议定义 (`Shared/Communication/IPCProtocol.cs`)
- ✓ 基础进程间通信实现 (`Plugin/Infrastructure/Communication/ProcessCommunication.cs`, `Server/Infrastructure/Communication/ProcessCommunication.cs`)
- ✓ 基础通信单元测试 (`Shared.Tests/Communication/MessageSerializationTests.cs`)
- ✓ 基础通信集成测试 (`Integration.Tests/Communication/ProcessCommunicationTests.cs`)
- ⬜ 高级通信特性 (断线重连、大消息分片处理等)
- ⬜ Revit UI线程适配优化 
- ⬜ 性能压力测试
