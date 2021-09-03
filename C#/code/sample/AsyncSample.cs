/*
 * @Author: KingWJC
 * @Date: 2021-09-03 17:25:42
 * @LastEditors: KingWJC
 * @LastEditTime: 2021-09-03 17:38:17
 * @Descripttion: 
 * @FilePath: \code\sample\AsyncSample.cs
 */
using System;
using System.Threading;
using System.Collections.Generic;
using static System.Console;

namespace code.sample
{
    public class AsyncSample
    {
        public static void Test()
        {
            DelegatePattern();
        }

        private static void DelegatePattern()
        {
            List<string> list = new List<string>();

            Func<string, int, string> downloadString = (address, index) =>
             {
                 WriteLine($"address:{address}");
                 Thread.Sleep(3000);
                 return $"download file {index}";
             };

            Action<string> addItem = item => list.Add(item);

            for (int i = 0; i < 10; i++)
            {
                downloadString.BeginInvoke("http://www.baidu.com", i, ar =>
                 {
                     string resp = downloadString.EndInvoke(ar);
                     addItem.Invoke(resp);
                 }, null);
            }

            foreach (var item in list)
            {
                WriteLine(item);
            }

        }
    }
}