using System;
using System.Linq;
using System.Collections.Generic;

namespace code
{
    public class LinqSort
    {
        public static void Test()
        {
            "Linq 二分排序".WriteTemplate();
            int[] array = new int[10];
            Random random = new Random();
            for (int i = 0; i < 10; i++)
            {
                array[i] = random.Next(0, 100);
            }
            Console.Write("random array is :");
            foreach (var item in array)
            {
                Console.Write(item + ",");
            }

            Console.WriteLine();
            Console.Write("sorted array is :");
            var list = qSort(array.AsEnumerable());
            foreach (var item in list)
            {
                Console.Write(item + ",");
            }

            Console.WriteLine();
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
}