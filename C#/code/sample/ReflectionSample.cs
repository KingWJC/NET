/*
 * @Author: KingWJC
 * @Date: 2021-09-06 11:04:40
 * @LastEditors: KingWJC
 * @LastEditTime: 2021-09-06 18:06:25
 * @Descripttion: 
 * @FilePath: \code\sample\ReflectionSample.cs
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
            const string libName = "code";
            const string typename = "VectorClass";

#if NET46
            Assembly assemblyCode = Assembly.LoadFile(libPath);
            Object newObj = assemblyCode.CreateInstance(typename);
            WriteLine(newObj);
#else
            // .net core
            Assembly assemblyCode = AssemblyLoadContext.Default.LoadFromAssemblyPath(libPath);
            Type typeVC = assemblyCode.GetType(typename);
            Object newObj = Activator.CreateInstance(type);
            WriteLine((newObj as VectorClass).ToString());
#endif
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