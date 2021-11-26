/*
 * @Author: KingWJC
 * @Date: 2021-11-18 09:11:14
 * @LastEditTime: 2021-11-24 10:17:25
 * @LastEditors: your name
 * @FilePath: \code\synch\WaitHandleSample.cs
 * @Description:  
 * 
 * WaitHandle基类，在异步委托的使用，只支持。Net Framwork 4.7版本以下。
 */
using System;
using System.Threading.Tasks;

namespace code
{
    public class WaitHandleSample
    {
        public delegate int TakeAWhileDelegate(int a, int b);

        public void Test()
        {
            TakeAWhileDelegate whileDelegate = TakeAWhile;
            IAsyncResult result = whileDelegate.BeginInvoke(1, 3000, data => $"data".WriteTemplate(), null);

            while (true)
            {
                Console.Write(".");
                // 等待任务完成的信号，50是等待超时时间。
                if (result.AsyncWaitHandle.WaitOne(50))
                {
                    Console.Write("Can get the result now");
                    break;
                }
            }

            int data = whileDelegate.EndInvoke(result);
            $"{data}".WriteTemplate();
        }

        public int TakeAWhile(int a, int ms)
        {
            Task.Delay(ms).Wait();
            return 24;
        }
    }
}