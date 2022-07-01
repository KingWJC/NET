using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Linq;
using System.Threading;

namespace ADF.Utility
{
    /// <summary>
    /// IOC容器, (因为对象在这创建  所以可以进行对象生命周期管理）
    /// </summary>
    public class ObjectContainer : IObjectContainer
    {
        /// <summary>
        /// 抽象与实现的映射字典
        /// </summary>
        private Dictionary<string, ObjectModel> objectMappingDict = new Dictionary<string, ObjectModel>();

        /// <summary>
        /// 构造函数初始化的固定参数字典
        /// </summary>
        private Dictionary<string, object[]> constractParasDict = new Dictionary<string, object[]>();
         
        /// <summary>
        /// 作用域下的创建的实例字典
        /// </summary>
        private Dictionary<string, object> scopeInstanceDict = new Dictionary<string, object>();

        public ObjectContainer() { }

        public ObjectContainer(Dictionary<string, ObjectModel> mappings, Dictionary<string, object[]> paras, Dictionary<string, object> scopeInstances)
        {
            this.objectMappingDict = mappings;
            this.constractParasDict = paras;
            this.scopeInstanceDict = scopeInstances;
        }

        /// <summary>
        /// 获取映射字典的主键，类型全称加别名
        /// </summary>
        private string GetKey(string fullName, string alias) => $"{fullName}_{alias}";

        /// <summary>
        /// 注册映射关系
        /// </summary>
        public void Register<TFrom, TTo>(string alias = null, object[] paraList = null, LifetimeType lifetimeType = LifetimeType.Transient)
        {
            string key = GetKey(typeof(TFrom).FullName, alias);
            if (objectMappingDict.ContainsKey(key))
            {
                return;
            }
            objectMappingDict.Add(key, new ObjectModel()
            {
                Lifetime = lifetimeType,
                TargetType = typeof(TTo)
            });
            if (paraList != null && paraList.Length > 0)
            {
                constractParasDict.Add(key, paraList);
            }
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        public TFrom Resolve<TFrom>(string alias = null)
        {
            return (TFrom)this.Resolve(typeof(TFrom), alias);
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        private object Resolve(Type abstractType, string alias = null)
        {
            string key = GetKey(abstractType.FullName, alias);
            var model = objectMappingDict[key];

            #region lifetime 获取缓存
            switch (model.Lifetime)
            {
                case LifetimeType.Transient:
                    Console.WriteLine("Transient Do Nothing Before~~");
                    break;
                case LifetimeType.Singleton:
                    if (model.SingletonInstance != null)
                    {
                        return model.SingletonInstance;
                    }
                    break; 
                case LifetimeType.Scope:
                    if (scopeInstanceDict.ContainsKey(key))
                    {
                        return scopeInstanceDict[key];
                    }
                    break;
                case LifetimeType.PerThread:
                    object oValue = CustomCallContext<object>.GetData($"{key}_{Thread.CurrentThread.ManagedThreadId}");
                    if (oValue != null)
                    {
                        return oValue;
                    }
                    break;
                default:
                    break;
            }
            #endregion

            Type type = model.TargetType;

            #region  选择合适的构造函数
            // 先选择记特性的构造函数
            ConstructorInfo constractor = type.GetConstructors().FirstOrDefault(x => x.IsDefined(typeof(ConstructorAttribute), true));
            if (constractor == null)
            {
                //选择参数个数最多的构造函数
                constractor = type.GetConstructors().OrderByDescending(x => x.GetParameters().Length).First();
            }
            #endregion

            #region  准备构造函数的参数
            List<object> paraList = new List<object>();
            // 固定常量参数
            object[] paraConstant = constractParasDict.ContainsKey(key) ? constractParasDict[key] : null;
            int index = 0;
            foreach (var para in constractor.GetParameters())
            {
                if (para.IsDefined(typeof(ParamterConstantAttribute), true))
                {
                    paraList.Add(paraConstant[index++]);
                }
                else
                {
                    Type paraType = para.ParameterType;
                    string aliasPara = GetAlias(para);
                    object paraInstance = Resolve(paraType, aliasPara);
                    paraList.Add(paraInstance);
                }
            }
            #endregion

            object oInstance = Activator.CreateInstance(type, paraList.ToArray());

            #region 属性注入
            foreach (var prop in type.GetProperties().Where(x => x.IsDefined(typeof(PropertyInjectionAttribute), true)))
            {
                Type propType = prop.PropertyType;
                var aliasProp = GetAlias(prop);
                object propInstance = Resolve(propType, aliasProp);
                prop.SetValue(oInstance, propInstance);
            }
            #endregion

            #region 方法注入
            foreach (var method in type.GetMethods().Where(m => m.IsDefined(typeof(MethodInjectionAttribute), true)))
            {
                List<object> paraInjectionList = new List<object>();
                foreach (var para in method.GetParameters())
                {
                    Type paraType = para.ParameterType;
                    string aliasPara = GetAlias(para);
                    object paraInstance = Resolve(paraType, aliasPara);
                    paraInjectionList.Add(paraInstance);
                }
                method.Invoke(oInstance, paraInjectionList.ToArray());
            }
            #endregion

            #region lifetime 记入缓存
            switch (model.Lifetime)
            {
                case LifetimeType.Transient:
                    Console.WriteLine("Transient Do Nothing After~~");
                    break;
                case LifetimeType.Singleton:
                    model.SingletonInstance = oInstance;
                    break;
                case LifetimeType.Scope:
                    scopeInstanceDict[key] = oInstance;
                    break;
                case LifetimeType.PerThread:
                    CustomCallContext<object>.SetData($"{key}_{Thread.CurrentThread.ManagedThreadId}", oInstance);
                    break;
                default:
                    break;
            }
            #endregion

            return oInstance.GetProxy(abstractType);
        }

        /// <summary>
        /// 获取PropertyInfo,ParameterInfo上的特性
        /// </summary>
        private string GetAlias(ICustomAttributeProvider provider)
        {
            if (provider.IsDefined(typeof(AliasAttribute), true))
            {
                var attribute = (AliasAttribute)(provider.GetCustomAttributes(typeof(AliasAttribute), true)[0]);
                return attribute.Alias;
            }
            return null;
        }

        /// <summary>
        /// 克隆IOC容器（给一个请求）
        /// </summary>
        public IObjectContainer CloneContainer()
        {
            return new ObjectContainer(this.objectMappingDict, this.constractParasDict, new Dictionary<string, object>());
        }
    }
}