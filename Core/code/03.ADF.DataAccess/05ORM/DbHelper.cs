using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using System.Threading.Tasks;
using ADF.Utility;

namespace ADF.DataAccess.ORM
{
    public abstract class DbHelper : IDisposable
    {
        private DbProviderFactory factory;
        private string connectionStr;
        private DbConnection connection;
        private DbCommand command;
        public abstract string ParaPrefix { get; }

        protected bool Disposed { get; set; }

        public DbConnection Connection
        {
            get
            {
                if (Disposed || connection == null)
                {
                    connection = factory.CreateConnection();
                    connection.ConnectionString = connectionStr;
                    connection.Open();
                    Disposed = false;
                }
                else if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                else if (connection.State == ConnectionState.Broken)
                {
                    connection.Close();
                    connection.Open();
                }
                return connection;
            }
        }

        public DbCommand Command
        {
            get
            {
                if (Disposed || command == null)
                {
                    command = Connection.CreateCommand();
                }
                return command;
            }
        }

        public DbHelper(DatabaseType dbType, string conStr)
        {
            factory = DbProviderFactroyHelper.GetProviderFactory(dbType);
            this.connectionStr = conStr;
        }

        public DbTransaction UseTransation(IsolationLevel? level = null)
        {
            DbTransaction transaction = null;
            if (level.HasValue)
                transaction = Connection.BeginTransaction();
            else
                transaction = connection.BeginTransaction(level.Value);
            Command.Transaction = transaction;
            return transaction;
        }

        public void SetCommand(string strSQL, List<CusDbParameter> parameters = null, CommandType commandType = CommandType.Text, int timeOut = 600)
        {
            Command.CommandText = strSQL;
            Command.CommandType = commandType;
            Command.CommandTimeout = timeOut;
            Command.Parameters.Clear();
            if (parameters?.Count > 0)
            {
                IDataParameter[] ipars = ToIDbDataParameter(parameters.ToArray());
                command.Parameters.AddRange(ipars);
            }
        }

        public abstract IDataParameter[] ToIDbDataParameter(CusDbParameter[] parameter);

        #region ????????????
        /*
         * @description: ?????????????????????
         * @param {type} 
         * @return: 
         */
        public int ExecuteNonQuery(string strSQL, List<CusDbParameter> parameters = null, CommandType commandType = CommandType.Text)
        {
            try
            {
                SetCommand(strSQL, parameters, commandType);
                return Command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("InsertData", ex);
            }
            finally
            {
                if (this.Connection != null && this.Connection.State != ConnectionState.Closed)
                {
                    this.Connection.Close();
                }
            }
        }

        /// <summary>
        /// ????????????
        /// </summary>
        /// <param name="destTableName">??????????????????????????????</param>
        /// <param name="copyData">DataTable</param>
        /// <param name="timeOut">????????????????????????????????? 300 ????????? 0 ??????????????????????????????????????????????????????</param>
        public abstract void ExecuteBulkCopy(string destTableName, DataTable copyData, int timeOut = 5 * 60);
        #endregion

        #region ????????????
        /*
         * @description: ?????????????????????
         * @param {type} 
         * @return: 
         */
        public int ExecuteCount(string strSQL, List<CusDbParameter> parameters = null, CommandType commandType = CommandType.Text)
        {
            try
            {
                SetCommand(strSQL, parameters, commandType);
                int result = 0;
                using (DbDataReader reader = Command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result = reader.GetInt32(0);
                        }
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("GetCount", ex);
            }
            finally
            {
                if (this.Connection != null && this.Connection.State != ConnectionState.Closed)
                {
                    this.Connection.Close();
                }
            }
        }
        /*
         * @description: ??????????????????
         * @param {type} 
         * @return: 
         */
        public object ExecuteScalar(string strSQL, List<CusDbParameter> parameters = null, CommandType commandType = CommandType.Text)
        {
            try
            {
                SetCommand(strSQL, parameters, commandType);
                object result = Command.ExecuteScalar();
                if (object.Equals(null, result) || object.Equals(DBNull.Value, result))
                {
                    return null;
                }
                else
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("GetCount", ex);
            }
            finally
            {
                if (this.Connection != null && this.Connection.State != ConnectionState.Closed)
                {
                    this.Connection.Close();
                }
            }

        }
        /*
         * @description: ???????????????
         * @param {type} 
         * @return: 
         */
        public DataSet ExecuteDataSet(string strSQL, List<CusDbParameter> parameters = null, CommandType commandType = CommandType.Text)
        {
            try
            {
                SetCommand(strSQL, parameters, commandType);
                DbDataAdapter dataAdapter = factory.CreateDataAdapter();
                dataAdapter.SelectCommand = Command;
                DataSet dataSet = new DataSet();
                dataAdapter.Fill(dataSet);
                return dataSet;
            }
            catch (Exception ex)
            {
                throw new Exception("GetDataSet", ex);
            }
            finally
            {
                if (this.Connection != null && this.Connection.State != ConnectionState.Closed)
                {
                    this.Connection.Close();
                }
            }
        }
        /*
         * @description: ???????????????
         * @param {type} 
         * @return: 
         */
        public DataTable ExecuteDataTable(string strSQL, List<CusDbParameter> parameters = null, CommandType commandType = CommandType.Text)
        {
            DataSet dataSet = ExecuteDataSet(strSQL, parameters, commandType);
            if (dataSet == null || dataSet.Tables.Count == 0)
                return null;
            else
                return dataSet.Tables[0];
        }

        /*
         * @description: ???????????????-??????-In??????
         * @param {type} 
         * @return: s
         */
        public DataTable ExecuteDataTableParallel<T>(string strSQL, List<T> wheres)
        {
            DataTable tables = new DataTable();
            if (null == wheres && 0 == wheres.Count)
            {
                throw new Exception();
            }

            if (100 > wheres.Count)
            {
                string bodySql = string.Format("{0} {1}", strSQL, wheres.TryToWhere());
                return ExecuteDataTable(bodySql);
            }
            else
            {
                var batchValue = wheres.TryToBatchValue();
                Parallel.ForEach(batchValue, values =>
                {
                    string sqlBody = string.Format("{0} {1}", strSQL, values);
                    DataTable table = ExecuteDataTable(sqlBody);
                    tables.Merge(table, true);
                });
            }
            return tables;
        }

        /// <summary>
        /// ????????????????????????
        /// </summary>
        /// <param name="strSQL">?????????????????????????????????</param>
        /// <param name="pageSize">???????????????(??????10)</param>
        /// <param name="pageCurrent">???????????????</param>
        /// <param name="fdShow">????????????????????????(?????????????????????????????????</param>
        /// <param name="fdOrder">??????????????????(?????????????????????????????????????????????)</param>
        /// <param name="totalCount">???????????????</param>
        /// <returns>DataTable</returns>
        public abstract DataTable ExecPageProc(string strSQL, int pageSize, int pageCurrent, string fdShow, string fdOrder, out int totalCount);
        #endregion

        #region  Dispose
        protected virtual void Dispose(bool disposing)
        {
            // ??????????????????????????????????????????
            if (Disposed)
                return;

            if (disposing)
            {
                // ???????????????????????????
                // ??????????????????Dispose???????????????
                factory = null;
            }

            // ??????????????????????????????
            connection?.Dispose();
            command?.Dispose();

            // ????????????????????????
            Disposed = true;
        }
        ~DbHelper()
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