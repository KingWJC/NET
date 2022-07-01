using ADF.DataAccess;
using ADF.Entity;
using ADF.IBusiness;
using System.Data;
using System.Linq.Expressions;

namespace ADF.Business
{
    /// <summary>
    /// 业务处理基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseBusiness<T> where T : BaseModel, new()
    {
        public BaseBusiness()
        {
        }

        protected DbContext GetService()
        {
            return new DbContext();
        }

        public DbContext GetService(DatabaseType? type, string conStr)
        {
            return new DbContext(type, conStr);
        }

        /// <summary>
        /// 执行（封装）,不能用相同类型字符T
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        private S Execute<S>(Func<DbContext, S> func)
        {
            using (DbContext Service = GetService())
            {
                return func.Invoke(Service);
            }
        }

        #region 添加数据
        public void Insert(T entity)
        {
            Execute(service =>
            {
                service.Insert(entity);
                return true;
            });
        }

        public void Insert(List<T> entities)
        {
            Execute(service =>
            {
                service.Insert<T>(entities);
                return true;
            });
        }

        public void BulkInsert(List<T> entities)
        {
            Execute(service =>
            {
                service.BulkInsert(entities);
                return true;
            });
        }
        #endregion

        #region 删除数据
        /// <summary>
        /// 删除所有数据
        /// </summary>
        public void DeleteAll()
        {
            Execute(service =>
            {
                service.DeleteAll<T>();
                return true;
            });
        }

        /// <summary>
        /// 删除指定主键数据
        /// </summary>
        /// <param name="key"></param>
        public void Delete(string key)
        {
            Execute(service =>
            {
                service.Delete(new List<string> { key });
                return true;
            });
        }

        /// <summary>
        /// 删除指定复杂条件的数据
        /// </summary>
        /// <param name="key"></param>
        public void Delete(Expression<Func<T, bool>> expression)
        {
            Execute(service =>
            {
                service.Delete(expression);
                return true;
            });
        }

        /// <summary>
        /// 通过主键删除多条数据
        /// </summary>
        /// <param name="keys"></param>
        public void Delete(List<string> keys)
        {
            Execute(service =>
            {
                service.Delete(keys);
                return true;
            });
        }

        /// <summary>
        /// 删除单条数据
        /// </summary>
        /// <param name="entity">实体对象</param>
        public void Delete(T entity)
        {
            Execute(service =>
            {
                service.Delete(entity);
                return true;
            });
        }

        /// <summary>
        /// 删除多条数据
        /// </summary>
        /// <param name="entities">实体对象集合</param>
        public void Delete(List<T> entities)
        {
            Execute(service =>
            {
                service.Delete(entities);
                return true;
            });
        }
        #endregion

        #region 更新数据

        /// <summary>
        /// 更新单条数据
        /// </summary>
        /// <param name="entity"></param>
        public void Update(T entity)
        {
            Execute(service =>
            {
                service.Update(entity);
                return true;
            });
        }

        /// <summary>
        /// 更新多条数据
        /// </summary>
        /// <param name="entities"></param>
        public void Update(List<T> entities)
        {
            Execute(service =>
            {
                service.Update(entities);
                return true;
            });
        }

        /// <summary>
        /// 更新单条数据指定属性
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <param name="properties">属性</param>
        public void UpdateAny(T entity, List<string> properties)
        {
            Execute(service =>
            {
                service.UpdateAny(entity, properties);
                return true;
            });
        }

        /// <summary>
        /// 更新多条数据执行属性
        /// </summary>
        /// <param name="entities">实体对象集合</param>
        /// <param name="properties">属性</param>
        public void UpdateAny(List<T> entities, List<string> properties)
        {
            Execute(service =>
            {
                service.UpdateAny(entities, properties);
                return true;
            });
        }
        #endregion

        #region 查询数据
        /// <summary>
        /// 通过主键获取单条数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public T GetEntity(int keyValue)
        {
            return Execute(service => service.GetEntity<T>(keyValue));
        }

        /// <summary>
        /// 获取所有数据
        /// 注:会获取所有数据,数据量大请勿使用
        /// </summary>
        /// <returns></returns>
        public List<T> GetList()
        {
            return Execute(service => service.GetList<T>());
        }

        /// <summary>
        /// 通过SQL获取DataTable
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <returns></returns>
        public DataTable GetDataTableWithSql(string sql)
        {
            return Execute(service => service.GetDataTableWithSql(sql));
        }

        /// <summary>
        /// 通过SQL获取DataTable
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public DataTable GetDataTableWithSql(string sql, List<CusDbParameter> parameters)
        {
            return Execute(service => service.GetDataTableWithSql(sql,parameters));
        }

        /// <summary>
        /// 通过SQL获取List
        /// </summary>
        /// <typeparam name="U">泛型</typeparam>
        /// <param name="sqlStr">SQL</param>
        /// <returns></returns>
        public List<U> GetListBySql<U>(string sqlStr) where U : class, new()
        {
            return Execute(service => service.GetListBySql<U>(sqlStr));
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
            return Execute(service => service.GetListBySql<U>(sqlStr,param));
        }

        #endregion

        #region 执行Sql语句

        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="sql">SQL</param>
        public int ExecuteSql(string sql)
        {
            return Execute(service => service.ExecuteSql(sql));
        }

        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <param name="spList">参数</param>
        public int ExecuteSql(string sql, List<CusDbParameter> cusDbParameters)
        {
            using (DbContext Service = GetService())
            {
                return Service.ExecuteSql(sql, cusDbParameters);
            }
            return Execute(service => service.ExecuteSql(sql,cusDbParameters));
        }
        #endregion
    }
}