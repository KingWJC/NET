using System;
using System.Collections.Generic;

namespace code {
    public class GenericsConvariance {
        public static void Test () {
            Student wjc = new Student { Name = "wjc", Grade = 1 };
            Student wxy = new Student { Name = "wxy", Grade = 3 };
            Teacher yl = new Teacher { Name = "ln", Level = 5 };

            // 调用了 Student 里定义的 Compare() 函数
            Console.WriteLine (wjc.IsGreateThan (wxy));

            /* 由于 Student 类实现了 IMyComparable<Student> 和 IMyComparable<Person> 接口，
               所以编译器不知道第一个参数 own 是 IMyComparable<Person> 类型 还是 IMyComparable<Student>
               于是编译器放弃第一个参数，根据第二个参数 rhs 作出的类型推断
               bool IsGreateThan<Teacher>(this IMyComparable<Teacher> lhs, Teacher rhs);
            */
            Console.WriteLine (wjc.IsGreateThan (yl));
            // 相当于: 
            IMyComparable<Person> person = wjc;
            IMyComparable<Teacher> teacher = person;
            bool flag = CompareExtension.IsGreateThan<Teacher> (teacher, yl);
            Console.WriteLine ($"另一种方式: {flag}");

            // 不用in逆变的两种方式: 调用了 Person 里定义的 Compare() 函数, wjc Student类型可转为 Person.
            Console.WriteLine (yl.IsGreateThan (wjc));
            flag = CompareExtension.IsGreateThan<Person> (wjc, yl);
        }
    }

    interface IMyComparable<in T> {
        int CompareTo (T obj);
    }

    static class CompareExtension {
        public static bool IsGreateThan<T> (this IMyComparable<T> own, T other) {
            return own.CompareTo (other) > 0;
        }

        public static bool IsLessThan<T> (this IMyComparable<T> own, T other) {
            return own.CompareTo (other) < 0;
        }
    }

    class Person : IMyComparable<Person> {
        public string Name { get; set; }
        public int CompareTo (Person person) {
            return Name.CompareTo (person.Name);
        }
    }

    class Teacher : Person {
        public int Level { get; set; }
    }

    class Student : Person, IMyComparable<Student> {
        public int Grade { get; set; }
        public int CompareTo (Student sutdent) {
            if (Name == sutdent.Name) {
                return Grade.CompareTo (sutdent.Grade);
            }
            return Name.CompareTo (sutdent.Name);
        }
    }

    /* 
     * 复合情况
     * 类型参数T，定义为协变后，只能做返回值。
     * 无法适用于不可变的类型参数，如IList<T>
     * 会被其它接口的定义所覆盖。
     */
    public interface ICovariantDemo<out T> {
        //变型无效: 类型参数“T”必须是在“ICovariantDemo<T>.SetAnIList(IList<T>)”上有效的 固定式。“T”为 协变
        // void SetAnIList(IList<T> list); // 编译错误
        // IList<T> GetAnIList(); // 编译错误

        //变型无效: 类型参数“T”必须是在“ICovariantDemo<T>.SetAnItem(T)”上有效的 逆变式。“T”为 协变。
        // void SetAnItem(T v); // 编译错误
        // IContravarianceDemo<T> GetACotraInterface(); // 编译错误
        // void SetACoInterface(ICovariantDemo<T> a); // 编译错误
        // void GetAnItemFromDelegate(Func<T> f); // 编译错误
        // Action<T> DoSthLater(); // 编译错误

        // 正确的使用
        T GetAnItem ();
        ICovariantDemo<T> GetACoInterface ();
        void SetAContraInterface (IContravarianceDemo<T> a);
        Func<T> GetAnItemLater ();
        void DoSth (Action<T> a);
    }

    public interface IContravarianceDemo<in T> {

    }
}