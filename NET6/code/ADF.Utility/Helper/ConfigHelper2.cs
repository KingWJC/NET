using Microsoft.Extensions.Configuration;

namespace ADF.Utility
{
    /// <summary>
    /// 配置文件帮助类(单例模式）
    /// </summary>
    public class ConfigHelper2
    {
        static Object _lock = new();
        static IConfigurationRoot? _config;
        /// <summary>
        /// 读取配置文件
        /// </summary>
        public static IConfigurationRoot Config
        {
            get
            {
                if (_config == null)
                {
                    lock (_lock)
                    {
                        if (null == _config)
                        {
                            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
                            _config = builder.Build();

                        }
                    }
                }
                return _config;
            }
        }

        /// <summary>
        /// 获取连接字符串
        /// </summary>
        /// <param name="dbName">数据库名称</param>
        /// <returns></returns>
        public static string GetConnectionStr(string dbName)
        {
            return Config.GetConnectionString(dbName);
        }

        /// <summary>
        /// 获取配置信息
        /// </summary>
        /// <param name="appName">键名</param>
        /// <returns></returns>
        public static string GetValue(string appName)
        {
            return Config[appName];
        }
    }
}