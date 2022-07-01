namespace ADF.Utility
{
    /// <summary>
    /// IOC容器接口
    /// </summary>
    public interface IObjectContainer
    {
        /// <summary>
        /// 注册映射关系
        /// </summary>
        void Register<TFrom, TTo>(string alias = null, object[] paraList = null, LifetimeType lifetimeType = LifetimeType.Transient);
        /// <summary>
        /// 获取对象
        /// </summary>
        TFrom Resolve<TFrom>(string alias = null);
        /// <summary>
        /// 克隆IOC容器（给一个请求）
        /// </summary>
        IObjectContainer CloneContainer();
    }
}