using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADF.Utility
{
    public abstract class BaseAOPAttribute : Attribute
    {
        public virtual void Before(IInvocation invocation)
        {

        }

        public virtual void After(IInvocation invocation)
        {

        }

        public virtual Action Do(IInvocation invocation, Action action)
        {
            return () => action.Invoke();
        }
    }
}
