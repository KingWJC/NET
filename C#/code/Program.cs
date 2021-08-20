using System;
using code.sample;
using code.programs;
using static System.Console;
using System.IO;

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
            */

            /* 小程序
            Observer.Test();

            code.programs.documentconsume.Client.Test();

            code.programs.documentpriority.Client.Test();
            */

            /* 知识点
            GenericsConvariance.Test();

            DelegateRef.Test();

            LinqSort.Test();

            Deadlock.Test();
            */

            code.programs.pipelinetask.Clent.Test();
        }
    }

    static class StringExtension
    {
        public static void WriteTemplate(this string s)
        {
            WriteLine();
            WriteLine($"===================={s}==================");
        }
    }
}