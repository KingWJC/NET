/*
 * @Author: KingWJC
 * @Date: 2021-11-18 09:11:14
 * @LastEditTime: 2021-11-24 14:53:17
 * @LastEditors: your name
 * @FilePath: \code\synch\CoundDownSample.cs
 * @Description:  
 * 
 * Events类，CountDownEvent
 */

using System;
using System.Threading;
using System.Threading.Tasks;
using static System.Console;

namespace code
{
    public class CoundDownSample
    {
        public void Test()
        {
            const int taskCount = 4;
            var calculators = new Calculator[taskCount];
            CountdownEvent countdownEvent = new CountdownEvent(taskCount);

            for (int i = 0; i < taskCount; i++)
            {
                int i1 = i;
                calculators[i] = new Calculator(countdownEvent);
                // 内部类或Lambda中必须使用重新赋值的变量。否则会有数据不一致的问题。
                Task.Run(() => calculators[i1].Calculation(i1 + 1, i1 + 3));
            }

            countdownEvent.Wait();

            for (int i = 0; i < taskCount; i++)
            {

                WriteLine($"finish task for {i} result={calculators[i].Result}");
            }
        }

        class Calculator
        {
            private CountdownEvent countdown;

            public int Result { get; set; }

            public Calculator(CountdownEvent cde)
            {
                this.countdown = cde;
            }

            public void Calculation(int x, int y)
            {
                WriteLine($"Task {Task.CurrentId} starts calculation");
                Task.Delay(new Random().Next(3000)).Wait();
                Result = x + y;
                // signal the event-completed!
                WriteLine($"Task {Task.CurrentId} is ready");
                // 计数增1
                countdown.Signal();
            }
        }
    }
}