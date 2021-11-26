/*
 * @Author: KingWJC
 * @Date: 2021-11-18 09:11:14
 * @LastEditTime: 2021-11-25 10:35:21
 * @LastEditors: your name
 * @FilePath: \code\synch\BarrierSample.cs
 * @Description:  
 * 
 * Barrier, 控制多任务，分步执行。
 * 示例：一个有5个元素的数组，每个元素是一个包含2000000个字符串的集合， 
 *       使用两个任务遍历数组，统计以a,b,c...等开头的字符的个数
 * 方案：使用两个任务遍历数组的集合，一个线程负责遍历0-1000000，第二个线程遍历1000000-2000000
 *       分为5个阶段执行，并将每个阶段的两个任务的结果进行计算并打印
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Console;

namespace code
{
    public class BarrierSample
    {
        public void Test()
        {
            const int numberTasks = 2;
            const int partitionSize = 1000000;
            const int loops = 5;

            var taskResults = new Dictionary<int, int[][]>();
            var data = new List<string>[loops];
            for (int i = 0; i < loops; i++)
            {
                data[i] = new List<string>(FillData(partitionSize * numberTasks));
            }

            var barrier = new Barrier(1);
            LogBarrierInfo("initial participants in barrier", barrier);
            for (int i = 0; i < numberTasks; i++)
            {
                barrier.AddParticipant();
                int jobNumber = i;
                taskResults.Add(i, new int[loops][]);
                for (int loop = 0; loop < loops; loop++)
                {
                    taskResults[i][loop] = new int[26];
                }
                WriteLine($"Main - starting task job {jobNumber}");
                // lambda 表达式中必须使用重新赋值的变量，保证数据一致性。
                Task.Run(() => CalculationInTask(jobNumber, partitionSize, barrier, data, loops, taskResults[jobNumber]));
            }

            for (int loop = 0; loop < 5; loop++)
            {
                LogBarrierInfo("main task, start signaling and wait", barrier);
                barrier.SignalAndWait();
                LogBarrierInfo("main task waiting completed", barrier);
                int[][] resultCollection1 = taskResults[0];
                int[][] resultCollection2 = taskResults[1];
                var resultCollection = resultCollection1[loop].Zip(resultCollection2[loop], (c1, c2) => c1 + c2);

                char ch = 'a';
                int sum = 0;
                foreach (var item in resultCollection)
                {
                    WriteLine($"{ch++},count:{item}");
                    sum += item;
                }
            }

            WriteLine("at the end");
        }

        private void CalculationInTask(int jobNumber, int partitionSize, Barrier barrier, IList<string>[] coll, int loops, int[][] results)
        {
            LogBarrierInfo("CalculationInTask started", barrier);
            for (int i = 0; i < loops; i++)
            {
                var data = new List<string>(coll[i]);
                int start = jobNumber * partitionSize;
                int end = start + partitionSize;
                WriteLine($"Task {Task.CurrentId} in loop {i} : partition from {start} to {end}");

                for (int j = start; j < end; j++)
                {
                    char c = data[j][0];
                    results[i][c - 97]++;
                }

                WriteLine($"Calculation completed from task {Task.CurrentId}" +
                $"in loop {i}.{results[i][0]} times a, {results[i][25]} times z");

                LogBarrierInfo("sending siginal and wait for all", barrier);
                barrier.SignalAndWait();
                LogBarrierInfo("waiting completed", barrier);
            }

            barrier.RemoveParticipant();
            LogBarrierInfo("finished task, removed participant", barrier);
        }

        private IEnumerable<string> FillData(int size)
        {
            var r = new Random();
            return Enumerable.Range(0, size).Select(x => GetString(r));
        }

        // 生成小写字母的6位字符串
        private string GetString(Random random)
        {
            var sb = new StringBuilder(6);
            for (int i = 0; i < 6; i++)
            {
                sb.Append((char)(random.Next(26) + 97));
            }
            return sb.ToString();
        }

        private void LogBarrierInfo(string info, Barrier barrier)
        {
            WriteLine($"Task {Task.CurrentId}:{info}." +
            $" {barrier.ParticipantCount} current and" +
            $" {barrier.ParticipantsRemaining} remaining participants, " +
            $"phase {barrier.CurrentPhaseNumber}");
        }
    }
}