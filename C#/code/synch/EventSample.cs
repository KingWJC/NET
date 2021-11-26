/*
 * @Author: KingWJC
 * @Date: 2021-11-18 09:11:14
 * @LastEditTime: 2021-11-24 14:29:19
 * @LastEditors: your name
 * @FilePath: \code\synch\EventSample.cs
 * @Description:  
 * 
 * Events类，ManualResetEventSlim
 */

using System;
using System.Threading;
using System.Threading.Tasks;
using static System.Console;

namespace code
{
    public class EventSample
    {
        public void Test()
        {
            const int taskCount = 4;
            var calculators = new Calculator[taskCount];
            var mEvents = new ManualResetEventSlim[taskCount];
            var waitHandles = new WaitHandle[taskCount];

            for (int i = 0; i < taskCount; i++)
            {
                int i1 = i;
                mEvents[i] = new ManualResetEventSlim(false);
                waitHandles[i] = mEvents[i].WaitHandle;
                calculators[i] = new Calculator(mEvents[i]);
                // 内部类或Lambda中必须使用重新赋值的变量。否则会有数据不一致的问题。
                Task.Run(() => calculators[i1].Calculation(i1 + 1, i1 + 3));
            }

            for (int i = 0; i < taskCount; i++)
            {
                //   int index = WaitHandle.WaitAny(mEvents.Select(e => e.WaitHandle).ToArray());
                int index = WaitHandle.WaitAny(waitHandles);
                if (index == WaitHandle.WaitTimeout)
                {
                    WriteLine("timeout");
                }
                else
                {
                    mEvents[index].Reset();
                    WriteLine($"finish task for {index} result={calculators[index].Result}");
                }
            }
        }

        class Calculator
        {
            private ManualResetEventSlim resetEventSlim;

            public int Result { get; set; }

            public Calculator(ManualResetEventSlim eventSlim)
            {
                this.resetEventSlim = eventSlim;
            }

            public void Calculation(int x, int y)
            {
                WriteLine($"Task {Task.CurrentId} starts calculation");
                Task.Delay(new Random().Next(3000)).Wait();
                Result = x + y;
                // signal the event-completed!
                WriteLine($"Task {Task.CurrentId} is ready");
                // 向WaitAny事件发信号。
                resetEventSlim.Set();
            }
        }
    }
}