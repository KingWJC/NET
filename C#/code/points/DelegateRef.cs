/*
 * @Author: KingWJC
 * @Date: 2021-08-11 14:22:11
 * @LastEditors: KingWJC
 * @LastEditTime: 2021-08-19 13:56:50
 * @Descripttion: 
 * @FilePath: \code\DelegateRef.cs
 *
 *
 * 委托引用普通方法和静态方法相比
 * 当委托进行引用时，System.Delegate 类的Target属性指向的对象不同，
 * 实例方法：指向实例方法所在的实例，有可能引发内存泄漏。
 * 静态方法：指向为null.所以将静态方法赋值给委托时性能更优。
 * 
 */
using System;

namespace code {
    public class DelegateRef {
        public delegate void referenceDelegate ();
        public static void Test () {
            "Delegate Method Invoked".WriteTemplate ();
            Target target = new Target ();
            //实例方法
            referenceDelegate instanceMethod = target.instanceMethod;
            instanceMethod ();
            Console.WriteLine (instanceMethod.Target == target);
            //静态方法
            referenceDelegate staticMethod = Target.staticMethod;
            staticMethod.Invoke ();
            Console.WriteLine (staticMethod.Target == null);
        }
    }
    class Target {
        public static void staticMethod () {
            Console.Write ("this is a static method. Target of delegate is null : ");
        }

        public void instanceMethod () {
            Console.Write ("this is a instance method. Target of delegate is target : ");
        }
    }
}