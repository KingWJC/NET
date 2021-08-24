/*
 * @Author: KingWJC
 * @Date: 2021-08-19 14:19:51
 * @LastEditors: KingWJC
 * @LastEditTime: 2021-08-24 10:47:42
 * @Descripttion: 
 * @FilePath: \code\programs\documentconsume\Client.cs
 *
 * 队列Queue的示例：文档的生成和获取。
 * 使用任务实现生产者和消费者模式。
 *
 */
using System;
using System.Threading;

namespace code.programs.documentconsume
{
    public class Client
    {
        public static void Test()
        {
            DocumentManager dm = new DocumentManager();
            ProcessDocument.Start(dm);

            for (int i = 0; i < 100; i++)
            {
                Document document = new Document($"title {i}", $"content");
                dm.Add(document);
                Console.WriteLine($"Add {document}");
                Thread.Sleep(new Random().Next(20));
            }
        }
    }
}