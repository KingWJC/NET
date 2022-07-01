using Microsoft.Extensions.Configuration;

namespace ADF.Utility
{
    /// <summary>
    /// 配置文件帮助类（读写分离，负载均衡）
    /// </summary>
    public class ConfigHelper
    {
        static IConfigurationRoot? _config;

        private static string _sqlConnectionStrWrite = null;
        public static string SqlConnectionStrWrite { get { return _sqlConnectionStrWrite; } }

        private static string[] _sqlConnectionStrRead = null;
        public static string[] SQLConnectionStrRead { get { return _sqlConnectionStrRead; } }

        static ConfigHelper()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            _config = builder.Build();
            _sqlConnectionStrWrite = _config["ConectionStrings:Write"];
            _sqlConnectionStrRead = _config.GetSection("ConnectionStrings").GetSection("Read").GetChildren().Select(x => x.Value).ToArray();
        }

        /// <summary>
        /// 获取连接字符串
        /// </summary>
        /// <param name="dbName">数据库名称</param>
        /// <returns></returns>
        public static string GetConnectionStr(string dbName)
        {
            return _config.GetConnectionString(dbName);
        }

        /// <summary>
        /// 获取配置信息
        /// </summary>
        /// <param name="appName">键名</param>
        /// <returns></returns>
        public static string GetValue(string appName)
        {
            return _config[appName];
        }
    }
}