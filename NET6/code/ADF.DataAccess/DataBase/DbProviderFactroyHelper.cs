using System.Data.Common;
using System.Data.OracleClient;
using System.Data.SqlClient;

namespace ADF.DataAccess
{
    /// <summary>
    /// 生成数据库操作对象的工厂
    /// </summary>
    public class DbProviderFactroyHelper
    {
        /// <summary>
        /// 获取数据库的辅助工厂
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static DbProviderFactory GetProviderFactory(DatabaseType dbType)
        {
            DbProviderFactory? dbProviderFactory = null;
            switch (dbType)
            {
                case DatabaseType.SqlServer: dbProviderFactory = SqlClientFactory.Instance; break;
                case DatabaseType.Oracle: dbProviderFactory = OracleClientFactory.Instance; break;
                default: throw new Exception("请传入有效的数据库");
            }
            return dbProviderFactory;
        }
    }
}
