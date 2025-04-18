# ERROR_HANDLING

> 本文件用于说明项目的错误处理机制、异常设计和常见问题。

# RevitMCP 错误处理与版本兼容性设计

本文档描述了 RevitMCP 的错误处理策略、异常管理机制和版本兼容性设计，以确保系统的健壮性和稳定性。

## 1. 错误处理架构

### 1.1 错误分类

RevitMCP 将错误分为以下几个主要类别：

1. **技术错误**
   - API 调用错误（请求格式错误、参数无效等）
   - 基础设施错误（网络问题、服务不可用等）
   - 系统错误（内存不足、进程崩溃等）

2. **业务逻辑错误**
   - 领域验证错误（数据验证失败）
   - 业务规则冲突（状态转换无效等）
   - 资源约束错误（超出限制、资源不足等）

3. **集成错误**
   - Revit API 错误（API 调用失败、事务回滚等）
   - 跨进程通信错误（进程间通信失败）
   - 外部系统集成错误（同步失败、映射错误等）

4. **权限错误**
   - 认证错误（凭证无效、会话过期等）
   - 授权错误（权限不足、操作禁止等）
   - 限流错误（请求频率超限等）

### 1.2 错误标识体系

1. **错误码结构**
   ```
   [错误类型]-[模块]-[具体错误码]
   ```
   示例：
   - `TECH-API-001`：API 格式错误
   - `BIZ-MODEL-003`：元素状态转换无效
   - `INT-REVIT-002`：Revit 事务失败
   - `AUTH-PERM-001`：权限不足

2. **错误级别**
   - `Fatal`：严重错误，需要立即干预
   - `Error`：错误，需要处理但系统可继续运行
   - `Warning`：警告，可能导致问题的情况
   - `Info`：信息性消息，不影响操作

### 1.3 错误响应标准

1. **API 错误响应格式**
   ```json
   {
     "error": {
       "code": "BIZ-MODEL-003",
       "message": "无法将元素状态从 'COMPLETED' 更改为 'IN_PROGRESS'",
       "details": "元素状态遵循单向流程，无法回退到先前状态",
       "correlationId": "c0a8f9b3-1234-5678-9abc-def012345678",
       "timestamp": "2023-08-15T10:30:00Z",
       "path": "/api/elements/123/status",
       "suggestedAction": "请检查状态转换规则或联系管理员"
     }
   }
   ```

2. **用户友好错误描述**
   - 详细错误描述，避免技术术语
   - 提供可能的原因和解决方案
   - 包含跟踪标识符用于支持
   - 避免暴露内部实现细节

## 2. Revit API 异常管理

### 2.1 Revit API 常见异常

1. **事务异常**
   - 事务提交失败
   - 事务被中止
   - 事务范围错误
   - **ModificationOutsideTransactionException**: 当尝试在事务外修改模型时触发，常见于忘记将修改操作包裹在事务中
   - **RegenerationFailedException**: 当文档重生成过程失败时触发，通常发生在无效的几何操作后

2. **元素操作异常**
   - 元素不存在或已被删除
   - 元素修改冲突
   - 元素关系约束违反
   - **AutoJoinFailedException**: 当自动连接墙体、楼板等元素时失败
   - **ArgumentException**: 当给Revit API方法提供无效参数时触发

3. **API 使用异常**
   - 无效参数
   - API 使用顺序错误
   - 功能不支持
   - **InternalException**: 表示未预期的失败路径，包含额外诊断信息，可反馈给Autodesk

### 2.2 异常封装策略

1. **异常转换层**
   - 将 Revit API 原生异常转换为领域异常
   - 保留原始异常信息用于调试
   - 使用一致的异常层次结构

   ```csharp
   try
   {
       // Revit API 调用
   }
   catch (Autodesk.Revit.Exceptions.InvalidOperationException ex)
   {
       throw new RevitMCP.Domain.Exceptions.ElementOperationException(
           "无法修改已锁定元素", 
           "INT-REVIT-005", 
           ex);
   }
   ```

2. **异常丰富**
   - 添加上下文信息（操作ID、用户等）
   - 记录相关元素信息
   - 添加系统状态信息

3. **异常传播控制**
   - 适当位置处理异常，避免无控制传播
   - 使用专门的异常处理中间件
   - 实现全局异常处理器

### 2.3 事务管理

1. **事务策略**
   - 最小化事务范围和持续时间
   - 批量操作使用单一事务
   - 实现事务重试机制
   - **事务嵌套限制**: 避免深层嵌套事务，嵌套超过16层会导致崩溃
   - **共享事务模式**: 使用`TransactionMode.Shared`允许多个命令共享同一事务

2. **事务恢复**
   - 记录事务前状态
   - 事务失败后回滚到一致状态
   - 提供手动恢复选项
   - **部分提交保护**: 实现所有操作成功或全部回滚的原子性保证
   
   ```csharp
   try
   {
       using (Transaction transaction = new Transaction(doc, "批量更新"))
       {
           FailureHandlingOptions options = transaction.GetFailureHandlingOptions();
           FailurePreprocessor preprocessor = new CustomFailurePreprocessor();
           options.SetFailuresPreprocessor(preprocessor);
           transaction.SetFailureHandlingOptions(options);
           
           transaction.Start();
           
           // 执行批量操作
           bool allSucceeded = PerformBatchOperations();
           
           // 只有全部成功才提交
           if (allSucceeded)
               transaction.Commit();
           else
               transaction.RollBack();
       }
   }
   catch (Exception ex)
   {
       // 记录异常并进行恢复处理
       LogException(ex);
       RecoverFromFailure();
   }
   ```

3. **事务监控**
   - 监控长时间运行事务
   - 记录事务统计信息
   - 警告可能的事务死锁
   - **故障恢复点**: 在复杂操作的关键点设置故障恢复点，允许部分回滚

## 3. 跨进程通信错误处理

### 3.1 通信故障模式

1. **连接中断**
   - 检测进程间连接丢失
     - **心跳检测**: 定期发送小型心跳消息验证连接状态
     - **超时检测**: 设置操作超时阈值，超过阈值判定为连接异常
   - 实现连接重试和恢复机制
     - **指数退避算法**: 连接失败后逐渐增加重试间隔时间
     - **最大重试次数**: 设置上限防止无限重试消耗资源
   - 在连接恢复前维持状态
     - **状态缓存**: 保存关键操作状态，连接恢复后继续处理
     - **脏标记**: 标记未同步的本地更改，恢复连接后自动同步

2. **消息处理错误**
   - 消息序列化/反序列化失败
   - 消息处理超时
   - 消息顺序错误
   - **消息重传机制**: 未收到确认的消息自动重传
   - **消息去重**: 使用唯一标识符识别并忽略重复消息

3. **进程崩溃**
   - 检测进程崩溃
     - **监控进程ID**: 定期检查目标进程是否存在
     - **进程退出代码监控**: 区分正常退出和异常崩溃
   - 自动重启进程
     - **进程重启策略**: 定义重启流程、最大重启次数及间隔
     - **启动参数传递**: 保存关键状态参数用于重启时恢复
   - 恢复正在处理的操作
     - **操作日志**: 记录所有关键操作，用于恢复中断的工作
     - **断点续传**: 从最后成功点继续执行操作

### 3.2 恢复策略

1. **心跳机制**
   - 定期发送心跳消息
   - 检测通信延迟和中断
   - 触发恢复程序

2. **消息持久化**
   - 持久化重要消息
   - 支持消息重播
   - 幂等消息处理

3. **状态同步**
   - 定期检查状态一致性
   - 自动解决不一致状态
   - 维护操作日志用于重建

### 3.3 降级服务

1. **部分功能可用**
   - 定义核心功能和非核心功能
   - 在通信中断时保持核心功能可用
   - 提供降级服务指示

2. **离线模式**
   - 支持有限的离线操作
   - 恢复连接后同步更改
   - 解决冲突的策略

## 4. MCP 协议错误处理

### 4.1 MCP 错误模式

1. **协议错误**
   - 协议版本不兼容
     - **版本协商失败**: 客户端和服务器支持的协议版本无交集
     - **功能集不匹配**: 请求的功能在协商的版本中不可用
   - 消息格式错误
   - 序列号不连续
   - **协议握手错误**: 初始连接阶段的协议协商失败

2. **工具调用错误**
   - 工具不存在
   - 工具参数无效
   - 工具执行失败
   - **工具超时**: 工具执行时间超过预定阈值
   - **资源占用过高**: 工具执行导致资源（CPU、内存）使用超过限制

3. **资源访问错误**
   - 资源不存在
   - 资源访问超时
   - 资源格式错误
   - **资源锁定**: 资源被其他进程或操作锁定
   - **权限不足**: 客户端权限不足以访问请求的资源

### 4.2 错误通信

1. **标准错误消息**
   - 使用 MCP 定义的错误消息格式
   - 包含错误类型和详细信息
   - 支持结构化错误数据

2. **友好错误呈现**
   - 将技术错误转换为用户友好文本
   - 提供上下文相关的解决建议
   - 支持多语言错误消息

### 4.3 恢复机制

1. **会话恢复**
   - 支持 MCP 会话恢复
   - 维持会话状态
   - 自动重连和继续

2. **批处理保护**
   - 批处理操作的原子性
   - 部分失败处理
   - 错误报告聚合

## 5. 版本兼容性设计

### 5.1 Revit API 版本兼容性

#### 5.1.1 版本检测和适配

1. **功能探测**
   - 运行时检测 Revit API 能力
   - 根据 Revit 版本选择适当实现
   - 维护功能支持矩阵
   - **反射探测**: 使用反射检测特定API是否可用
   
   ```csharp
   public bool IsMethodAvailable(string methodName)
   {
       Type type = typeof(Autodesk.Revit.DB.Document);
       return type.GetMethod(methodName) != null;
   }
   ```

2. **API 封装**
   - 封装版本特定的 API 调用
   - 提供统一接口
   - 隔离版本差异
   - **版本特定实现**: 根据Revit版本加载不同的实现类

#### 5.1.2 降级策略

1. **功能降级**
   - 定义高级和基本功能
   - 在低版本上回退到基本实现
   - 明确功能限制

2. **替代实现**
   - 为不支持的功能提供替代实现
   - 使用多种方法实现相同目标
   - 记录功能差异

#### 5.1.3 未来版本适配

1. **抽象访问层**
   - 使用工厂和策略模式抽象 API 访问
   - 允许动态加载版本特定实现
   - 简化新版本支持

2. **版本测试套件**
   - 针对不同 Revit 版本的测试套件
   - 自动验证兼容性
   - 检测 API 行为变化

### 5.2 MCP 协议版本兼容性

#### 5.2.1 协议版本管理

1. **协议协商**
   - 支持客户端和服务器协议版本协商
   - 使用最高共同支持版本
   - 拒绝不兼容的连接
   - **版本协商算法**: 
   
   ```
   function mcp-version(client-min, client-max, server-min, server-max)
   1. if (client-max >= server-min and server-max >= client-min) then
      1. return min(server-max, client-max)
   2. else
      1. return NONE
   ```

2. **版本映射**
   - 维护协议版本映射
   - 在版本间转换消息格式
   - 支持向后兼容
   - **协议翻译层**: 实现不同协议版本间的消息转换适配器

#### 5.2.2 功能广告

1. **能力发现**
   - 公布支持的功能和版本
   - 允许客户端查询功能支持
   - 根据客户端能力调整行为

2. **功能矩阵**
   - 维护功能支持矩阵
   - 明确标记实验性功能
   - 提供功能使用建议

### 5.3 API 版本策略

#### 5.3.1 API 版本控制

1. **URL 路径版本**
   ```
   /api/v1/elements
   /api/v2/elements
   ```

2. **HTTP 头版本**
   ```
   Accept: application/json; version=1.0
   ```

3. **版本共存**
   - 同时支持多个 API 版本
   - 明确的版本生命周期政策
   - 版本迁移指南

#### 5.3.2 兼容性保证

1. **兼容性承诺**
   - 同一主版本内保持向后兼容
   - 明确不兼容变更
   - 提供详细的迁移指南

2. **增量变更**
   - 优先使用非破坏性变更
   - 添加而非修改或删除
   - 使用废弃标记和过渡期

## 6. 实施指南

### 6.1 错误处理最佳实践

1. **开发规范**
   - 异常处理编码标准
   - 错误日志记录规范
   - 错误码分配和维护流程

2. **测试策略**
   - 错误场景测试
   - 故障注入测试
   - 恢复机制验证

### 6.2 监控与告警

1. **错误监控**
   - 监控关键错误率
   - 设置错误阈值告警
   - 错误模式分析

2. **恢复监控**
   - 监控恢复操作成功率
   - 重试频率监控
   - 平均恢复时间跟踪

### 6.3 文档与支持

1. **错误参考文档**
   - 完整的错误码文档
   - 常见问题排查指南
   - 自助解决方案

2. **支持工具**
   - 诊断工具和脚本
   - 日志分析工具
   - 状态一致性检查工具 