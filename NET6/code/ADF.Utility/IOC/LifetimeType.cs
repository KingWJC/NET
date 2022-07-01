using System;

namespace ADF.Utility
{
    /// <summary>
    /// 对象生命周期
    /// </summary>
    public enum LifetimeType
    {
        Transient,
        Singleton,
        Scope,
        PerThread//线程单例
    }
}