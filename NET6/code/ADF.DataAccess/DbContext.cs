using ADF.Entity;
using ADF.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ADF.DataAccess
{
    public class DbContext : ITransaction, IDisposable
    {
        private DbTransaction dbTransaction;
        private DatabaseType? databaseType;
        private string connectionStr;
        private DbHelper _db;
        private Action transactionHandler;
        protected bool Disposed = false;
        protected bool OpenTransaction = false;
        private const string paraPrefix = "[DB.PARAPREFIX]";
        public DbHelper Db
        {
            get
            {
                if (Disposed || _db == null)
                {
                    _db = DbHelperFactory.GetDbHelper(databaseType, connectionStr);
                }
                return _db;
            }
        }

        public DbContext()
        { }

        public DbContext(DatabaseType? dbType, string conStr)
        {
            this.databaseType = dbType;
            this.connectionStr = conStr;
        }

        #region 私有方法
        /// <summary>
        /// 获取更新SQL
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        protected (string, List<CusDbParameter>) GenerateUpdateSQL<T>(T t, Func<PropertyInfo, bool>? where = null)
        {
            StringBuilder updateSql = new StringBuilder($"Update {t.GetType().GetMappingTableName()} SET ");
            List<CusDbParameter> parameters = new List<CusDbParameter>();
            var properties = typeof(T).GetPropertiesWithNoKey();
            foreach (PropertyInfo subItem in properties)
            {
                if (subItem.Name == "CREATE_DATE" || (where != null && !where(subItem)))
                {
                    continue;
                }

                var subValue = subItem.GetValue(t, null);
                parameters.Add(new CusDbParameter($"{Db.ParaPrefix + subItem.Name}", subValue ?? DBNull.Value, GetDataType(subItem.PropertyType)));
                updateSql.Append($"\"{subItem.Name}\" = {Db.ParaPrefix + subItem.Name},");
            }

            PropertyInfo? keyInfo = typeof(T).GetKeyProperty();
            parameters.Add(new CusDbParameter($"{Db.ParaPrefix + keyInfo?.Name}", keyInfo.GetValue(t) ?? DBNull.Value, GetDataType(keyInfo.PropertyType)));

            updateSql.Remove(updateSql.Length - 1, 1);
            updateSql.Append($" Where {keyInfo?.GetMappingColumnName()}={Db.ParaPrefix + keyInfo?.GetMappingColumnName()}");

            return (updateSql.ToString().TrimEnd(','), parameters);
        }

        /// <summary>
        /// 获取删除SQL
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        protected string GenerateDeleteSQL<T>(List<string> keys, bool isLogic = false)
        {
            Type type = typeof(T);
            var property = type.GetKeyProperty();
            string sql = string.Empty;
            if (property != null)
            {
                sql = $"DELETE FROM {type.GetMappingTableName()} WHERE {property.Name} IN {keys.TryToWhere()}";
                if (isLogic)
                {
                    sql = $"UPDATE {type.GetMappingTableName()} SET DeleteFlag= 1 WHERE {property.Name} IN {keys.TryToWhere()}";
                }
            }
            return sql;
        }

        /// <summary>
        /// 从特定sequence中获取值
        /// </summary>
        /// <param name="seqName">序列名称</param>
        /// <returns>获取到的sequence值</returns>
        protected string GetNextId(string seqName)
        {
            string strSQL = $"SELECT {seqName}.NEXTVAL MAXID  FROM DUAL";
            object obj = Db.ExecuteScalar(strSQL);
            return obj.ToString();
        }

        /// <summary>
        /// 获取自定义参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        protected IEnumerable<CusDbParameter> GenerateParameters<T>(T t)
        {
            var parameters = typeof(T).GetPropertiesWithNoKey().Select(item => new CusDbParameter($"{Db.ParaPrefix + item.Name}", item.GetValue(t) ?? DBNull.Value, GetDataType(item.PropertyType)));
            return parameters;
        }

        /// <summary>
        /// 获取数据库的类型
        /// </summary>
        /// <param name="type">C#类型</param>
        /// <returns></returns>
        private DbType GetDataType(Type type)
        {
            if (type == Constants.ByteArrayType)
            {
                return System.Data.DbType.Binary;
            }
            else if (type == Constants.GuidType)
            {
                return System.Data.DbType.Guid;
            }
            else if (type == Constants.IntType)
            {
                return System.Data.DbType.Int32;
            }
            else if (type == Constants.ShortType)
            {
                return System.Data.DbType.Int16;
            }
            else if (type == Constants.LongType)
            {
                return System.Data.DbType.Int64;
            }
            else if (type == Constants.DateType)
            {
                return System.Data.DbType.DateTime;
            }
            else if (type == Constants.DobType)
            {
                return System.Data.DbType.Double;
            }
            else if (type == Constants.DecType)
            {
                return System.Data.DbType.Decimal;
            }
            else if (type == Constants.ByteType)
            {
                return System.Data.DbType.Byte;
            }
            else if (type == Constants.FloatType)
            {
                return System.Data.DbType.Single;
            }
            else if (type == Constants.BoolType)
            {
                return System.Data.DbType.Boolean;
            }
            else if (type == Constants.StringType)
            {
                return System.Data.DbType.String;
            }
            else if (type == Constants.DateTimeOffsetType)
            {
                return System.Data.DbType.DateTimeOffset;
            }
            else
            {
                return System.Data.DbType.String;
            }

        }

        /// <summary>
        /// 执行数据库操作，判断事务
        /// </summary>
        /// <param name="work"></param>
        private void PackWork(Action work)
        {
            if (OpenTransaction)
            {
                transactionHandler += work;
            }
            else
            {
                work();
                Dispose();
            }
        }
        #endregion

        #region 事务处理
        public ITransaction BeginTransaction()
        {
            dbTransaction = Db.UseTransation();
            return this;
        }

        public ITransaction BeginTransaction(IsolationLevel level)
        {
            dbTransaction = Db.UseTransation(level);
            return this;
        }

        public void CommitTransaction()
        {
            dbTransaction?.Commit();
        }

        public void RollbackTransaction()
        {
            dbTransaction?.Rollback();
        }

        public (bool success, Exception exception) EndTransaction()
        {
            bool success = true;
            Exception exception = null;
            try
            {
                transactionHandler?.Invoke();
                CommitTransaction();
            }
            catch (Exception ex)
            {
                success = false;
                exception = ex;
                RollbackTransaction();
            }
            finally
            {
                Dispose();
            }
            return (success, exception);
        }
        #endregion

        #region 添加数据
        public void Insert<T>(T entity) where T : BaseModel, new()
        {
            string insertSQL = SqlCacheBuilder<T>.GetSql(SqlType.Insert).Replace(paraPrefix, Db.ParaPrefix);
            var parameters = typeof(T).GetProperties().Select(item => new CusDbParameter($"{Db.ParaPrefix + item.Name}", item.GetValue(entity) ?? DBNull.Value, GetDataType(item.PropertyType)));
            Db.ExecuteNonQuery(insertSQL, parameters.ToArray());
        }

        public void Insert<T>(List<T> entities) where T : BaseModel, new()
        {
            PackWork(() =>
            {
                entities.ForEach(p =>
                {
                    Insert<T>(p);
                });
            });

        }

        public void BulkInsert<T>(List<T> entities)
        {
            PackWork(() =>
            {
                Db.ExecuteBulkCopy(typeof(T).Name, null);
            });
        }
        #endregion

        #region 删除数据
        /// <summary>
        /// 删除所有数据
        /// </summary>
        public void DeleteAll<T>()
        {
            var deleteSQL = $"Truncate Table {typeof(T).Name}";
            PackWork(() =>
            {
                Db.ExecuteNonQuery(deleteSQL);
            });

        }

        /// <summary>
        /// 删除指定主键数据
        /// </summary>
        /// <param name="key"></param>
        public void DeleteByKey<T>(string key)
        {
            DeleteByKeys<T>(new List<string> { key });
        }

        /// <summary>
        /// 通过主键删除多条数据
        /// </summary>
        /// <param name="keys"></param>
        public void DeleteByKeys<T>(List<string> keys)
        {
            var deleteSQL = GenerateDeleteSQL<T>(keys);
            PackWork(() =>
            {
                Db.ExecuteNonQuery(deleteSQL);
            });
        }

        /// <summary>
        /// 删除单条数据
        /// </summary>
        /// <param name="entity">实体对象</param>
        public void Delete<T>(T entity)
        {
            var key = typeof(T).GetKeyProperty().GetValue(entity);
            DeleteAny(new List<string> { key.ToString() });
        }

        /// <summary>
        /// 删除多条数据
        /// </summary>
        /// <param name="entities">实体对象集合</param>
        public void DeleteAny<T>(List<T> entities)
        {
            PropertyInfo? keyInfo = typeof(T).GetKeyProperty();
            List<string> keys = entities.Select(p => keyInfo.GetValue(p).ToString()).ToList();
            DeleteByKeys<T>(keys);
        }

        /// <summary>
        /// 删除复杂条件的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        public void Delete<T>(Expression<Func<T, bool>> expression)
        {
            SqlVisitor visitor = new SqlVisitor();
            visitor.Visit(expression);
            string _deleteSql = $"DELETE FROM {typeof(T).GetMappingTableName()} WHERE {visitor.GetWhere()}";
            Db.ExecuteNonQuery(_deleteSql);
        }
        #endregion

        #region 更新数据

        /// <summary>
        /// 更新单条数据
        /// </summary>
        /// <param name="entity"></param>
        public void Update<T>(T entity) where T : BaseModel, new()
        {
            PropertyInfo? keyInfo = typeof(T).GetKeyProperty();

            string updateSql = SqlCacheBuilder<T>.GetSql(SqlType.Update).Replace(paraPrefix, Db.ParaPrefix); ;

            var parameters = GenerateParameters(entity).Append(new CusDbParameter($"{Db.ParaPrefix + keyInfo?.Name}", keyInfo?.GetValue(entity) ?? DBNull.Value, GetDataType(keyInfo.PropertyType)));
            Db.ExecuteNonQuery(updateSql, parameters.ToArray());
        }

        /// <summary>
        /// 更新多条数据
        /// </summary>
        /// <param name="entities"></param>
        public void Update<T>(List<T> entities) where T : BaseModel, new()
        {
            PackWork(() =>
            {
                entities.ForEach(p =>
                {
                    if (!p.ValidateModel())
                    {
                        throw new Exception("数据校验失败");
                    }

                    if (!p.Id.IsNullOrEmpty() && p.Id.ToString() != "0")
                    {
                        Update(p);
                    }
                    else
                    {
                        Insert(p);
                    }
                });
            });
        }

        /// <summary>
        /// 更新单条数据指定属性
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <param name="properties">属性</param>
        public void UpdateAny<T>(T entity, List<string> properties) where T : BaseModel, new()
        {
            UpdateAny<T>(new List<T> { entity }, properties);
        }

        /// <summary>
        /// 更新多条数据执行属性
        /// </summary>
        /// <param name="entities">实体对象集合</param>
        /// <param name="properties">属性</param>
        public void UpdateAny<T>(List<T> entities, List<string> properties) where T : BaseModel, new()
        {
            PackWork(() =>
            {
                entities.ForEach(entity =>
                {
                    if (!entity.ValidateModel())
                    {
                        throw new Exception("数据校验失败");
                    }

                    var (UpdateSQL, Parameters) = GenerateUpdateSQL(entity, p => properties.Contains(p.Name));
                    Db.ExecuteNonQuery(UpdateSQL, Parameters.ToArray());
                });
            });
        }
        #endregion

        #region 查询数据

        /// <summary>
        /// 通过主键获取单条数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public T GetEntity<T>(int id) where T : BaseModel, new()
        {
            string selectSQL = SqlCacheBuilder<T>.GetSql(SqlType.FindOne).Replace(paraPrefix, Db.ParaPrefix);

            PropertyInfo key = typeof(T).GetKeyProperty();
            var parameters = new CusDbParameter[] { new CusDbParameter(Db.ParaPrefix + key.GetMappingColumnName(), id, GetDataType(typeof(int))) };

            List<T> list = Db.ExecuteDataTable(selectSQL, parameters).ToList<T>();
            if (list.Count > 0)
                return list[0];
            else
                return null;
        }

        /// <summary>
        /// 获取所有数据
        /// 注:会获取所有数据,数据量大请勿使用
        /// </summary>
        /// <returns></returns>
        public List<T> GetList<T>()
        {
            Type type = typeof(T);
            string columnNameStr = string.Join(",", type.GetProperties().Select(p => $"\"{p.Name}\""));
            string selectSQL = $"Select {columnNameStr} From {type.GetMappingTableName()}";
            DataTable dataTable = Db.ExecuteDataTable(selectSQL);
            return dataTable.ToList<T>();
        }

        /// <summary>
        /// 通过SQL获取DataTable
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <returns></returns>
        public DataTable GetDataTableWithSql(string sql)
        {
            return Db.ExecuteDataTable(sql);
        }

        /// <summary>
        /// 通过SQL获取DataTable
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public DataTable GetDataTableWithSql(string sql, List<CusDbParameter> parameters)
        {
            return Db.ExecuteDataTable(sql, parameters.ToArray());
        }

        /// <summary>
        /// 通过SQL获取List
        /// </summary>
        /// <typeparam name="U">泛型</typeparam>
        /// <param name="sqlStr">SQL</param>
        /// <returns></returns>
        public List<U> GetListBySql<U>(string sqlStr) where U : class, new()
        {
            return Db.ExecuteDataTable(sqlStr).ToList<U>();
        }

        /// <summary>
        /// 通过SQL获取List
        /// </summary>
        /// <typeparam name="U">泛型</typeparam>
        /// <param name="sqlStr">SQL</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public List<U> GetListBySql<U>(string sqlStr, List<CusDbParameter> param) where U : class, new()
        {
            return Db.ExecuteDataTable(sqlStr, param.ToArray()).ToList<U>();
        }

        #endregion

        #region 执行Sql语句

        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="sql">SQL</param>
        public int ExecuteSql(string sql)
        {
            return Db.ExecuteNonQuery(sql);
        }

        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <param name="spList">参数</param>
        public int ExecuteSql(string sql, List<CusDbParameter> cusDbParameters)
        {
            return Db.ExecuteNonQuery(sql, cusDbParameters.ToArray());
        }

        #endregion

        #region Dispose
        private void Dispose(bool disposing)
        {
            if (Disposed)
                return;

            if (disposing)
            {
                Db?.Dispose();
                dbTransaction?.Dispose();
            }
            OpenTransaction = false;
            transactionHandler = null;
            Disposed = true;
        }

        ~DbContext()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
