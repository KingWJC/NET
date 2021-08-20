/*
 * @Author: KingWJC
 * @Date: 2021-08-20 14:31:44
 * @LastEditors: KingWJC
 * @LastEditTime: 2021-08-20 16:14:47
 * @Descripttion: 
 * @FilePath: \code\programs\pipelinetask\PipellineStage.cs
 *
 * 管路中的阶段性任务管理.
 *
 */

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace code.programs.pipelinetask
{
    public class PipellineStage
    {
        /*
         * @description: 异步读取目录路径下的文件，加入到并发集合中
         * @param {*}
         * @return {*}
         */
        public static Task ReadFileAsync(string path, BlockingCollection<string> output)
        {
            return Task.Factory.StartNew(() =>
            {
                foreach (string filename in Directory.EnumerateFiles(path, "*.cs", SearchOption.AllDirectories))
                {
                    output.Add(filename);
                    ColorConsole.WriteLine($"stage 1: added {filename}");
                }
                output.CompleteAdding();
            }, TaskCreationOptions.LongRunning);
        }

        /*
         * @description: 异步读取并发集合中的文件内容，并加入到并发集合
         * @param {*}
         * @return {*}
         */
        public static async Task LoadContentAsync(BlockingCollection<string> input, BlockingCollection<string> output)
        {
            // 从集合取出并返回元素，Quene.Dequeue()方法
            foreach (var filename in input.GetConsumingEnumerable())
            {
                using (FileStream stream = File.OpenRead(filename))
                {
                    var reader = new StreamReader(stream);
                    string line = null;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        output.Add(line);
                        ColorConsole.WriteLine($"stage 2: added {line}");
                        await Task.Delay(20);
                    }
                }
            }
            output.CompleteAdding();
        }

        /*
         * @description: 异步拆分文本，把拆分后的单词，加入并发字典中，设置自增value。
         * @param {*}
         * @return {*}
         */
        public static Task ProcessContentAsync(BlockingCollection<string> input, ConcurrentDictionary<string, int> output)
        {
            return Task.Factory.StartNew(() =>
            {
                foreach (var line in input.GetConsumingEnumerable())
                {
                    string[] words = line.Split(' ', ';', '\t', '{', '}', '(', ')', ':', ',', '"');
                    foreach (var word in words.Where(s => !string.IsNullOrEmpty(s)))
                    {
                        output.AddOrUpdate(key: word, addValue: 1, updateValueFactory: (s, i) => ++i);
                        ColorConsole.WriteLine($"stage 3: added {word}");
                    }
                }
            }, TaskCreationOptions.LongRunning);
        }

        /*
         * @description: 异步组装Info类型，并加入到并发集合中
         * @param {*}
         * @return {*}
         */
        public static Task TransferContentAsync(ConcurrentDictionary<string, int> input, BlockingCollection<Info> output)
        {
            return Task.Factory.StartNew(() =>
            {
                foreach (var word in input.Keys)
                {
                    int value;
                    if (input.TryGetValue(word, out value))
                    {
                        Info info = new Info { Word = word, Count = value };
                        output.Add(info);
                        ColorConsole.WriteLine($"stage 4: added {info}");
                    }
                }
                output.CompleteAdding();
            }, TaskCreationOptions.LongRunning);
        }

        /*
         * @description: 异步修改Info集合的元素属性
         * @param {*}
         * @return {*}
         */
        public static Task AddColorAsync(BlockingCollection<Info> input, BlockingCollection<Info> output)
        {
            return Task.Factory.StartNew(() =>
            {
                foreach (var item in input.GetConsumingEnumerable())
                {
                    if (item.Count > 40)
                    {
                        item.Color = "Red";
                    }
                    else if (item.Count > 20)
                    {
                        item.Color = "Yellow";
                    }
                    else
                    {
                        item.Color = "Green";
                    }
                    output.Add(item);
                    ColorConsole.WriteLine($"stage 5: added color {item.Color} to {item}");
                }
                output.CompleteAdding();
            }, TaskCreationOptions.LongRunning);
        }

        /*
         * @description: 异步执行：用指定颜色输出集合内容。
         * @param {*}
         * @return {*}
         */
        public static Task ShowContentAsync(BlockingCollection<Info> input)
        {
            return Task.Factory.StartNew(() =>
            {
                foreach (var item in input.GetConsumingEnumerable())
                {
                    ColorConsole.WriteLine($"stage 6: {item}", item.Color);
                }
            }, TaskCreationOptions.LongRunning);
        }
    }
}