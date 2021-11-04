/*
 * @Author: KingWJC
 * @Date: 2021-11-01 09:55:59
 * @LastEditors: KingWJC
 * @LastEditTime: 2021-11-03 11:17:08
 * @Descripttion: 
 * @FilePath: \code\sample\ParallelTaskSample.cs
 *
 * 任务和并行编程(Task,Parallel)
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using static System.Console;

namespace code.sample {
    public class TaskSample {
        public static void Test () {
            while (true) {
                string input = ReadLine ();
                switch (input) {
                    case "-p":
                        TasksUsingThreadPool ();
                        break;
                    case "-s":
                        RunSynchronousTask ();
                        break;
                    case "-l":
                        LongRunningTask ();
                        break;
                    case "-r":
                        TaskWithResultDemo ();
                        break;
                    case "-c":
                        ContinuationTasks ();
                        break;
                    case "-pc":
                        ParentAndChild ();
                        break;
                    case "-ct":
                        CancelTask ();
                        break;
                    case "exit":
                        return;
                    default:
                        ShowUsage ();
                        break;
                }
            }
        }

        private static void ShowUsage () {
            WriteLine ("TaskSamples option");
            WriteLine ("options");
            WriteLine ("\t-p\tUse Thread Pool");
            WriteLine ("\t-s\tUse Synchronous Task");
            WriteLine ("\t-l\tUse Long Running Task");
            WriteLine ("\t-r\tTask with Result");
            WriteLine ("\t-c\tContinuation Tasks");
            WriteLine ("\t-pc\tParent and Child");
        }

        /*
         * @description: 创建一个新任务，并使用线程池中的一个线程
         * @param {*}
         * @return {*}
         */
        private static void TasksUsingThreadPool () {
            TaskFactory taskFactory = new TaskFactory ();
            Task t1 = taskFactory.StartNew (TaskMethod, "using a task factory");

            Task t2 = new Task (TaskMethod, "using a task constructor and Start");
            t2.Start ();

            Task t3 = Task.Factory.StartNew (TaskMethod, "factory via a task");

            Task t4 = Task.Run (() => TaskMethod ("using the Run method"));
        }

        /*
         * @description: 同步任务 
         * @param {*}
         * @return {*}
         */
        private static void RunSynchronousTask () {
            TaskMethod ("just the main thread");
            var t1 = new Task (TaskMethod, "run sync");
            // 使用相同的线程，若以前没有任务，会创建一个新任务。
            t1.RunSynchronously ();
        }

        /*
         * @description: 长时间运行的任务
         * @param {*}  告诉任务调度器等待没有意义，直接创建新线程。
         * @return {*} 
         */
        private static void LongRunningTask () {
            var t1 = new Task (TaskMethod, "long running", TaskCreationOptions.LongRunning);
            t1.Start ();
        }

        /*
         * @description: 使用元组传递参数，并接收任务的结果
         * @param {*}
         * @return {*}
         */
        private static void TaskWithResultDemo () {
            Task < (int, int) > t1 = Task.Run (() => TaskWithResult (Tuple.Create (8, 3)));
            // result属性被禁用，直到任务完成
            WriteLine (t1.Result);
            t1.Wait ();
            WriteLine ($"result from task: {t1.Result.Item1}  {t1.Result.Item2}");
        }

        private static (int, int) TaskWithResult (object division) {
            Task.Delay (10000).Wait ();
            var div = (Tuple<int, int>) division;
            int result = div.Item1 / div.Item2;
            int reminder = div.Item1 % div.Item2;
            WriteLine ("task creates a result");
            return (result, reminder);
        }

        /*
         * @description: 连续任务
         * @param {*}
         * @return {*}
         */
        public static void ContinuationTasks () {
            Task t1 = new Task (DoOnFirst);
            Task t2 = t1.ContinueWith (DoOnSecond, TaskContinuationOptions.OnlyOnRanToCompletion);
            Task t3 = t1.ContinueWith (DoOnSecond);
            Task t4 = t2.ContinueWith (DoOnSecond);
            t1.Start ();
        }

        private static void DoOnFirst () {
            WriteLine ($"doing some task {Task.CurrentId}");
            Task.Delay (3000).Wait ();
        }

        private static void DoOnSecond (Task t) {
            WriteLine ($"task {t.Id} finished");
            WriteLine ($"this task id {Task.CurrentId}");
            WriteLine ("do some cleanup");
            Task.Delay (3000).Wait ();
        }

        /*
         * @description: 嵌套任务，父任务内创建子任务。子任务创建使用AttachedToParent。
         * @param {*}  
         * @return {*}  取消父任务，也会取消子任务。
         */
        private static void ParentAndChild () {
            var parent = new Task (ParentTask);
            parent.Start ();
            Task.Delay (2000).Wait ();
            // 是 WaitingForChildrenToComplete.
            WriteLine (parent.Status);
            Task.Delay (4000).Wait ();
            // 变为 RanToCompletion
            WriteLine (parent.Status);
        }

        private static void ParentTask () {
            WriteLine ($"task id {Task.CurrentId}");
            var child = new Task (ChildTask, TaskCreationOptions.AttachedToParent);
            child.Start ();
            Task.Delay (1000).Wait ();
            WriteLine ("parent started child");
        }

        private static void ChildTask () {
            WriteLine ("child");
            Task.Delay (5000).Wait ();
            WriteLine ("child finished");
        }

        /*
         * @description: 取消任务
         * @param {*}
         * @return {*}
         */
        private static void CancelTask () {
            var cts = new CancellationTokenSource ();
            cts.Token.Register (() => WriteLine ("***** task cancelled ****"));
            cts.CancelAfter (500);

            var t1 = Task.Run (() => {
                WriteLine ("in task");
                for (int i = 0; i < 20; i++) {
                    Task.Delay (100).Wait ();
                    CancellationToken token = cts.Token;
                    if (token.IsCancellationRequested) {
                        WriteLine ("cancelling was reauested, cancelling from within the task");
                        token.ThrowIfCancellationRequested ();
                        break;
                    }
                    WriteLine ("in loop");
                }
                WriteLine ("task finished without cancellation");
            }, cts.Token);

            try {
                t1.Wait ();
            } catch (AggregateException ex) {
                WriteLine ($"exception:{ex.GetType().Name},{ex.Message}");
                foreach (var innerException in ex.InnerExceptions) {
                    WriteLine ($"inner exception:{innerException.GetType().Name},{innerException.Message}");
                }
            }
        }

        private static void TaskMethod (object obj) {
            Log (obj?.ToString ());
        }

        private static object logLock = new object ();

        /*
         * @description: 写入日志，线程信息（线程池或后台线程）
         * @param {*}
         * @return {*}
         */
        private static void Log (string title) {
            lock (logLock) {
                WriteLine (title);
                WriteLine ($"Task id: {Task.CurrentId?.ToString() ?? "no task"}, Thread:{Thread.CurrentThread.ManagedThreadId}");
                WriteLine ($"is pooled thread: {Thread.CurrentThread.IsThreadPoolThread}");
                WriteLine ($"is background thread: {Thread.CurrentThread.IsBackground}");
                WriteLine ();
            }
        }
    }

    public class ParallelSample {
        public static void Test () {
            while (true) {
                string obj = ReadLine ();
                //在switch中使用return,不仅会跳出switch,还会直接结束循环
                switch (obj) {
                    case "pf":
                        ParallelFor ();
                        break;
                    case "pfa":
                        ParallelForWithAsync ();
                        break;
                    case "spfe":
                        StopParallelForEarly ();
                        break;
                    case "pfi":
                        ParallelForWithInit ();
                        break;
                    case "pfe":
                        ParallelForEach ();
                        break;
                    case "pi":
                        ParallelInvoke ();
                        break;
                    case "cp":
                        CancelParallelFor ();
                        break;
                    case "exit":
                        return;
                    default:
                        ShowUsage ();
                        break;
                }
            }
        }

        private static void ShowUsage () {
            WriteLine ("ParallelSamples options");
            WriteLine ("Options:");
            WriteLine ("\t-pf\tParallel For");
            WriteLine ("\t-pfa\tParallel For Async");
            WriteLine ("\t-spfe\tStop Parallel For Early");
            WriteLine ("\t-pfwi\tParallel For With Init");
            WriteLine ("\t-pfe\tParallel ForEach");
            WriteLine ("\t-pi\tParallel Invoke");
            WriteLine ("\t-cp\tCancel Parallel");
        }

        private static void ParallelFor () {
            ParallelLoopResult result = Parallel.For (0, 10, i => {
                Log ($"S {i}");
                //阻塞当前线程，直到延迟结束，为了有机会创建新的线程，否则会用更少的任务和线程。
                Task.Delay (10000).Wait ();
                // Thread.Sleep(100); 作用一样。
                Log ($"E {i}");
            });
            WriteLine ($"Is completed: {result.IsCompleted}");
        }

        /*
         * @description: 异步嵌套。Parallel的For方法没有延迟，直接完成。
         * @param {*}
         * @return {*} 结束时，任务不再存在，只有线程。因为Parallel只等待它创建的任务，而不等待其它后台任务。
         * 延迟后，有可能看不到结束日志的输出，因为主线程结束，所有后台线程被终止。
         */
        private static void ParallelForWithAsync () {
            ParallelLoopResult result = Parallel.For (0, 10, async i => {
                Log ($"S {i}");
                await Task.Delay (10000);
                Log ($"E {i}");
            });
            WriteLine ($"Is completed: {result.IsCompleted}");
        }

        /*
         * @description: 按条件终止个别的迭代任务
         * @param {*}
         * @return {*}
         */
        private static void StopParallelForEarly () {
            ParallelLoopResult result = Parallel.For (10, 40, (int i, ParallelLoopState pls) => {
                Log ($"S {i}");
                if (i > 12) {
                    pls.Break ();
                    Log ($"break now {i}");
                }
                Task.Delay (1000).Wait ();
                Log ($"E {i}");
            });
            WriteLine ($"Is completed: {result.IsCompleted}");
            WriteLine ($"Lowest break iteration : {result.LowestBreakIteration}");
        }

        /*
         * @description: 初始化的全部参数（线程的初始化和结束方法）
         * @param {*}
         * @return {*}
         */
        private static void ParallelForWithInit () {
            ParallelLoopResult result = Parallel.For<string> (0, 10, () => {
                // invoked once for each thread
                Log ("init thread");
                return $"t{Thread.CurrentThread.ManagedThreadId}";
            }, (int i, ParallelLoopState pls, string threadID) => {
                // invoked for each member
                Log ($"body i {i}, thread id : {threadID}");
                Task.Delay (10).Wait ();
                return $"i {i}";
            }, (string index) => {
                // final action on each thread
                Log ($"finally {index}");
            });
        }

        private static void ParallelForEach () {
            string[] data = { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve" };
            Parallel.ForEach<string> (data, (item, pls, i) => {
                WriteLine (item);
            });
        }

        /*
         * @description: 并行执行多个方法
         * @param {*}
         * @return {*}
         */
        private static void ParallelInvoke () {
            System.Action first = () => {
                Task.Delay (100).Wait ();
                Log ("first result");
            };

            System.Action second = () => {
                Task.Delay (100).Wait ();
                Log ("second result");
            };
            Parallel.Invoke (first, second);
        }

        /*
         * @description: 取消并发任务（启动的迭代操作允许完成，其它迭代操作在启动前就取消了，因为取消操作以协作方式进行，避免取消迭代操作中间泄露资源)
         * @param {*}
         * @return {*}
         */
        private static void CancelParallelFor () {
            var cts = new CancellationTokenSource ();
            // 注册任务取消时，执行的函数。
            cts.Token.Register (() => WriteLine ("*****token cancelled*****"));
            // 500ms后发送取消的指令
            cts.CancelAfter (500);
            try {
                ParallelLoopResult result = Parallel.For (0, 100, new ParallelOptions {
                    CancellationToken = cts.Token
                }, x => {
                    WriteLine ($"Loop {x} started");
                    int sum = 0;
                    for (int i = 0; i < 100; i++) {
                        Task.Delay (30).Wait ();
                        sum += i;
                    }
                    WriteLine ($"Sum is {sum} , loop {x} finished");
                });
            } catch (Exception ex) {
                WriteLine (ex.Message);
            }
        }

        private static void Log (string prefix) {
            WriteLine ($"{prefix} Task:{Task.CurrentId} Thread:{Thread.CurrentThread.ManagedThreadId}");
        }
    }

    public class DataFlowSample {
        public static void Test () {
            // Task t1 = Task.Run (Producer);
            // Task t2 = Task.Run (async () => await ConsumerAsync ());
            // Task.WaitAll (t1, t2);

            var target = SetPipeline ();
            target.Post (".\\points");
            ReadLine();
        }

        /*
         * @description: 源和目标数据块：实现ISourceBlock,ITargetBlock接口的数据块，可同时做数据源和数据目标
         */
        private static BufferBlock<string> s_buffer = new BufferBlock<string> ();

        /*
         * @description: 生产数据
         * @param {*}
         * @return {*}
         */
        private static void Producer () {
            bool isExit = false;
            while (!isExit) {
                string message = ReadLine ();
                if (string.Compare (message, "exit", ignoreCase : true) == 0) {
                    isExit = true;
                } else {
                    s_buffer.Post (message);
                }
            }
        }

        /*
         * @description: 消费数据
         * @param {*}
         * @return {*}
         */
        private static async Task ConsumerAsync () {
            while (true) {
                string message = await s_buffer.ReceiveAsync ();
                WriteLine ($"user input {message}");
            }
        }

        /*
         * @description: 连续块，组成数据管道
         * @param {*}
         * @return {*}
         */
        private static ITargetBlock<string> SetPipeline () {
            var fileNameForPath = new TransformBlock<string, IEnumerable<string>> (path => {
                    return GetFileNames (path);
                });
            var lines = new TransformBlock<IEnumerable<string>, IEnumerable<string>> (fileNames => {
                    return LoadLines (fileNames);
                });
            var words = new TransformBlock<IEnumerable<string>, IEnumerable<string>> (lines => {
                    return GetWords (lines);
                });
            var display = new ActionBlock<IEnumerable<string>> (words => {
                foreach (var item in words) {
                    WriteLine (item);
                }
            });

            fileNameForPath.LinkTo (lines);
            lines.LinkTo (words);
            words.LinkTo (display);

            return fileNameForPath;
        }

        /*
         * @description: 读取目录下的文件
         * @param {*}
         * @return {*}
         */
        private static IEnumerable<string> GetFileNames (string path) {
            foreach (var fileName in Directory.EnumerateFiles (path, "*.cs")) {
                yield return fileName;
            }
        }

        /*
         * @description: 读取文件的内容行
         * @param {*}
         * @return {*}
         */
        private static IEnumerable<string> LoadLines (IEnumerable<string> fileNames) {
            foreach (var item in fileNames) {
                using (FileStream fileStream = File.OpenRead (item)) {
                    var reader = new StreamReader (fileStream);
                    string readLine = null;
                    while ((readLine = reader.ReadLine ()) != null) {
                        yield return readLine;
                    }
                }
            }
        }

        /*
         * @description: 拆分每行内容中的单词
         * @param {*}
         * @return {*}
         */
        private static IEnumerable<string> GetWords (IEnumerable<string> lines) {
            foreach (var item in lines) {
                string[] words = item.Split (' ', ';', '{', '}', '.', ',');
                foreach (var word in words) {
                    if (!String.IsNullOrEmpty (word)) {
                        yield return word;
                    }
                }
            }
        }
    }
}