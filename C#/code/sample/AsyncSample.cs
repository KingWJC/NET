/*
 * @Author: KingWJC
 * @Date: 2021-09-03 17:25:42
 * @LastEditors: KingWJC
 * @LastEditTime: 2021-11-02 14:25:48
 * @Descripttion: 
 * @FilePath: \code\sample\AsyncSample.cs
 *
 * 异步编程
 * 利用Bing Search API进行搜索, 需要azure 注册
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using static System.Console;

namespace code.sample {
    public class AsyncSample {
        static CancellationTokenSource cts = new CancellationTokenSource ();
        static bool flag = true;

        public static void Test () {
            Usage ();

            while (flag) {
                string arg = ReadLine ();
                switch (arg) {
                    case "sync":
                        WebRequestSync ();
                        break;
                    case "delegate1":
                        DelegatePattern1 ();
                        break;
                    case "delegate2":
                        DelegatePattern2 ();
                        break;
                    case "event":
                        EventPattern ();
                        break;
                    case "thread":
                        ThreadPattern ();
                        break;
                    case "task":
                        TaskPattern ();
                        break;
                    case "comb":
                        MultipleAsyncMethodsWithCombinators ();
                        break;
                    case "casync":
                        ConvertingAsyncPattern ();
                        break;
                    case "cancel":
                        cts.Cancel ();
                        break;
                    case "esc":
                        flag = false;
                        break;
                }

                for (int i = 0; i < 5; i++) {
                    Thread.Sleep (1000);
                    WriteLine (Thread.CurrentThread.ManagedThreadId);
                }
            }
        }

        private static void Usage () {
            WriteLine ("Usage: Foundations Command");
            WriteLine ();
            WriteLine ("Commands:");
            WriteLine ("\t-sync\t\tcaller with sync");
#if NET46
            WriteLine ("\t-delegate(1/2)\t\tcaller with delegate");
#endif
            WriteLine ("\t-event\t\tcaller with event");
            WriteLine ("\t-thread\t\tcaller with thread");
            WriteLine ("\t-task\t\tcaller with task");
            WriteLine ("\t-async\t\tcaller with async");
            WriteLine ("\t-comb\t\tmultiple async methods with combinators");
#if NET46
            WriteLine ("\t-casync\t\tconvert async pattern");
#endif
            WriteLine ("\t");
            return;
        }

        private static void WebRequestSync () {
            try {
                const string accessKey = "enter key here";
                const string uriBase = "https://api.cognitive.microsoft.com/bing/v7.0/search";
                const string searchTerm = "Microsoft Cognitive Services";
                var uriQuery = uriBase + "?q=" + Uri.EscapeDataString (searchTerm);
                var request = HttpWebRequest.Create (uriQuery);
                request.Headers["Ocp-Apim-Subscription-Key"] = accessKey;
                HttpWebResponse resp = (HttpWebResponse) request.GetResponseAsync ().Result;
                string json = new StreamReader (resp.GetResponseStream ()).ReadToEnd ();

                var searchResult = new SearchResult {
                    jsonResult = json,
                    releventHeaders = new Dictionary<string, string> ()
                };

                foreach (string header in resp.Headers) {
                    if (header.StartsWith ("BingAPIs-") || header.StartsWith ("X-MSEdge-"))
                        searchResult.releventHeaders[header] = resp.Headers[header];
                }
                WriteLine (resp);
            } catch (Exception e) {
                WriteLine (e.Message);
            }
        }

        /* 异步模式,只有.Net Framwork平台 支持 */
        private static void DelegatePattern1 () {
            List<string> list = new List<string> ();

            Func<string, int, string> downloadString = (address, index) => {
                WriteLine ($"web url address:{address}");
                Thread.Sleep (3000);
                return $"download file {index}";
            };

            Action<string> addItem = item => list.Add (item);

            for (int i = 0; i < 10; i++) {
                downloadString.BeginInvoke ("http://www.baidu.com", i, ar => {
                    ar.AsyncWaitHandle.WaitOne (100);
                    string resp = downloadString.EndInvoke (ar);
                    addItem.Invoke (resp);
                }, null);
            }

            foreach (var item in list) {
                WriteLine (item);
            }

        }

        private static void DelegatePattern2 () {
            BeginGreeting ("wjc", ar => {
                string result = EndGreeting (ar);
                WriteLine (result);
            }, null);
        }

        private static void EventPattern () {
            var client = new WebClient ();
            client.DownloadStringCompleted += (sender, el) => {
                WriteLine ($"download string: {el.Result}");
            };
            client.DownloadStringAsync (new Uri ("http://www.baidu.com"));
        }

        private static void ThreadPattern () {
            //ThreadStart,ParameterizedThreadStart 是委托,可用匿名表达式.
            Thread thread = new Thread (new ParameterizedThreadStart (data => {
                WriteLine ($"web url address:{data}");
                Thread.Sleep (3000);
                WriteLine ($"download file {data} complete.");
            }));
            thread.Start ("www.baidu.com");
        }

        private static void TaskPattern () {
            TraceThreadAndTask ("started Task Pattern");

            Task<String> task = Task.Factory.StartNew (() => {
                return Greeting ("wangjicheng");
            });
            task.ContinueWith (t => {
                WriteLine ($"{t.Result} complete");
                TraceThreadAndTask ("finished Task Pattern");
            });
            task.Wait ();
        }

        private async static void MultipleAsyncMethodsWithCombinators () {
            Task taskResult = null;
            try {

                // string s1 = await GreetingAsync("wangjicheng");
                // string s2 = await GreetingAsync("wangyan");

                // string s3 = GreetingAsync("wwww").GetAwaiter().GetResult();

                TraceThreadAndTask ($"started {nameof(MultipleAsyncMethodsWithCombinators)}");

                Task<string> t1 = GreetingAsync ("www");
                Task<string> t2 = GreetingAsync ("wy");

                // await 修饰相当于Task.GetAwaiter().GetResult()的执行，异步等待最终获取异步方法的执行结果。
                // display the exception information of the first and second task
                await (taskResult = Task.WhenAll (t1, t2));
                WriteLine ($"Finish both method.\n Result 1: {t1.Result}\n Result2: {t2.Result}");

                TraceThreadAndTask ("finished MultipleAsyncMethodsWithCombinators");
            } catch (Exception ex) {
                WriteLine ($"handled {ex.Message}");
                foreach (var item in taskResult.Exception.InnerExceptions) {
                    WriteLine ($"inner exception: {item.Message}");
                }
            }
        }

        private static async void ConvertingAsyncPattern () {
            string result = await Task<string>.Factory.FromAsync<string> (BeginGreeting, EndGreeting, "wjc", null);
            WriteLine (result);
        }

        private static Func<string, string> greetingInvoker = Greeting;

        private static IAsyncResult BeginGreeting (string name, AsyncCallback callback, object state) {
            return greetingInvoker.BeginInvoke (name, callback, state);
        }

        private static string EndGreeting (IAsyncResult ar) {
            return greetingInvoker.EndInvoke (ar);
        }

        private static string Greeting (string name) {
            TraceThreadAndTask ("running Greeting");
            Task.Delay (3000).Wait ();
            // throw new Exception($"{name} exception");
            return $"Hello {name}";
        }

        private static Task<string> GreetingAsync (string name) {
            return Task.Run<string> (() => {
                Task.Delay (3000).Wait ();
                cts.Token.ThrowIfCancellationRequested ();
                return Greeting (name);
            }, cts.Token);
        }

        private static void TraceThreadAndTask (string info) {
            string taskInfo = Task.CurrentId == null ? "no task" : "task" + Task.CurrentId;
#if NET46
            WriteLine ($"{info} in thread {Thread.CurrentThread.ManagedThreadId} and  {taskInfo}");
#else
            WriteLine ($"{info} in {taskInfo}");
#endif
        }

        private struct SearchResult {
            public string jsonResult;
            public Dictionary<string, string> releventHeaders;
        }
    }

    /*
     * @description: 异步委托，错误示例和解释
     * @param {*}  WatiOne和Callback的执行顺序理解错误。
     * @return {*}
     */
    public class AsyncDelegate {
        Action funcDelegate = AsyncDelegate.Func;
        string funcResult = null;
        public string Call () {
            IAsyncResult result = funcDelegate.BeginInvoke (Callback, null);
            result.AsyncWaitHandle.WaitOne ();
            return funcResult;
        }

        static void Func () {
            Thread.Sleep (1000);
        }

        /* Callback在WaitOne之后执行。
         * ASyncWaitHandle.WaitOne() 在异步操作完成时发出信号。同时调用CallBack()，意味着 WaitOne() 之后的代码在主线程中运行，而 CallBack 则在另一个线程中运行（可能与运行 func() 相同）。这会导致竞争条件，其中 longOpResult 的值在返回时基本上是未知的
         * 由于您的操作Func已完成，因此Call在回调操作完成之前，控制将在内部恢复并且完成函数.
         * WaitOne()只会等待你的异步操作完成，而不是你的回调
         * 在线程池中回调函数其实和异步处理函数用的同一线程。 只是回调函数是在myHander.EndInvoke(Result)或Result.AsyncWaitHandle.WaitOne() 之后才运行，它运行完了线程池中的这个线程才空闲。
         */
        void Callback (IAsyncResult result) {
            funcResult = "Completed";
            funcDelegate.EndInvoke (result);
        }
    }

    /*
     * @description: result.WaitOne只意味着目标BeginInvoke已经完成，而不是回调。所以只需将后处理代码放入BeginInvoke委托中即可
     * @param {*}
     * @return {*}
     */
    public class AsyncDelegate1 {
        string funcResult = null;

        public string Call () {
            Action funcDelegate = delegate { Func (); CallAfter (); };
            IAsyncResult result = funcDelegate.BeginInvoke (r => funcDelegate.EndInvoke (r), null);
            result.AsyncWaitHandle.WaitOne ();
            return funcResult;
        }

        void Func () {
            Thread.Sleep (1000);
            WriteLine ("invoke func");
        }

        void CallAfter () {
            funcResult = "Completed";
            Thread.Sleep (1000);
            WriteLine ("invoke callbacck");
        }
    }

    /*
     * @description: 如果要同时等待操作和回调，则需要自己处理, 使用ManualResetEvent 来让主线程等待 CallBack 设置 longOpResult
     * @param {*}
     * @return {*}
     */
    public class AsyncDelegate3 {
        // the delegate
        Action funcDelegate = AsyncDelegate3.Func;
        // thre result
        string funcResult = null;
        // Declare a manual reset at module level so it can be 
        // handled from both your callback and your called method
        ManualResetEvent waiter;
        public string Call () {
            // Set a manual reset which you can reset within your callback
            waiter = new ManualResetEvent (false);
            // Call the asynchronous operation
            IAsyncResult result = funcDelegate.BeginInvoke (Callback, null);

            waiter.WaitOne ();
            // result.AsyncWaitHandle.WaitOne ();
            return funcResult;
        }

        static void Func () {
            Thread.Sleep (1000);
        }

        void Callback (IAsyncResult result) {
            funcResult = "Completed";
            funcDelegate.EndInvoke (result);
            waiter.Set ();
        }
    }
}