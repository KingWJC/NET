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
using System.Net;
using System.Net.Http;
using static System.Console;

namespace code.sample
{
    public class AsyncSample
    {
        const string accessKey = "enter key here";
        const string uriBase = "https://api.cognitive.microsoft.com/bing/v7.0/search";
        const string searchTerm = "Microsoft Cognitive Services";
        public static void Test()
        {
            // var client = new WebClient();
            // client.Headers["Ocp-Apim-Subscription-Key"] = accessKey;
            // client.Headers["User-Agent"] = "Professional C# Sample App";
            // client.Headers.Add("Ocp-Apim-Subscription-Key", accessKey);
            // client.headers.Add("User-Agent", "Professional C# Sample App");
            // client.Credentials = new NetworkCredential(accessKey, accessKey);

            var client = new HttpWeb();
            client.Headers["Ocp-Apim-Subscription-Key"] = accessKey;
            client.Headers["User-Agent"] = "Professional C# Sample App";

            // SearchSync(client);

            // DelegatePattern();
        }

        private static void SearchSync(WebClient client)
        {
            var uriQuery = uriBase + "?q=" + Uri.EscapeDataString(searchTerm);
            string resp = client.DownloadString(uriQuery);
            WriteLine(resp);
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

        private static void EventPattern()
        {

        }

        private struct SearchResult
        {
            public string jsonResult;
            public Dictionary<string, string> releventHeaders;
        }
    }
}