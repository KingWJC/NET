using System;
using code.sample;
using static System.Console;

namespace code
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Hello World!");

            /* 样例
            ObjectsAndTypes.Test();

            Inheritance.Test();

            DisposableSample.Test();

            PInvoke.Test();

            Pointer.Test();

            Pointer.QuickArray();

            Generics.Test();

            OperatorsAndCast.Test();

            DelegateSample.Test();

            StringAndRegularEx.Test();

            CollectionsSample.Test();

            CollectionSepcial.Test();

            LinqSample.Test();

            Exceptions.Test();

            AsyncSample.Test();

            ReflectionSample.Test();

            DynamicSample.Test();

            ParallelSample.Test ();

            TaskSample.Test();

            DataFlowSample.Test ();
            */

            /* 小程序
            Observer.Test();

            code.programs.documentconsume.Client.Test();

            code.programs.documentpriority.Client.Test();

            code.programs.pipelinetask.Clent.Test();

            code.programs.solicitcoldcall.Client.Test();
            */

            /* 知识点
            GenericsConvariance.Test();

            DelegateRef.Test();

            LinqSort.Test();

            Deadlock.Test();
            */

            /* 同步锁
            new ThreadIssues().RaceCondition();
            new ThreadIssues().DeadLock();
            new LockSample().Test();
            new WaitHandleSample().Test();
            new SemaphoreSample().Test();
            new EventSample().Test();
            new CoundDownSample().Test();
            new BarrierSample().Test();
            ReadLine();*/

            // new ThreadSample().TestAbort();
            new ThreadSample().TestInterrupt();
        }
    }

    static class StringExtension
    {
        public static void WriteTemplate(this string s)
        {
            WriteLine();
            WriteLine($"===================={s}==================");
        }

        public static string FirstName(this string name)
        {
            int index = name.IndexOf(" ");
            return name.Substring(0, index);
        }

        public static string LastName(this string name)
        {
            int index = name.IndexOf(" ");
            return name.Substring(index + 1);
        }

        public static void Wastetime(this DateTime now, DateTime past)
        {
            TimeSpan timeSpan = now.Subtract(past);
            WriteLine($"Elapsed time : {timeSpan }, in seconds : {timeSpan.TotalSeconds}");
        }
    }
}