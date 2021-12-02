using System;
using System.Threading;

namespace code.sample
{
    /// <summary>
    /// 线程基础
    /// </summary>
    public class ThreadSample
    {
        /// <summary>
        /// 线程销毁及取消销毁,
        /// .net core 版本不支持abort
        /// </summary>
        public void TestAbort()
        {
            Thread t = new Thread(() =>
            {
                try
                {
                    Console.WriteLine("1.try内部，调用Abort前。");
                    // ……等待其他线程调用该线程的Abort()
                    Thread.Sleep(1000);
                    Console.WriteLine("try内部，调用Abort后。");
                }
                catch (ThreadAbortException abortEx)
                {
                    Console.WriteLine("3.catch:" + abortEx.GetType());
                    // 在catch中调用了ResetAbort，则try块外面的代码依旧执行，即：线程没有终止.
                    Thread.ResetAbort();
                    Console.WriteLine("4.catch：调用ResetAbort()。");
                }
                finally
                {
                    Console.WriteLine("5.finally");
                    // 在finally中调用Thread.ResetAbort()不能取消线程的销毁
                    // Thread.ResetAbort();
                }
                Console.WriteLine("6.try外面，调用Abort后。");
            });
            t.Start();

            t.Abort();
            Console.WriteLine("2.主线程，调用Abort。");
        }

        /// <summary>
        /// 线程中断,中断处于WaitSleepJoin线程状态的线程
        /// </summary>
        public void TestInterrupt()
        {
            Thread t = new Thread(() =>
            {
                try
                {
                    Console.WriteLine("1.try内部，调用Interrupt前。");
                    // ……等待其他线程调用该线程的Interrupt()
                    Thread.Sleep(1000);
                    Console.WriteLine("try内部，调用Interrupt后。");
                }
                catch (ThreadInterruptedException interruptEx)
                {
                    Thread.CurrentThread.Interrupt();
                    Console.WriteLine("ThreadState:" + Thread.CurrentThread.ThreadState);
                    Console.WriteLine("3.catch:" + interruptEx.GetType());
                }
                finally
                {
                    Console.WriteLine("ThreadState:" + Thread.CurrentThread.ThreadState);
                    Console.WriteLine("5.finally");
                }
                Console.WriteLine("6.try外面，调用Interrupt后。");
            });
            t.Start();

            t.Interrupt();
            Console.WriteLine("2.主线程，调用Interrupt。");
        }
    }
}