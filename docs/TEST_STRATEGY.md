# TEST_STRATEGY

> 本文件用于说明项目的测试策略、测试用例、覆盖率目标和测试工具。

# RevitMCP 测试策略

本文档详细说明了RevitMCP项目的测试策略、测试类型和测试实践。

## 1. 测试目标

RevitMCP测试策略的主要目标是：

- 确保软件功能符合需求规格
- 验证与Revit API的正确集成
- 确保MCP协议的正确实现
- 验证自然语言处理的准确性和可靠性
- 确保系统在各种条件下的稳定性和性能
- 支持持续集成和持续交付流程

## 1.1 测试框架实现

我们已经实现了一个基于xUnit的测试框架，包括：

- **RevitMCP.Tests**项目，包含单元测试、集成测试和系统测试
- 使用**Moq**进行依赖模拟
- 使用**FluentAssertions**提供流畅的断言语法
- 使用**Coverlet**进行代码覆盖率分析
- 使用**XunitXml.TestLogger**生成标准化测试报告
- 通过**GitHub Actions**实现持续集成

## 2. 测试级别

### 2.1 单元测试

单元测试关注于验证代码的最小可测试单元（通常是方法或类）的正确性。

**目标**：
- 验证各个组件的独立功能
- 确保业务逻辑的正确实现
- 提供快速反馈循环
- 支持重构和代码改进

**实现方法**：
- 使用xUnit作为主要测试框架
- 使用Moq创建模拟对象，隔离外部依赖
- 使用FluentAssertions提供更易读的断言
- 针对每个公共方法编写测试
- 覆盖正常路径和异常路径

**示例**：
```csharp
[Fact]
public void GetElementById_WhenElementExists_ReturnsElement()
{
    // Arrange
    var mockRepository = new Mock<IElementRepository>();
    var expectedElement = new Element { Id = 123, Name = "Test Wall" };
    mockRepository.Setup(r => r.GetById(123)).Returns(expectedElement);

    var service = new ElementService(mockRepository.Object);

    // Act
    var result = service.GetElementById(123);

    // Assert
    result.Should().NotBeNull();
    result.Id.Should().Be(123);
    result.Name.Should().Be("Test Wall");
}
```

### 2.2 集成测试

集成测试验证多个组件或子系统之间的交互。

**目标**：
- 验证组件之间的接口和交互
- 测试与外部系统的集成
- 验证数据流和通信
- 检测组件集成中的问题

**实现方法**：
- 使用TestHost进行集成测试
- 使用WireMock.NET模拟外部服务
- 使用内存数据库进行数据访问测试
- 测试层与层之间的集成
- 测试与Revit API的集成
- 测试与MCP协议的集成

**示例**：
```csharp
[Fact]
public async Task QueryElements_IntegrationTest()
{
    // Arrange
    var host = new TestHostBuilder()
        .ConfigureServices(services =>
        {
            services.AddRevitMCP();
            services.AddScoped<IRevitConnection>(provider =>
                new MockRevitConnection());
        })
        .Build();

    var client = host.GetTestClient();

    // Act
    var response = await client.PostAsJsonAsync("/api/elements/query",
        new { Category = "Walls", Parameter = "Height", Value = 3.0 });

    // Assert
    response.EnsureSuccessStatusCode();
    var elements = await response.Content.ReadFromJsonAsync<List<ElementDto>>();
    elements.Should().NotBeEmpty();
}
```

### 2.3 系统测试

系统测试验证整个系统的功能和非功能需求。

**目标**：
- 验证端到端工作流
- 测试系统作为一个整体的行为
- 验证性能、安全性和可靠性
- 模拟真实用户场景

**实现方法**：
- 使用自动化测试工具（如Selenium/Playwright）
- 执行端到端测试场景
- 性能和负载测试
- 安全性测试
- 可靠性和恢复测试

**示例**：
```csharp
[Fact]
public async Task EndToEndTest_QueryWallsByHeight()
{
    // Arrange
    var testEnvironment = new RevitMCPTestEnvironment();
    await testEnvironment.StartAsync();

    var client = testEnvironment.CreateClient();

    // Act
    var response = await client.SendNaturalLanguageQuery(
        "找出所有高度大于3米的墙");

    // Assert
    response.Should().Contain("找到了5面墙");

    // Cleanup
    await testEnvironment.StopAsync();
}
```

### 2.4 验收测试

验收测试确保系统满足用户需求和业务目标。

**目标**：
- 验证系统满足用户需求
- 确保系统可用性和用户体验
- 验证业务流程和规则
- 获取利益相关者的认可

**实现方法**：
- 基于用户故事和验收标准
- 使用行为驱动开发(BDD)方法
- 结合自动化和手动测试
- 用户参与测试过程

**示例**：
```gherkin
功能: 查询高墙

场景: 用户查询高于特定高度的墙
  假设 用户已连接到Revit模型
  当 用户询问"找出所有高度大于3米的墙"
  那么 系统应返回所有高度大于3米的墙的列表
  并且 响应应包含墙的数量
  并且 响应应包含每面墙的高度
```

## 3. 特殊测试考虑

### 3.1 Revit API测试

由于Revit API的特殊性，需要特别考虑：

**挑战**：
- Revit API需要在Revit环境中运行
- 测试可能修改模型数据
- 自动化测试复杂
- 版本依赖性

**解决方案**：
- **沙箱测试环境**：
  - 使用专门的测试环境运行Revit API测试
  - 使用测试专用的Revit模型
  - 在测试后恢复模型状态

- **事务管理**：
  - 在测试事务中执行所有修改
  - 在测试完成后回滚事务
  - 使用只读操作进行查询测试

- **模拟Revit API**：
  - 创建Revit API的模拟版本用于单元测试
  - 实现关键接口的测试替代品
  - 使用依赖注入支持测试

- **测试自动化**：
  - 使用Revit API的批处理能力
  - 开发专用的测试运行器
  - 集成到CI/CD流程

**示例**：
```csharp
[RevitTest]
public void ModifyWallHeight_ShouldUpdateHeight()
{
    // Arrange
    using var testModel = RevitTestModel.Open("TestWalls.rvt");
    using var transaction = new Transaction(testModel.Document, "Test");
    transaction.Start();

    var wallId = new ElementId(123456);
    var wall = testModel.Document.GetElement(wallId) as Wall;
    var originalHeight = wall.get_Parameter(BuiltInParameter.WALL_HEIGHT).AsDouble();

    var service = new WallService(new RevitConnection(testModel.Document));

    try
    {
        // Act
        service.ModifyWallHeight(wallId.IntegerValue, originalHeight + 1.0);

        // Assert
        var newHeight = wall.get_Parameter(BuiltInParameter.WALL_HEIGHT).AsDouble();
        newHeight.Should().BeApproximately(originalHeight + 1.0, 0.001);
    }
    finally
    {
        // Cleanup
        transaction.RollBack();
    }
}
```

### 3.2 跨进程通信测试

测试跨进程通信的策略：

**挑战**：
- 进程间通信复杂
- 异步通信难以测试
- 环境依赖性
- 错误处理复杂

**解决方案**：
- **端到端测试**：
  - 验证Revit插件与RevitMCP服务器之间的通信
  - 测试完整的通信流程
  - 验证消息传递的正确性

- **协议一致性测试**：
  - 确保消息格式和协议实现正确
  - 验证序列化和反序列化
  - 测试协议版本兼容性

- **错误处理测试**：
  - 模拟通信错误和异常情况
  - 测试重连和恢复机制
  - 验证错误报告和日志记录

- **性能测试**：
  - 测量通信延迟和吞吐量
  - 测试大数据量传输
  - 测试长时间运行的连接

**示例**：
```csharp
[Fact]
public async Task ProcessCommunication_ShouldTransferMessages()
{
    // Arrange
    var server = new RevitMCPServer();
    await server.StartAsync();

    var client = new RevitPluginClient();
    await client.ConnectAsync("localhost", server.Port);

    // Act
    var request = new QueryElementsRequest { Category = "Walls" };
    var response = await client.SendRequestAsync<QueryElementsResponse>(request);

    // Assert
    response.Should().NotBeNull();
    response.Elements.Should().NotBeEmpty();

    // Cleanup
    await client.DisconnectAsync();
    await server.StopAsync();
}
```

### 3.3 自然语言处理测试

测试与LLM交互的策略：

**挑战**：
- 自然语言输入变化多样
- LLM响应可能不确定
- 意图理解复杂
- 上下文管理难以测试

**解决方案**：
- **提示模板测试**：
  - 验证提示模板产生预期的结果
  - 测试不同输入变体
  - 验证模板参数替换

- **意图识别测试**：
  - 测试系统正确理解用户意图的能力
  - 验证参数提取的准确性
  - 测试意图分类和路由

- **边缘情况测试**：
  - 测试模糊或不完整的用户输入
  - 验证系统对不相关输入的处理
  - 测试错误恢复和澄清机制

- **回归测试**：
  - 维护测试用例库
  - 定期重新测试关键场景
  - 监控LLM集成的稳定性

**示例**：
```csharp
[Theory]
[InlineData("找出所有高度大于3米的墙", "Walls", "Height", ">", 3.0)]
[InlineData("显示高于3m的墙", "Walls", "Height", ">", 3.0)]
[InlineData("哪些墙超过3米高", "Walls", "Height", ">", 3.0)]
public void IntentRecognizer_ShouldExtractQueryParameters(
    string input, string expectedCategory, string expectedParameter,
    string expectedOperator, double expectedValue)
{
    // Arrange
    var recognizer = new IntentRecognizer();

    // Act
    var result = recognizer.RecognizeQueryIntent(input);

    // Assert
    result.Should().NotBeNull();
    result.Intent.Should().Be("QueryElements");
    result.Parameters["Category"].Should().Be(expectedCategory);
    result.Parameters["Parameter"].Should().Be(expectedParameter);
    result.Parameters["Operator"].Should().Be(expectedOperator);
    result.Parameters["Value"].Should().Be(expectedValue);
}
```

### 3.4 浏览器自动化测试

为验证RevitMCP的Web界面功能、性能和用户体验，我们实施了专门的浏览器自动化测试策略。

**主要测试目标**：
- 验证Web界面功能正确性
- 确保跨浏览器和设备兼容性
- 验证界面可访问性符合WCAG 2.1标准
- 测试Web界面的性能和响应时间
- 提供自动化回归测试，减少手动测试工作量

**技术栈**：
- 使用Playwright作为主要自动化测试框架
- 结合Jest作为测试运行器
- 集成Axe-core用于可访问性测试
- 使用Percy进行视觉回归测试
- 采用Lighthouse进行性能测试

**测试类型**：
- 功能测试：验证Web界面功能正常工作
- 视觉回归测试：确保UI外观在代码变更后保持一致
- 可访问性测试：确保Web界面符合可访问性标准
- 跨浏览器测试：验证在不同浏览器和设备上的一致性
- 性能测试：评估Web界面的速度和资源使用情况

**最佳实践**：
- 使用页面对象模型(POM)组织测试代码
- 采用可靠的选择器策略
- 实现智能等待机制
- 确保测试隔离和独立性
- 实施错误处理和调试策略

详细的浏览器自动化测试策略、实现方法和最佳实践见[浏览器自动化测试](BROWSER_AUTOMATION_TEST.md)文档。

## 4. 测试自动化与CI/CD

### 4.1 持续集成

**目标**：
- 快速发现并修复问题
- 保持代码质量
- 支持频繁集成
- 提供快速反馈

**实现方法**：
- 使用GitHub Actions或Azure DevOps进行CI
- 每次提交自动运行单元测试
- 定期运行集成测试和系统测试
- 代码覆盖率分析和报告
- 静态代码分析

**配置示例**：
```yaml
# GitHub Actions工作流示例
name: RevitMCP CI

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main, develop ]

jobs:
  build:
    runs-on: windows-latest

    steps:
    - uses: actions/checkout@v2

    - name: Setup .NET
      uses: actions/setup-dotnet@v1
      with:
        dotnet-version: '8.0.x'

    - name: Restore dependencies
      run: dotnet restore

    - name: Build
      run: dotnet build --no-restore

    - name: Test
      run: dotnet test --no-build --verbosity normal --collect:"XPlat Code Coverage"

    - name: Upload coverage
      uses: codecov/codecov-action@v1
```

### 4.2 测试环境

**环境类型**：
- **开发环境**：
  - 开发人员本地测试
  - 快速反馈循环
  - 单元测试和基本集成测试

- **集成环境**：
  - 自动化测试和集成测试
  - 模拟生产环境
  - 包含测试数据和服务

- **预生产环境**：
  - 性能测试和用户验收测试
  - 与生产环境相似
  - 最终验证和签核

**环境管理**：
- 使用容器化技术（如Docker）
- 环境配置自动化
- 环境状态监控
- 测试数据管理

### 4.3 测试数据管理

**策略**：
- 使用版本控制管理测试数据
- 自动生成测试数据
- 数据清理和重置机制
- 敏感数据处理

**实现方法**：
- 创建标准测试Revit模型
- 使用数据生成器创建测试数据
- 实现测试前后的数据重置
- 使用事务隔离测试影响

## 5. 测试文档和报告

### 5.1 测试计划

测试计划应包括：
- 测试范围和目标
- 测试环境和工具
- 测试时间表和里程碑
- 资源和责任分配
- 风险和缓解策略

### 5.2 测试用例

测试用例应包括：
- 测试ID和名称
- 测试目的和描述
- 前置条件
- 测试步骤
- 预期结果
- 实际结果
- 通过/失败状态

### 5.3 测试报告

测试报告应包括：
- 测试摘要和状态
- 测试覆盖率分析
- 发现的缺陷和问题
- 风险评估
- 建议和后续步骤

### 5.4 缺陷跟踪

缺陷报告应包括：
- 缺陷ID和标题
- 严重性和优先级
- 详细描述和重现步骤
- 环境信息
- 相关截图或日志
- 状态和分配

## 6. 测试最佳实践

### 6.1 测试驱动开发(TDD)

- 先编写测试，再实现功能
- 小步迭代开发
- 持续重构
- 保持测试简单明了

### 6.2 代码覆盖率目标

- 单元测试：80%以上的代码覆盖率
- 集成测试：覆盖所有关键路径
- 系统测试：覆盖所有用户场景
- 关注质量而非数量

### 6.3 测试命名和组织

- 使用描述性测试名称
- 遵循一致的命名约定
- 按功能或组件组织测试
- 保持测试独立性

### 6.4 测试维护

- 定期审查和更新测试
- 删除过时或冗余的测试
- 改进测试质量和效率
- 监控测试执行时间

## 7. 测试角色和责任

### 7.1 开发人员

- 编写单元测试和集成测试
- 在提交代码前运行测试
- 修复测试中发现的问题
- 参与代码审查和测试审查

### 7.2 测试工程师

- 设计和实施测试策略
- 创建和维护测试计划
- 执行系统测试和验收测试
- 报告和跟踪缺陷

### 7.3 DevOps工程师

- 设置和维护CI/CD管道
- 配置测试环境
- 监控测试执行和结果
- 优化测试自动化

### 7.4 产品所有者

- 定义验收标准
- 参与用户验收测试
- 确认功能满足业务需求
- 优先处理测试中发现的问题

## 8. 结论

RevitMCP的测试策略旨在确保软件质量、可靠性和用户满意度。通过实施全面的测试方法，我们可以：

- 提前发现并解决问题
- 确保软件符合需求和期望
- 支持快速、可靠的开发和部署
- 提高用户信任和满意度

测试不仅是验证软件的过程，也是提高软件质量和开发效率的关键实践。通过持续改进测试策略和实践，我们可以不断提高RevitMCP的质量和价值。
