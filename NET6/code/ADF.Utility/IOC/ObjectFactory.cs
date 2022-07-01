using System.Reflection;

namespace ADF.Utility.IOC
{
    /// <summary>
    /// 简单的IOC容器
    /// config配置文件存储具体实现类的类名和命名空间。使用反射实例化。
    /// </summary>
    public class ObjectFactory
    {
        public static T CreateInstance<T>()
        {
            T instance = default(T);
            // "IUserDAL": "Zhaoxi.IOCDI.DAL.UserDAL;Zhaoxi.IOCDI.DAL.dll",
            string config = ConfigHelper.GetValue("IUserDAL");
            Assembly assembly = Assembly.Load(config.Split(';')[1]);
            //load需要dev.json配置依赖关系 
            //LoadFile 完整路径    LoadFromUnsafe 直接dll名称
            Type type = assembly.GetType(config.Split(';')[0]);
            instance = (T)Activator.CreateInstance(type);
            return instance;
        }
        //如果事先没有引用dll而是copydll，load失败了
    }
}
