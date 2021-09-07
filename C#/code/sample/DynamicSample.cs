/*
 * @Author: KingWJC
 * @Date: 2021-09-07 11:18:42
 * @LastEditors: KingWJC
 * @LastEditTime: 2021-09-07 14:55:02
 * @Descripttion: 
 * @FilePath: \code\sample\DynamicSample.cs
 */
using System;
using System.Dynamic;
using System.Collections.Generic;
using Microsoft.Scripting.Hosting;
using static System.Console;

namespace code.sample
{
    public class DynamicSample
    {
        public static void Test()
        {
            dynamic dynObj;

            WriteLine("First Set : ");
            dynObj = 100;
            WriteLine(dynObj.GetType());
            WriteLine(dynObj);

            WriteLine("\nSecond Set : ");
            dynObj = new Person { Name = "Bugs" };
            WriteLine(dynObj.GetType());
            WriteLine(dynObj.Name);

            WriteLine("\nThird Set : ");
            dynObj = new WroxDynamicOject();
            dynObj.FirstName = "Bugs";
            dynObj.LastName = "Bunny";
            dynObj.Friends = new List<Person> { new Person { Name = "Daffy" }, new Person { Name = "Porky" } };
            Func<DateTime, string> GetTomorrow = today => today.AddDays(1).ToString("d");
            dynObj.GetTomorrowDate = GetTomorrow;
            WriteLine(dynObj.GetType());
            WriteLine($"{dynObj.FirstName} {dynObj.LastName}");
            foreach (Person friend in dynObj.Friends)
            {
                WriteLine($"{friend.Name}");
            }
            WriteLine($"Tomorrow is {dynObj.GetTomorrowDate(DateTime.Now)}");

            WriteLine("\nFour Set : ");
            // 可直接使用的密封类
            dynamic expObj = new ExpandoObject();
            expObj.FirstName = "Daffy";
            expObj.LastName = "Duck";
            expObj.GetTomorrowDate = GetTomorrow;
            expObj.Friends = new List<Person> { new Person { Name = "Daffy" }, new Person { Name = "Porky" } };
            WriteLine(expObj.GetType());
            WriteLine($"{expObj.FirstName} {expObj.LastName}");
            foreach (Person friend in expObj.Friends)
            {
                WriteLine($"{friend.Name}");
            }
            WriteLine($"Tomorrow is {expObj.GetTomorrowDate(DateTime.Now)}");
        }

        /*
         * @description: 动态语言运行时，执行Python脚本。
         * @param {*}
         * @return {*}
         */
        public static void DLRSTest()
        {
            ScriptRuntime runtime = ScriptRuntime.CreateFromConfiguration();
            ScriptEngine engine = runtime.GetEngine("python");
            ScriptSource source = engine.CreateScriptSourceFromFile("resource/CountDisc.py");
            ScriptScope scope = engine.CreateScope();
            scope.SetVariable("prodCount", 20);
            scope.SetVariable("amt", 0.3d);
            Console.WriteLine(scope.GetVariable("retAmt"));

            //直接获取Scope
            dynamic calcRate = runtime.UseFile("resource/CountDisc.py");
            Console.WriteLine(calcRate.CalcTax(10));
        }
    }

    /*
     * @description: 自定义动态类型，必须重写方法。
     * @param {*}
     * @return {*}
     */
    public class WroxDynamicOject : DynamicObject
    {
        private Dictionary<string, object> _dyanmicData = new Dictionary<string, object>();

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            bool success = false;
            result = null;
            if (_dyanmicData.ContainsKey(binder.Name))
            {
                result = _dyanmicData[binder.Name];
                success = true;
            }
            else
            {
                result = "property not found!";
            }
            return success;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            _dyanmicData[binder.Name] = value;
            return true;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            dynamic method = _dyanmicData[binder.Name];
            // 参数类型和方法的参数，必须一致，且需要转换。
            result = method((DateTime)args[0]);
            return result != null;
        }
    }
}