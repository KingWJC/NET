# .NET

手写IOC容器

ADO.NET

ASP.NET MVC

ORM: EF, SqlSugar

Web API

ASP.NET Core、EFCore、Core WebAP

MySQL数据库调优、Redis、MongoDB、RabbitMQ、Kafka等高并发中间件

熟悉微服务架构、熟悉Docker&Kubernetes容器技术，熟悉CI/CD

.NET5+Docker落地微服务架构

- C#图解教程(第五版) **(5星推荐入门教程)**
- C#高级编程
- CLR via C#  **(很厚很厚的书，可以看做工具书使用)**
- ASP.NET MVC 5高级编程
- 《深入浅出ASP.NET Core》

- 菜鸟教程（https://www.runoob.com/csharp/csharp-tutorial.html）
- 微软官方文档(https://docs.microsoft.com/zh-cn/dotnet/)
- https://docs.microsoft.com/zh-cn/dotnet/csharp/tour-of-csharp/types
- 全网最全MVC教程汇总(https://www.cnblogs.com/mq0036/p/9008969.html) ，其中的ASP.NET MVC 小牛之路系列可以重点看看，对你理解MVC实战很有帮助。
- 2021新版Asp.Net MVC | 入门到精通自学必备最全实战教程(2021新版Asp.Net MVC | 入门到精通自学必备最全实战教程（C#/.NET入门教程/.Net Core/程序员/编程）B0222_哔哩哔哩_bilibili)
- 关于ASP.NET Core的分享之路(壹佰文章最全总结| 《关于ASP.NETCore的分享之路》 - 老张的哲学 - 博客园 (cnblogs.com))
- 系列一：ASP.NET Core学习视频教程(Blog.Core项目)(系列一、ASP.NET Core 学习视频教程（Blog.Core）_哔哩哔哩_bilibili)
- ASP.NET Core合集(ASP.NET Core合集_哔哩哔哩_bilibili)

# [ASP.NET](http://ASP.NET) Core 开发者指南

> 2019 年 [ASP.NET](http://ASP.NET) Core 开发者指南:

在下面，您可以看到一个图，说明可以采用的路径以及要成为 [ASP.NET](http://ASP.NET) Core 开发人员所想要学习的库。我将此图作为提示，给所有问我 “作为 [ASP.NET](http://ASP.NET) Core 开发人员，接下来应该学习什么？” 的人。

## 免责声明

> 该指南的目的是为了给读者心有个大概的轮廓。如果你对接下来要学习的内容感到困惑时，这张路线图将指导你，而不是鼓励你选择时髦的东西。 你应该逐渐理解为什么一种工具比另一种工具更适合某些场景，并且记住时髦和新颖的东西并不总是意味着最适合这个工作。

## 请给一个星星! ⭐

如果你喜欢或正在使用这个项目进行学习或引用在你的解决方案中，请给它一个星星。谢谢!

## 路线图

![https://github.com/MoienTajik/AspNetCore-Developer-Roadmap/raw/master/aspnetcore-developer-roadmap.zh-Hans.png](https://github.com/MoienTajik/AspNetCore-Developer-Roadmap/raw/master/aspnetcore-developer-roadmap.zh-Hans.png)

## 资源

1. 先决条件
   - [C#](https://www.pluralsight.com/paths/csharp)
   - [Entity Framework](https://www.pluralsight.com/search?q=entity framework core)
   - [ASP.NET Core](https://www.pluralsight.com/search?q=asp.net core)
   - SQL 基础知识
2. 通用开发技能
   - 学习 GIT, 在 GitHub 中创建开源项目
   - 掌握 HTTP(S) 协议, 及其请求方法 (GET, POST, PUT, PATCH, DELETE, OPTIONS)
   - 不要害怕使用 Google, [Google 搜索技巧](http://www.powersearchingwithgoogle.com/)
   - 学习 [dotnet CLI](https://docs.microsoft.com/zh-cn/dotnet/core/tools)
   - 阅读一些关于算法和数据结构的书籍
3. 依赖注入
   1. DI 容器
      - [Microsoft.Extensions.DependencyInjection](https://docs.microsoft.com/zh-cn/aspnet/core/fundamentals/dependency-injection)
      - [AutoFac](https://autofaccn.readthedocs.io/en/latest/integration/aspnetcore.html)
      - [Ninject](http://www.ninject.org/)
      - [StructureMap](https://github.com/structuremap/structuremap)
      - [Castle Windsor](https://github.com/castleproject/Windsor)
   2. [生命周期](https://docs.microsoft.com/zh-cn/aspnet/core/fundamentals/dependency-injection#service-lifetimes)
   3. [Scrutor](https://github.com/khellang/Scrutor)
4. 数据库
   1. 关系数据库
      1. [SQL Server](https://www.microsoft.com/zh-cn/sql-server/sql-server-2017)
      2. [PostgreSQL](https://www.postgresql.org/)
      3. [MariaDB](https://mariadb.org/)
      4. [MySQL](https://www.mysql.com/)
   2. 云数据库
      - [CosmosDB](https://docs.microsoft.com/zh-cn/azure/cosmos-db)
      - [DynamoDB](https://aws.amazon.com/dynamodb/)
   3. 搜索引擎
      - [ElasticSearch](https://www.elastic.co/)
      - [Solr](http://lucene.apache.org/solr/)
      - [Sphinx](http://sphinxsearch.com/)
   4. NoSQL
      - [MongoDB](https://docs.microsoft.com/zh-cn/aspnet/core/tutorials/first-mongo-app)
      - [Redis](https://redis.io/)
      - [Apache Cassandra](http://cassandra.apache.org/)
      - [LiteDB](https://github.com/mbdavid/LiteDB)
      - [RavenDB](https://github.com/ravendb/ravendb)
      - [CouchDB](http://couchdb.apache.org/)
5. 缓存
   1. Entity Framework 二级缓存
      1. [EFSecondLevelCache.Core](https://github.com/VahidN/EFSecondLevelCache.Core)
      2. [EntityFrameworkCore.Cacheable](https://github.com/SteffenMangold/EntityFrameworkCore.Cacheable)
   2. 分布式缓存
      1. [Redis](https://redis.io/)
      2. [Memcached](https://memcached.org/)
   3. [内存缓存](https://docs.microsoft.com/en-us/aspnet/core/performance/caching/memory)
6. 日志
   1. 日志框架
      - [Serilog](https://github.com/serilog/serilog)
      - [NLog](https://github.com/NLog/NLog)
      - [Elmah](https://elmah.github.io/)
      - [log4net](https://github.com/huorswords/Microsoft.Extensions.Logging.Log4Net.AspNetCore)
   2. 日志管理系统
      - [Sentry.io](http://sentry.io/)
      - [Loggly.com](https://loggly.com/)
      - [Elmah.io](http://elmah.io/)
7. 模板引擎
   1. [Razor](https://docs.microsoft.com/zh-cn/aspnet/core/mvc/views/razor)
   2. [DotLiquid](https://github.com/dotliquid/dotliquid)
   3. [Scriban](https://github.com/lunet-io/scriban)
   4. [Fluid](https://github.com/sebastienros/fluid)
8. 实时通信
   1. [SignalR](https://docs.microsoft.com/en-us/aspnet/core/signalr)
9. 对象映射
   - [AutoMapper](https://github.com/AutoMapper/AutoMapper)
   - [Mapster](https://github.com/MapsterMapper/Mapster)
   - [AgileMapper](https://github.com/agileobjects/AgileMapper)
   - [ExpressMapper](http://expressmapper.org/)
10. API 客户端
    1. REST
       - [OData](https://blogs.msdn.microsoft.com/odatateam/2018/07/03/asp-net-core-odata-now-available/)
       - [Sieve](https://github.com/Biarity/Sieve)
    2. GraphQL
       - [GraphQL-dotnet](https://github.com/graphql-dotnet/graphql-dotnet)
11. 最好掌握
    - [MediatR](https://github.com/jbogard/MediatR)
    - [Fluent Validation](https://github.com/JeremySkinner/FluentValidation)
    - [Swashbuckle](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)
    - [Benchmark.NET](https://github.com/dotnet/BenchmarkDotNet)
    - [Polly](https://github.com/App-vNext/Polly)
    - [NodaTime](https://github.com/nodatime/nodatime)
    - [GenFu](https://github.com/MisterJames/GenFu)
12. 测试
    1. 单元测试
       1. 测试框架
          - [MSTest](https://docs.microsoft.com/zh-cn/dotnet/core/testing/unit-testing-with-mstest)
          - [NUnit](https://docs.microsoft.com/zh-cn/dotnet/core/testing/unit-testing-with-nunit)
          - [xUnit](https://docs.microsoft.com/zh-cn/dotnet/core/testing/unit-testing-with-dotnet-test)
       2. 模拟工具
          - [Moq](https://github.com/moq/moq4)
          - [NSubstitute](https://github.com/nsubstitute/NSubstitute)
          - [FakeItEasy](https://github.com/FakeItEasy/FakeItEasy)
       3. 断言工具
          - [FluentAssertion](https://github.com/fluentassertions/fluentassertions)
          - [Shouldly](https://github.com/shouldly/shouldly)
    2. 行为测试
       - [BDDfy](https://github.com/TestStack/TestStack.BDDfy)
       - [SpecFlow](https://github.com/techtalk/SpecFlow/tree/DotNetCore)
       - [LightBDD](https://github.com/LightBDD/LightBDD)
    3. 集成测试
       - [WebApplicationFactory](https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests)
       - [TestServer](https://koukia.ca/integration-testing-in-asp-net-core-2-0-51d14ede3968)
    4. 端到端测试
       - [Selenium](https://www.automatetheplanet.com/webdriver-dotnetcore2/)
       - [Puppeteer-Sharp](https://github.com/kblok/puppeteer-sharp)
13. 任务调度
    - [HangFire](https://github.com/HangfireIO/Hangfire)
    - [Coravel](https://github.com/jamesmh/coravel)
    - [Fluent Scheduler](https://github.com/fluentscheduler/FluentScheduler)
14. 微服务
    1. 消息队列
       - [RabbitMQ](https://www.rabbitmq.com/tutorials/tutorial-one-dotnet.html)
       - [Apache Kafka](https://github.com/confluentinc/confluent-kafka-dotnet)
       - [ActiveMQ](https://github.com/apache/activemq)
       - [Azure Service Bus](https://docs.microsoft.com/zh-cn/azure/service-bus-messaging/service-bus-messaging-overview)
    2. 消息总线
       - [MassTransit](https://github.com/MassTransit/MassTransit)
       - [NServiceBus](https://github.com/Particular/NServiceBus)
       - [CAP](https://github.com/dotnetcore/CAP)
15. SOLID 原则
    - [单一责任原则 (SRP)](https://www.dotnetcurry.com/software-gardening/1148/solid-single-responsibility-principle)
    - [开放封闭原则 (OCP)](https://www.dotnetcurry.com/software-gardening/1176/solid-open-closed-principle)
    - [里氏替换原则 (LSP)](https://www.dotnetcurry.com/software-gardening/1235/liskov-substitution-principle-lsp-solid-patterns)
    - [接口分离原则 (ISP)](https://www.dotnetcurry.com/software-gardening/1257/interface-segregation-principle-isp-solid-principle)
    - [依赖倒置原则 (DIP)](https://www.dotnetcurry.com/software-gardening/1284/dependency-injection-solid-principles)
16. 设计模式
    - [CQRS](https://docs.microsoft.com/zh-cn/azure/architecture/patterns/cqrs)
    - [装饰模式](https://www.dofactory.com/net/decorator-design-pattern)
    - [策略模式](https://www.dofactory.com/net/strategy-design-pattern)
    - [观察者模式](https://www.dofactory.com/net/observer-design-pattern)
    - [建造者模式](https://www.dofactory.com/net/builder-design-pattern)
    - [单例模式](https://www.dofactory.com/net/singleton-design-pattern)
    - [外观模式](https://www.dofactory.com/net/facade-design-pattern)
    - [中介者模式](https://www.dofactory.com/net/mediator-design-pattern)

## 总结

如果你认为该指南可以改进，请提交包含任何更新的 PR 并提交任何问题。此外，我将继续改进这个仓库，因此你可以 star 这个仓库以便于重新访问。

灵感来源： [React Developer RoadMap](https://github.com/adam-golab/react-developer-roadmap)

## 贡献

该指南是使用 [xMind](https://www.xmind.net/) 构建的。中文版项目文件为`aspnetcore-developer-roadmap.zh-Hans.xmind`。要修改它, 在[此处](https://www.xmind.net/download/)下载 xMind Zen, 点击 **Open File** 并选择项目中的 `xmind` 文件。它将为你渲染生成路线图，更新它，上传和更新 readme 中的图像并创建一个 PR（使用[Compressor.io](https://compressor.io/compress)压缩导出的 PNG；不过中国无法访问，用 PS 吧）。

- 改进后提交 PR
- 在 Issues 中讨论问题
- 推广项目
