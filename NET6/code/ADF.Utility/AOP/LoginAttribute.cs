using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADF.Utility
{
    public class LoginAttribute : BaseAOPAttribute
    {
        public override void Before(IInvocation invocation)
        {
            Console.WriteLine($"This is LoginAttribute1 {invocation.Method.Name} {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")}");
        }

        public override void After(IInvocation invocation)
        {
            Console.WriteLine($"This is LoginAttribute2 {invocation.Method.Name} {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")}");
        }
    }
}
