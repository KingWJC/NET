using ADF.Entity;
using ADF.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ADF.DataAccess
{
    /// <summary>
    /// 生成sql，缓存重用（不适合更新）
    /// 提升读取性能，跟类型对应，用泛型缓存
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class SqlCacheBuilder<T> where T : BaseModel, new()
    {
        private static string _findOneSql = null;
        private static string _insertSql = null;
        private static string _updateSql = null;
        private static string _deleteSql = null;

        private static string paraPrefix = "[DB.PARAPREFIX]";

        static SqlCacheBuilder()
        {
            {
                Type type = typeof(T);
                PropertyInfo key = type.GetKeyProperty();
                string columnNameStr = string.Join(",", type.GetProperties().Select(p => $"{p.GetMappingName()}"));
                _findOneSql = $"Select {columnNameStr} From {type.GetMappingTableName()} Where {key.GetMappingColumnName()}  = {paraPrefix + key.GetMappingColumnName()}";
            }
            {
                Type type = typeof(T);
                IEnumerable<PropertyInfo> properties = type.GetProperties();
                string sqlColumns = string.Join(",", properties.Select(x => $"\"{x.GetMappingColumnName()}\"")).ToUpper();
                string sqlValues = string.Join(",", properties.Select(x => $"{paraPrefix + x.GetMappingColumnName()}")).ToUpper();
                _insertSql = string.Format("INSERT INTO {0}({1}) VALUES({2})", type.GetMappingTableName(), sqlColumns, sqlValues);
            }
            {
                Type type = typeof(T);
                PropertyInfo key = type.GetKeyProperty();
                string columnValueStr = string.Join(",", type.GetPropertiesWithNoKey().Select(x => $"\"{x.GetMappingColumnName()}\" = {paraPrefix + x.GetMappingColumnName()}").ToArray()).ToUpper();
                _updateSql = $"Update {type.GetMappingTableName()} SET {columnValueStr} Where {key.GetMappingColumnName()}  = {paraPrefix + key.GetMappingColumnName()}";
            }
            {
                Type type = typeof(T);
                PropertyInfo key = type.GetKeyProperty();
                _deleteSql = $"DELETE FROM {type.GetMappingTableName()} WHERE {key.GetMappingColumnName()}  = {paraPrefix + key.GetMappingColumnName()}";
            }
        }

        internal static string GetSql(SqlType sqlType)
        {
            switch (sqlType)
            {
                case SqlType.Insert: return _insertSql;
                case SqlType.FindOne: return _findOneSql;
                case SqlType.Update: return _updateSql;
                case SqlType.Delete: return _deleteSql;
                default: throw new ArgumentException("Unknown sqlType!");
            }
        }
    }
}
