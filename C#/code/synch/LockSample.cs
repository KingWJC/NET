/*
 * @Author: KingWJC
 * @Date: 2021-11-23 17:58:34
 * @LastEditTime: 2021-11-23 18:36:08
 * @LastEditors: your name
 * @Description: 
 * @FilePath: \code\synch\LockSample.cs
 *
 * lock 和 Interlocked 用法
 */

using System.Threading.Tasks;
using System.Threading;

namespace code
{
    public class LockSample
    {
        public void Test()
        {
            int taskNum = 20;
            var tasks = new Task[taskNum];
            ShareState shareState = new ShareState();
            for (int i = 0; i < taskNum; i++)
            {
                tasks[i] = Task.Run(() => new Job(shareState).DoJob());
            }

            Task.WaitAll(tasks);

            $"summarized {shareState.State}".WriteTemplate();
        }

        class ShareState
        {
            // 若是将SyncRoot模式用在get和set的存取器上，还是会产生争用条件，
            // 因为State的赋值不是原子的操作。
            public int State { get; set; }

            private int _state;
            public int State1
            {
                get
                {
                    // 是原子性的操作，比lock更快，使用CAS无锁。
                    // 内部调用CompareExchange<SomeState>(ref somestate,new state,null)
                    return Interlocked.Increment(ref _state);
                }
            }
        }

        class Job
        {
            private ShareState _shareState;
            public Job(ShareState shareState)
            {
                _shareState = shareState;
            }

            public void DoJob()
            {
                for (int j = 0; j < 50000; j++)
                {
                    lock (_shareState)
                    {
                        _shareState.State += 1;
                    }
                }
            }
        }
    }
}