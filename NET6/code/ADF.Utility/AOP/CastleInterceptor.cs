using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADF.Utility
{
    /// <summary>
    /// 织入的拦截器
    /// </summary>
    internal class CastleInterceptor : StandardInterceptor
    {
        private List<BaseAOPAttribute> _attributes;

        private void Init(IInvocation invocation)
        {
            _attributes = GetMethodAops(invocation);
        }
        protected override void PreProceed(IInvocation invocation)
        {
            Init(invocation);
            foreach (var aop in _attributes)
            {
                aop.Before(invocation);
            }
        }

        protected override void PostProceed(IInvocation invocation)
        {
            foreach (var aop in _attributes.ToArray().Reverse())
            {
                aop.After(invocation);
            }
        }

        protected override void PerformProceed(IInvocation invocation)
        {
            Action action = () => base.PerformProceed(invocation);
            foreach (var aop in _attributes)
            {
                action = aop.Do(invocation, action);
            }

            action.Invoke();
        }

        /// <summary>
        /// 获取执行方法的所有aop特性(在接口方法和实现类的特性中找）
        /// </summary>
        /// <param name="invocation"></param>
        /// <returns></returns>
        private List<BaseAOPAttribute> GetMethodAops(IInvocation invocation)
        {
            return invocation.MethodInvocationTarget.GetCustomAttributes(typeof(BaseAOPAttribute), true)
                .Concat(invocation.InvocationTarget.GetType().GetCustomAttributes(typeof(BaseAOPAttribute), true))
                .Select(a => (BaseAOPAttribute)a).ToList();
        }
    }
}
