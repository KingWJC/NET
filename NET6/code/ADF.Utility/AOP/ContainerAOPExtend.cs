using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADF.Utility
{
    /// <summary>
    /// IOC容器的AOP扩展
    /// </summary>
    public static class ContainerAOPExtend
    {
        /// <summary>
        /// 根据指定对象和其抽象接口生成基于抽象接口代理对象
        /// </summary>
        /// <param name="t">对象</param>
        /// <param name="interfaceType">抽象接口</param>
        /// <returns></returns>
        public static object GetProxy(this object t, Type interfaceType)
        {
            ProxyGenerator generator = new ProxyGenerator();
            CastleInterceptor interceptor = new CastleInterceptor();
            return generator.CreateInterfaceProxyWithTarget(interfaceType, t, interceptor);
        }
    }
}
