/*
 * @Author: KingWJC
 * @Date: 2021-11-18 09:11:14
 * @LastEditTime: 2021-11-24 11:20:50
 * @LastEditors: your name
 * @FilePath: \code\synch\SemaphoreSample.cs
 * @Description:  
 * 
 * Semaphore 信号量
 */
using System.Threading;
using System.Threading.Tasks;
using static System.Console;

namespace code
{
    public class SemaphoreSample
    {
        public void Test()
        {
            int taskCount = 6;
            int semaphoreCount = 3;
            var semaphore = new SemaphoreSlim(semaphoreCount, semaphoreCount);
            Task[] tasks = new Task[taskCount];
            for (int i = 0; i < taskCount; i++)
            {
                tasks[i] = Task.Run(() => TaskMain(semaphore));
            }

            Task.WaitAll(tasks);
            WriteLine("All tasks finished");
        }

        private void TaskMain(SemaphoreSlim semaphoreSlim)
        {
            bool isComplated = false;
            while (!isComplated)
            {
                if (semaphoreSlim.Wait(600))
                {
                    try
                    {
                        WriteLine($"Task {Task.CurrentId} locks the semaphore");
                        Task.Delay(2000).Wait();
                    }
                    finally
                    {
                        WriteLine($"Task {Task.CurrentId} released the semaphore");
                        semaphoreSlim.Release();
                        isComplated = true;
                    }
                }
                else
                {
                    WriteLine($"Timeout for task {Task.CurrentId}; wait again");
                }
            }
        }
    }
}