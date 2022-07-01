using System;

namespace ADF.Utility
{
    /// <summary>
    /// 封装对象
    /// </summary>
    public class ObjectModel
    {
        public Type TargetType { get; set; }
        public LifetimeType Lifetime { get; set; }
        public object SingletonInstance { get; set; }
    }
}