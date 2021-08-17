using System;
using System.Collections.Generic;

namespace code
{
    public class DelegateSample
    {
        public delegate void ActionDelegate(string message);
        public delegate String FuncDelegate();

        public static void Test()
        {
            "委托的声明".WriteTemplate();
            string mid = ", middle part,";
            // 委托函数
            Action<IList<Employee>, Func<Employee, Employee, bool>> compare = BubbleSort;
            // 委托实例
            var deleAct = new ActionDelegate(message => Console.WriteLine(message));
            var deleFuc = new FuncDelegate(mid.ToString);
            // 匿名委托
            Func<string, string> anonDel = delegate (string param)
            {
                param += mid;
                param += " and this was added to the string.";
                return param;
            };
            // lambda
            FuncDelegate func = () => mid.ToString();

            Console.WriteLine($"String is {anonDel("Start of string")}");
            // 相当于:
            // Console.WriteLine("String is {0}", anonDel("Start of string"));

            "委托的使用".WriteTemplate();
            Employee[] employees =
            {
                new Employee("Bugs Bunny", 20000),
                new Employee("Elmer Fudd", 10000),
                new Employee("Daffy Duck", 25000),
                new Employee("Wile Coyote", 1000000.38m),
                new Employee("Foghorn Leghorn", 23000),
                new Employee("RoadRunner", 50000)
            };

            BubbleSort(employees, Employee.CompareSalary);
            foreach (var employee in employees)
            {
                Console.WriteLine(employee);
            }
        }

        /* 冒泡排序 */
        static void BubbleSort<T>(IList<T> sortArray, Func<T, T, bool> comparison)
        {
            for (int i = 0; i < sortArray.Count; i++)
            {
                for (int j = i; j < sortArray.Count - 1; j++)
                {
                    if (comparison(sortArray[j + 1], sortArray[j]))
                    {
                        T t = sortArray[j];
                        sortArray[j] = sortArray[j + 1];
                        sortArray[j + 1] = t;
                    }
                }
            }

            bool swapped = true;
            do
            {
                swapped = false;
                for (int i = 0; i < sortArray.Count - 1; i++)
                {
                    if (comparison(sortArray[i + 1], sortArray[i]))
                    {
                        T temp = sortArray[i];
                        sortArray[i] = sortArray[i + 1];
                        sortArray[i + 1] = temp;
                        swapped = true;
                    }
                }
            } while (swapped);
        }
    }

    class Employee
    {
        public string Name { get; }
        public decimal Salary { get; }
        public Employee(string name, decimal salary)
        {
            Name = name;
            Salary = salary;
        }
        public override string ToString() => $"Name: {Name}, Salary: {Salary}";
        public static bool CompareSalary(Employee e1, Employee e2) => e1.Salary < e2.Salary;
    }
}