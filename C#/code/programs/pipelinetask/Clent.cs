/*
 * @Author: KingWJC
 * @Date: 2021-08-20 14:20:12
 * @LastEditors: KingWJC
 * @LastEditTime: 2021-08-24 10:47:52
 * @Descripttion: 
 * @FilePath: \code\programs\pipelinetask\Clent.cs
 *
 * 并发集合示例：多个任务形成管道，处理多个文件，并显示处理后结果。
 * BlockingCollection的默认集合类型是ConcurrentQueue.
 * 修改完后，需要执行output.CompleteAdding();，否则任务不结束。
 */

using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace code.programs.pipelinetask
{
    public class Clent
    {
        public static void Test()
        {
            StartPipelline().Wait();
        }

        public static async Task StartPipelline()
        {
            var fileNames = new BlockingCollection<string>();
            var lines = new BlockingCollection<string>();
            var words = new ConcurrentDictionary<string, int>();
            var items = new BlockingCollection<Info>();
            var coloredItems = new BlockingCollection<Info>();

            Task t1 = PipellineStage.ReadFileAsync(@".\programs\pipelinetask", fileNames);
            Task t2 = PipellineStage.LoadContentAsync(fileNames, lines);
            Task t3 = PipellineStage.ProcessContentAsync(lines, words);
            await Task.WhenAll(t1, t2, t3);

            Task t4 = PipellineStage.TransferContentAsync(words, items);
            Task t5 = PipellineStage.AddColorAsync(items, coloredItems);
            Task t6 = PipellineStage.ShowContentAsync(coloredItems);
            ColorConsole.WriteLine("stages 4, 5, 6 started");

            await Task.WhenAll(t4, t5, t6);
            ColorConsole.WriteLine("all stages finished");
        }
    }
}