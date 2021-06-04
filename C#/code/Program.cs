using System;
using System.Collections.Generic;
using System.Linq;


namespace code
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            StaticDemo<int>.x = 10;
            StaticDemo<string>.x = 20;
            Console.WriteLine(StaticDemo<int>.x);

            IIndex<TestExtandMethod> temp = new RecCollection();

            int[] array = new int[10];
            Random random = new Random();
            for (int i = 0; i < 10; i++)
            {
                array[i] = random.Next(0, 100);
            }
            foreach (var item in array)
            {
                Console.Write(item + ",");
            }

            Console.WriteLine();
            var list = qSort(array.AsEnumerable());
            foreach (var item in list)
            {
                Console.Write(item + ",");
            }

            Person person = new Person();
            person.Name = "wjc";
            Console.WriteLine(person.Name);

            //扩展方法可继承
            TestExtandMethodA a = new TestExtandMethodA();
            a.ping();
            TestExtandMethod b = new TestExtandMethodA();
            b.ping();
        }

        private static IEnumerable<T> qSort<T>(IEnumerable<T> list) where T : IComparable<T>
        {
            if (list.Count() <= 1) return list;

            var pivot = list.First();
            //Stack overflow. 剩下两个元素，无法排序，进入死循环。
            var left = qSort(list.Where(x => x.CompareTo(pivot) < 0));
            var right = qSort(list.Where(x => x.CompareTo(pivot) > 0));
            var middle = list.Where(x => x.CompareTo(pivot) == 0);

            return left.Concat(middle).Concat(right);
        }
    }

    public class StaticDemo<T>
    {
        public static int x;
    }

    public interface IIndex<out T>
    {
        T this[int index] { get; }
    }

    public class RecCollection : IIndex<TestExtandMethodA>
    {
        public TestExtandMethodA this[int index]
        {
            get
            {
                return new TestExtandMethodA();
            }
        }
    }

    public class Person : IBehaves
    {
        private string name;
        public string Name
        {
            get { return name; }
            set { this.name = value; }
        }
        public int compute(int a, int b)
        {
            return a + b;
        }

        public override string ToString()
        {
            return name;
        }
    }

    public interface IBehaves
    {
        int compute(int a, int b);
    }

    public abstract class TestExtandMethod
    {
        string name;
        public string Name => name;

        //抽象类不适合包含公共的非静态方法，因为不会被实例化，永远都不会调用。
        public override string ToString()
        {
            return "父类" + name;
        }
    }

    public class TestExtandMethodA : TestExtandMethod
    {
        public override string ToString()
        {
            return "子类" + Name;
        }
    }

    static class ExtendHelper
    {
        public static void ping(this TestExtandMethod test)
        {
            Console.WriteLine(test.ToString());
        }
    }
}