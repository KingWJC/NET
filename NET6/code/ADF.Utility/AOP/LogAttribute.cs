using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADF.Utility
{
    /// <summary>
    /// 日志切面,把切面逻辑转移到特性中
    /// </summary>
    public class LogBeforeAttribute : BaseAOPAttribute
    {
        public override Action Do(IInvocation invocation, Action action)
        {
            return () =>
            {
                Console.WriteLine($"This is LogBeforeAttribute1 {invocation.Method.Name} {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")}");

                action.Invoke();

                Console.WriteLine($"This is LogBeforeAttribute2 {invocation.Method.Name} {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")}");
            };
        }
    }

    public class LogAfterAttribute : BaseAOPAttribute
    {
        public override Action Do(IInvocation invocation, Action action)
        {
            return () =>
            {
                Console.WriteLine($"This is LogAfterAttribute1 {invocation.Method.Name} {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")}");

                action.Invoke();

                Console.WriteLine($"This is LogAfterAttribute2 {invocation.Method.Name} {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")}");
            };
        }
    }
}
