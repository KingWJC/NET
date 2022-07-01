using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADF.Utility
{
    public class MonitorAttribute : BaseAOPAttribute
    {
        private string _operateStr;
        private Stopwatch stopwatch;

        public MonitorAttribute(string operate)
        {
            _operateStr = operate;
        }

        public override void Before(IInvocation invocation)
        {
            stopwatch = new Stopwatch();
            Console.WriteLine($"This is MonitorAttribute1 {invocation.Method.Name} {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")}");
            stopwatch.Start();
        }

        public override void After(IInvocation invocation)
        {
            stopwatch.Stop();
            Console.WriteLine($"本次方法花费时间{stopwatch.ElapsedMilliseconds}ms");
            Console.WriteLine($"This is MonitorAttribute2 {invocation.Method.Name} {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")}");
        }
    }
}
