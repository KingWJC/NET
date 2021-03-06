/*
 * @Author: KingWJC
 * @Date: 2021-09-06 11:04:40
 * @LastEditors: KingWJC
 * @LastEditTime: 2021-09-26 15:58:31
 * @Descripttion: 
 * @FilePath: \code\sample\ReflectionSample.cs
 *
 * 反射：Type对象的类型有：
 * FieldInfo封装了关于字段的所有信息   （通过Tyep对象的GetFields或GetField方法）
 * PropertyInfo类型，封装了类型的属性信息；（通过Type对象的GetProperties或GetProperty方法）
 * ConstructorInfo类型，封装了类型的构造函数信息； （..........）
 * MethodInfo类型，封装了类型的方法信息；  (........)
 * MemberInfo类型，封装了类型的所有公共成员；（**就是我们上面说的GetMembers方法**）
 * EventInfo类型，封装了类型的事件信息；(.......)
 * ParameterInfo类型，封装了方法和构造函数的参数信息；(........)
 */

using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Loader;
using static System.Console;

// 应用在整个程序集的特性，可以放在源代码的任何位置，但需要用关键字assembly和module做前缀。
[assembly: code.sample.SupportAssembly]

namespace code.sample
{
    public class ReflectionSample
    {
        static StringBuilder OutputText = new StringBuilder();
        private static DateTime backDateTo = new DateTime(2015, 2, 1);
        public static void Test()
        {
            //类型的属性和成员
            Type type = typeof(double);
            AnalyzeType(type);
            WriteLine($"Analysis of type {type.Name}");
            WriteLine(OutputText.ToString());

            //程序集，类型，方法成员的特性
            Assembly assembly = Assembly.Load(new AssemblyName("code"));
            Attribute supportAssembly = assembly.GetCustomAttribute(typeof(SupportAssemblyAttribute));
            if (supportAssembly == null)
            {
                AddToOutput("the assembly does not support SuportAssemblyAttribute");
                return;
            }
            AddToOutput("Defined types: ");
            IEnumerable<Type> exportType = assembly.GetExportedTypes();
            foreach (var item in exportType)
            {
                DisplayType(item);
            }

            WriteLine($"What\'s New since {backDateTo:D}");
            WriteLine(OutputText.ToString());

            // 动态实例化类型
            const string libPath = @"E:\github\NET\C#\code\bin\Debug\netcoreapp3.1\code.dll";
            // const string libName = "code";
            const string typename = "code.sample.VectorClass";

#if NET46
            Assembly assemblyCode = Assembly.LoadFile(libPath);
            Object newObj = assemblyCode.CreateInstance(typename);
            WriteLine(newObj);
#else
            // .net core
            Assembly assemblyCode = AssemblyLoadContext.Default.LoadFromAssemblyPath(libPath);
            // typeName必须包含完整的命名空间，否则无效。
            Type typeVC = assemblyCode.GetType(typename);
#endif
            //获取构造函数带参数的实例，并调用方法
            object[] parameter = new object[] { 10d, 20d };
            Object newObj = Activator.CreateInstance(typeVC, parameter);
            object result = typeVC.GetMethod("Sum").Invoke(newObj, null);
            WriteLine($"Sum Result = {result}");

            // 或者使用构造函数生成，用动态类型调用
            ConstructorInfo ci = typeVC.GetConstructor(new Type[] { typeof(double), typeof(double) });
            dynamic vectorClass = ci.Invoke(parameter);
            double sumResult = vectorClass.Sum();
            WriteLine($"Sum Result ={sumResult}");
        }

        /*
         * @description: 分析类型的的属性和成员
         * @param {*}
         * @return {*}
         */
        public static void AnalyzeType(Type t)
        {
            TypeInfo typeInfo = t.GetTypeInfo();
            AddToOutput($"Type Name: {typeInfo.Name}");
            AddToOutput($"Full Name: {typeInfo.FullName}");
            AddToOutput($"Namespace: {typeInfo.Namespace}");

            Type tBase = typeInfo.BaseType;
            if (tBase != null)
            {
                AddToOutput($"Base Type: {tBase.Name}");
            }

            AddToOutput("\n publec members:");
            foreach (var member in t.GetMembers())
            {
#if NET46
                AddToOutput($"{member.DeclaringType} {member.MemberType} {member.Name}");
#else
                AddToOutput($"{member.DeclaringType} {member.Name}");
#endif
            }
        }

        /*
         * @description: 显示类型的名称和方法，以及特性。
         * @param {*}
         * @return {*}
         */
        public static void DisplayType(Type type)
        {
            if (!type.IsClass || type.Name != "VectorClass")
            {
                return;
            }

            AddToOutput($"Class Name: {type.Name}");

            IEnumerable<LastModifiedAttribute> attributes = type.GetTypeInfo().GetCustomAttributes().OfType<LastModifiedAttribute>();
            if (attributes.Count() == 0)
            {
                AddToOutput("");
            }
            else
            {
                foreach (var item in attributes)
                {
                    WriteAttribute(item);
                }
            }

            //Type.DeclaringMethod
            AddToOutput("chages to methods of thes class");
            foreach (var method in type.GetTypeInfo().DeclaredMembers.OfType<MethodInfo>())
            {
                IEnumerable<LastModifiedAttribute> methodAttribute = method.GetCustomAttributes().OfType<LastModifiedAttribute>();
                if (methodAttribute.Count() > 0)
                {
                    AddToOutput($"{method.ReturnType} {method.Name}()");
                    foreach (var item in methodAttribute)
                    {
                        WriteAttribute(item);
                    }
                }
            }
        }

        public static void WriteAttribute(Attribute attribute)
        {
            if (attribute == null)
                return;
            LastModifiedAttribute modifiedAttribute = attribute as LastModifiedAttribute;

            DateTime modifiedDate = modifiedAttribute.DataModified;
            if (modifiedDate < backDateTo)
            {
                return;
            }

            AddToOutput($"modified: {modifiedDate:D} {modifiedAttribute.Changes}");
            if (modifiedAttribute.Issues != null)
            {
                AddToOutput($"Outstading issues : {modifiedAttribute.Issues}");
            }
        }

        public static void AddToOutput(string text)
        {
            OutputText.Append($"\n{text}");
        }

    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class LastModifiedAttribute : Attribute
    {
        private readonly DateTime _dataModified;
        private readonly string _changes;

        public LastModifiedAttribute(string dataModified, string changes)
        {
            this._changes = changes;
            this._dataModified = DateTime.Parse(dataModified);
        }

        public DateTime DataModified => _dataModified;
        public string Changes => _changes;
        public string Issues { get; set; }
    }

    [AttributeUsage(AttributeTargets.Assembly)]
    public class SupportAssemblyAttribute : Attribute { }
}