using System;

namespace code
{
    public class GenericsConvariance
    {
        public static void Test()
        {
            Student wjc = new Student { Name = "wjc", Grade = 1 };
            Student wxy = new Student { Name = "wxy", Grade = 3 };
            Teacher yl = new Teacher { Name = "ln", Level = 5 };

            // 调用了 Student 里定义的 Compare() 函数
            Console.WriteLine(wjc.IsGreateThan(wxy));

            /* 由于 Student 类实现了 IMyComparable<Student> 和 IMyComparable<Person> 接口，
               所以编译器不知道第一个参数 own 是 IMyComparable<Person> 类型 还是 IMyComparable<Student>
               于是编译器放弃第一个参数，根据第二个参数 rhs 作出的类型推断
               bool IsGreateThan<Teacher>(this IMyComparable<Teacher> lhs, Teacher rhs);
            */
            Console.WriteLine(wjc.IsGreateThan(yl));
            // 相当于: 
            IMyComparable<Person> person = wjc;
            IMyComparable<Teacher> teacher = person;
            bool flag = CompareExtension.IsGreateThan<Teacher>(teacher, yl);
            Console.WriteLine($"另一种方式: {flag}");

            // 不用in逆变的两种方式: 调用了 Person 里定义的 Compare() 函数, wjc Student类型可转为 Person.
            Console.WriteLine(yl.IsGreateThan(wjc));
            flag = CompareExtension.IsGreateThan<Person>(wjc, yl);
        }
    }

    interface IMyComparable<in T>
    {
        int CompareTo(T obj);
    }

    static class CompareExtension
    {
        public static bool IsGreateThan<T>(this IMyComparable<T> own, T other)
        {
            return own.CompareTo(other) > 0;
        }

        public static bool IsLessThan<T>(this IMyComparable<T> own, T other)
        {
            return own.CompareTo(other) < 0;
        }
    }

    class Person : IMyComparable<Person>
    {
        public string Name { get; set; }
        public int CompareTo(Person person)
        {
            return Name.CompareTo(person.Name);
        }
    }

    class Teacher : Person
    {
        public int Level { get; set; }
    }

    class Student : Person, IMyComparable<Student>
    {
        public int Grade { get; set; }
        public int CompareTo(Student sutdent)
        {
            if (Name == sutdent.Name)
            {
                return Grade.CompareTo(sutdent.Grade);
            }
            return Name.CompareTo(sutdent.Name);
        }
    }
}