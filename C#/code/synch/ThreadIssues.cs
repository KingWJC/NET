/*
 * @Author: KingWJC
 * @Date: 2021-11-18 09:11:14
 * @LastEditTime: 2021-11-24 10:00:16
 * @LastEditors: your name
 * @FilePath: \code\synch\ThreadIssues.cs
 * @Description:  
 * 
 * 线程问题：争用条件和死锁
 */
using System.Diagnostics;
using System.Threading.Tasks;
using static System.Console;

namespace code
{
    public class ThreadIssues
    {
        // 错误：Process terminated. 
        // Assertion Failed
        // Racce condition occurred afer 22432 loops
        public void RaceCondition()
        {
            StateObject state = new StateObject();
            for (int i = 0; i < 2; i++)
            {
                int index = i;
                Task.Factory.StartNew(() =>
                {
                    WriteLine($"task {index} start");
                    int i = 0;
                    while (true)
                    {
                        state.ChangeState(i++);
                    }
                });
            }
        }

        public void DeadLock()
        {
            StateObject s1 = new StateObject();
            StateObject s2 = new StateObject();

            Task.Run(() =>
            {
                int i = 0;
                while (true)
                {
                    lock (s1)
                    {
                        WriteLine("task 1 - waiting for s1");
                        lock (s2)
                        {
                            WriteLine("task 1 - s1 and s2 locked, now go on...");
                            s1.ChangeState(i);
                            s2.ChangeState(i++);
                            WriteLine($"task 1 still running, i: {i}");
                        }
                    }
                }
            });

            Task.Run(() =>
            {
                int i = 0;
                while (true)
                {
                    lock (s2)
                    {
                        WriteLine("task 2 - waiting for s2");
                        lock (s1)
                        {
                            WriteLine("task 2 - s1 and s2 locked, now go on...");
                            s1.ChangeState(i);
                            s2.ChangeState(i++);
                            WriteLine($"task 2 still running, i: {i}");
                        }
                    }
                }
            });

            Task.Delay(1000).Wait();
        }

        class StateObject
        {
            private int _state = 5;
            private object obj = new object();

            public void ChangeState(int loop)
            {
                // lock (obj)
                // {
                    if (_state == 5)
                    {
                        _state++;
                        WriteLine(_state);
                        Trace.Assert(_state == 6, $"Racce condition occurred afer {loop} loops");
                    }
                    _state = 5;
                // }
            }
        }
    }
}