using System;

namespace ADF.Utility
{
    /// <summary>
    /// 构造函数注入
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor)]
    public class ConstructorAttribute : Attribute
    {

    }

    /// <summary>
    /// 属性注入
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyInjectionAttribute : Attribute
    {

    }

    /// <summary>
    /// 方法注入
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class MethodInjectionAttribute : Attribute
    {

    }

    /// <summary>
    /// 构造函数的固定参数初始化
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class ParamterConstantAttribute : Attribute
    {

    }

    /// <summary>
    /// 对象别名
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class AliasAttribute : Attribute
    {
        public string Alias { get; set; }
        public AliasAttribute(string alias)
        {
            this.Alias = alias;
        }
    }
}