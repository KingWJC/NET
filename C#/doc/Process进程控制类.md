### 进程

进程是操作系统级别的一个资源分配的基本单位，操作系统就将某个程序加载到内存中时，既包含该程序所需要的资源，同时还对这些资源进行基本的内存边界管理。

### Process类

1. 调用外部程序以及执行dos命令。
2. 负责启动和停止本机进程，获取或设置进程优先级，确定进程是否响应，是否已经退出，
3. 获取系统正在运行的所有进行列表和各进程资源占用情况。
4. 可以查询远城计算机上进程相关信息，包括进程内的线程集合、加载模块(.dll文件和.exe文件)和性能信息(如当前进程使用的内存量)

#### 属性

- Id：获取进程ID,也就是任务管理器中PID。
- ProcessName：进程名称，也就是exe程序的文件名称排除后缀。

- HasExited：判断进程是否已经退出。

- ExitCode：退出代码，0表示正常退出，非0表示错误编号，只有通过Process.Start方式启动返回的对象的HasExited属性为true才可以访问ExitCode.

- ExitTime:退出时间，只有通过Process.Start方式启动返回的对象的HasExited属性为true才可以访问ExitTime。

- MachineName：获取进程所在的机器名，如果为点，则表示本机。

- MainModule：获取关联进程的主模块,返回类型为ProcessModule。主模块就是Main函数所在的exe文件。当访问该属性出现Win32Exception异常时，表示32bit进程访问64bit进程模块，通过VS->Properties->Build->General->Platform Target(目标平台)->将Any Cpu或x86设置为x64即可。

- MainWindowTitle：获取进程的主窗口标题。

- Modules：获取进程加载的模块，也就是exe文件和dll文件

- **StartInfo**：设置或获取应用程序启动时传递的参数。如果为进程为图形用户界面，可以设置StartInfo.WindowStyle属性，指定启动时如何显示窗口，如果进程不是通过Process.Start方式进行启动的，StartInfo属性将不包含启动时使用的参数，使用MainModule属性获取相关启动信息。
- .StandardOutput.ReadToEnd();'，通过读取输出流，以便释放相应的缓冲。
- StartTime：进程启动的时间，可以用来计算进程运行时间。
- Threads：获取进程中运行的线程，也就是与当前进程关联的所有线程，主线程不一定是索引0的线程.返回类型为ProcessThread集合类型。

- TotalProcessorTime:获取进程的总的处理器时间，也就是CPU总耗时，是UserProcessorTime和PrivilegedProcessorTime时间之和，并非是程序允许总时间

- UserProcessorTime:获取进程的用户处理器时间。

- PrivilegedProcessorTime:获取进程的特权处理器时间。

- EnableRegisingEvents：是否引发Exited事件，默认为false。

- Close：释放与进程关联的所有资源，释放资源后无法在访问Process中的属性。


#### 方法和事件：

- **WaitForExit**：等待进程退出，可以设置等待超时时间。使当前线程处于等待状态，直到关联的进程终止。 此方法指示 Process 组件无限期地等待该进程退出。 这可能会导致应用程序停止响应。
- WaitForInputidle：等待进程进入空闲状态，会只适用于用户图形界面。一般主窗体创建完成才会返回true。可以设置等待超时时间。

- **Kill**：强制终止进程，只能对本机应用程序调用该方法，是终止没有图形化界面唯一的方法。由于Kill是异步执行，调用WaitForExit方法等待程序退出或使用HasExited属性判断是否已经退出。
- CloseMainWindow：通过向进程的主窗口发送关闭消息来关闭进程，其效果与为用户在界面中单击[关闭]按钮效果相同。如果成功发送关闭消息，则返回true，如果关联进程没有主窗口或禁用了主窗口则返回false。
- **Start**：启动进程。
- **OnExited**：事件，当应用程序退出时会触发该事件，需将EnableRegisingEvents属性设置为true。
- Process.GetProcesses：获取本地计算机或远程计算机上的所有进程信息，参数machineName：远程主机的IP或计算机名。
- Process.GetProcessById：根据进程ID获取进程Process对象，machineName：远程计算机的IP或计算名        
- Process.GetProcessByName：根据进程名称获取进行数组，machineName：远程计算机的IP或计算名。进程名称就是exe对应的文件名。

#### 示例

```C#
//实例一个Process类，启动一个独立进程
Process p = new Process();
//Process类有一个StartInfo属性
//设定程序名
p.StartInfo.FileName = "cmd.exe";
//设定程式执行参数
p.StartInfo.Arguments = "/c " + command;
//关闭Shell的使用
p.StartInfo.UseShellExecute = false;
//重定向标准输入 输出会重定向到标准输出流中，不在命令行屏幕上直接输出。
p.StartInfo.RedirectStandardInput = true;
p.StartInfo.RedirectStandardOutput = true;
//重定向错误输出
p.StartInfo.RedirectStandardError = true;
//设置不显示窗口
p.StartInfo.CreateNoWindow = true;
//启动
p.Start();

//也可以用这种方式输入要执行的命令
//不过要记得加上Exit要不然下一行程式执行的时候会当机
//p.StandardInput.WriteLine(command);
//p.StandardInput.WriteLine("exit");
//从输出流取得命令执行结果
return p.StandardOutput.ReadToEnd();

string  str = await proc.StandardOutput.ReadToEndAsync();
Message(new object[] { str });
//进程关闭
proc.Close();
```

### 问题

1. process.StandardInput InvalidOperationException: StandardIn 尚未重定向,
   StreamWriter sw = process.StandardInput. 之前需要先启动程序。 

2. process.StandardOutput.ReadToEnd() 假死，不接收数据。

   必须先关闭StandardInput.

3. Process.Start失败：Win32Exception (0x80004005): 系统找不到指定的文件

   解决：设置WorkingDirectory后无效。 

   原因：应用程序的权限问题。Win10对exe程序的读写限制级别高，需要设置访问权限或用administrator运行vs2019。

4. 参考：https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.process.outputdatareceived?view=netframework-4.8
