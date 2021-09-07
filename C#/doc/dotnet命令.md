### 命令预览

```shell
C:\>dotnet
 
Usage: dotnet [options]
Usage: dotnet [path-to-application]
 
Options:
  -h|--help            Display help.
  --version         Display version.
 
path-to-application:
  The path to an application .dll file to execute.
  
C:\>dotnet -h
.NET 命令行工具 (2.1.202)
使用情况: dotnet [runtime-options] [path-to-application]
使用情况: dotnet [sdk-options] [command] [arguments] [command-options]
 
path-to-application:
  要执行的应用程序 .dll 文件的路径。
 
SDK 命令:
  new              初始化 .NET 项目。
  restore          还原 .NET 项目中指定的依赖项。
  run              编译并立即执行 .NET 项目。
  build            生成 .NET 项目。
  publish          发布 .NET 项目以进行部署(包括运行时)。
  test             使用项目中指定的测试运行程序运行单元测试。
  pack             创建 NuGet 包。
  migrate          将基于 project.json 的项目迁移到基于 MSBuild 的项目。
  clean            清除生成输出。
  sln              修改解决方案(SLN)文件。
  add              将引用添加到项目中。
  remove           从项目中删除引用。
  list             列出项目中的引用。
  nuget            提供其他 NuGet 命令。
  msbuild          运行 Microsoft 生成引擎 (MSBuild)。
  vstest           运行 Microsoft 测试执行命令行工具。
 
常用选项:
  -v|--verbosity        设置命令的详细级别。允许值为 q[uiet]、m[inimal]、n[ormal]、d[etailed] 和 diag[nostic]。
  -h|--help             显示帮助。
 
运行“dotnet 命令 --help”，获取有关命令的详细信息。
 
sdk-options:
  --version        显示 .NET Core SDK 版本。
  --info           显示 .NET Core 信息。
  -d|--diagnostics 启用诊断输出。
 
runtime-options:
  --additionalprobingpath <path>    要探测的包含探测策略和程序集的路径。
  --fx-version <version>            要用于运行应用程序的安装版共享框架的版本。
  --roll-forward-on-no-candidate-fx 已启用“不前滚到候选共享框架”。
  --additional-deps <path>          其他 deps.json 文件的路径。
```

### 创建项目

```shell
D:\>cd dotnetcode
D:\dotnetcode>dotnet new sln -o zmblog
已成功创建模板“Solution File”。
 
D:\dotnetcode>cd zmblog
D:\dotnetcode\zmblog>dotnet new classlib -o zmblog.Common
已成功创建模板“Class library”。

D:\dotnetcode\zmblog>dotnet new razor -o zmblog.WebApp
已成功创建模板“ASP.NET Core Web App”。

D:\dotnetcode\zmblog>dotnet sln add zmblog.Common/zmblog.Common.csproj
已将项目“zmblog.Common\zmblog.Common.csproj”添加到解决方案中。

D:\dotnetcode\zmblog>dotnet sln add zmblog.WebApp/zmblog.WebApp.csproj
已将项目“zmblog.WebApp\zmblog.WebApp.csproj”添加到解决方案中。
```

### 引用和包

```shell
# 列出当前项目的引用名称、包名称
dotnet list reference
dotnet list package
# 移除当前项目的引用名称、包名称
dotnet remove reference
dotnet remove package
# 向当前项目添加引用、包
dotnet add reference
dotnet add package [选项] <PACKAGE_NAME>
```

或者使用NuGet Package Manager扩展

1. 使用ctrl + shift + p或者ctrl + p（mac下将ctrl替换成cmd） 
2. 输入> nuget 在下拉框中
3. 选择>Nuget Package Manager:Add Package 
4. 输入需要安装的包名（不需要完整的包名，可以模糊搜索），
5. 进行搜索 进行版本选择并安装

安装nuget插件出现错误："Versioning information could not be retrieved from the NuGet package repository. Please try again later."

原因：主要是nuget插件里的拉组件的js文件没有进行小写的控制

解决方案：

1. 打开路径下的文件fetchPackageVersions.js（/Users/用户名/.vscode/extensions/jmrog.vscode-nuget-package-manager-1.1.6/out/src/actions/add-methods/fetchPackageVersions.js）
2. 修改代码：...node_fetch_1.default(`${versionsUrl}${selectedPackageName}/index.json`, utils_1.getFetchOptions(vscode.workspace.getConfiguration('http')))
3. 修改后的代码了toLowerCase()方法：...node_fetch_1.default(`${versionsUrl}${selectedPackageName.toLowerCase()}/index.json`, utils_1.getFetchOptions(vscode.workspace.getConfiguration('http')))
4. 重启vscode问题解决！