using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using System.Threading.Tasks;
using ADF.Utility;

namespace ADF.DataAccess.MixFactory
{
    public abstract class DbHelper
    {
        private DbProviderFactory factory;
        private string connectionStr;
        private DbConnection connection;

        public DbConnection Connection
        {
            get
            {
                if (connection == null)
                {
                    connection = factory.CreateConnection();
                    connection.ConnectionString = connectionStr;
                    connection.Open();
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

        public DbHelper(DbProviderFactory dbProviderFactory, string conStr)
        {
            factory = dbProviderFactory;
            this.connectionStr = conStr;
        }

        public DbCommand CreateCommand(DbConnection connect, string strSQL, List<CusDbParameter> parameters = null, CommandType commandType = CommandType.Text, DbTransaction transaction = null, int timeOut = 600)
        {
            DbCommand dbCommand = connect.CreateCommand();
            dbCommand.CommandText = strSQL;
            dbCommand.CommandType = commandType;
            dbCommand.CommandTimeout = timeOut;
            if (parameters?.Count > 0)
            {
                foreach (var item in parameters)
                {
                    dbCommand.Parameters.Add(CreateDbParameter(item));
                }
            }
            if (transaction != null)
            {
                dbCommand.Transaction = transaction;
            }
            return dbCommand;
        }

        public abstract DbParameter CreateDbParameter(CusDbParameter parameter);
        #region ????????????
        /*
         * @description: ?????????????????????
         * @param {type} 
         * @return: 
         */
        public int ExecuteNonQuery(string strSQL, List<CusDbParameter> parameters = null, CommandType commandType = CommandType.Text)
        {

            using (DbConnection connect = Connection)
            using (DbCommand command = CreateCommand(connect, strSQL, parameters, commandType))
            {
                return command.ExecuteNonQuery();
            }
        }

        public int ExecuteNonQuery(Dictionary<string, List<CusDbParameter>> sqlDict)
        {

            using (DbConnection connect = Connection)
            {
                using (DbCommand command = CreateCommand(connect, string.Empty))
                {
                    int result = 0;
                    foreach (var item in sqlDict)
                    {
                        command.Parameters.Clear();
                        command.CommandText = item.Key;
                        if (item.Value != null && item.Value.Count > 0)
                        {
                            foreach (var param in item.Value)
                            {
                                command.Parameters.Add(CreateDbParameter(param));
                            }
                        }
                        result += command.ExecuteNonQuery();
                    }
                    return result;
                }
            }
        }
        /*
         * @description: ?????????????????????????????????
         * @param {type} 
         * @return: 
         */
        public int ExecuteNonQueryUseTrans(string strSQL, List<CusDbParameter> parameters = null, CommandType commandType = CommandType.Text)
        {

            using (DbConnection connect = Connection)
            {
                DbTransaction transaction = connect.BeginTransaction();
                using (DbCommand command = CreateCommand(connect, string.Empty, parameters, commandType, transaction))
                {
                    int result = 0;
                    try
                    {
                        result = command.ExecuteNonQuery();
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw (ex);
                    }
                    return result;
                }
            }
        }
        /*
         * @description: ?????????????????????????????????-????????????
         * @param {type} 
         * @return: 
         */
        public int ExecuteNonQueryUseTrans(Dictionary<string, List<CusDbParameter>> sqlDict)
        {
            using (DbConnection connect = Connection)
            {
                DbTransaction transaction = connect.BeginTransaction();
                using (DbCommand command = CreateCommand(connect, string.Empty, null, CommandType.Text, transaction))
                {
                    int result = 0;
                    try
                    {
                        foreach (var item in sqlDict)
                        {
                            command.Parameters.Clear();
                            command.CommandText = item.Key;
                            if (item.Value != null && item.Value.Count > 0)
                            {
                                foreach (var param in item.Value)
                                {
                                    command.Parameters.Add(CreateDbParameter(param));
                                }
                            }

                            result += command.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw (ex);
                    }
                    return result;
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

        /// <summary>
        /// ????????????(???????????????????????????????????????????????????)
        /// </summary>
        /// <param name="destTableName">??????????????????????????????</param>
        /// <param name="copyData">DataTable</param>
        /// <param name="columns">??????????????????</param>       
        /// <param name="timeOut">????????????????????????????????? 300 ????????? 0 ??????????????????????????????????????????????????????</param>
        public abstract void ExecuteBulkCopy(string destTableName, DataTable copyData, string[][] columns, int timeOut = 5 * 60);
        #endregion

        #region ????????????
        /*
         * @description: ?????????????????????
         * @param {type} 
         * @return: 
         */
        public int ExecuteCount(string strSQL, List<CusDbParameter> parameters = null, CommandType commandType = CommandType.Text)
        {

            using (DbConnection connect = Connection)
            using (DbCommand command = CreateCommand(connect, strSQL, parameters, commandType))
            {
                int result = 0;
                DbDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        result = reader.GetInt32(0);
                    }
                }
                reader.Close();
                return result;
            }
        }
        /*
         * @description: ??????????????????
         * @param {type} 
         * @return: 
         */
        public object ExecuteScalar(string strSQL, List<CusDbParameter> parameters = null, CommandType commandType = CommandType.Text)
        {

            using (DbConnection connect = Connection)
            using (DbCommand command = CreateCommand(connect, strSQL, parameters, commandType))
            {
                object result = command.ExecuteScalar();
                if (object.Equals(null, result) || object.Equals(DBNull.Value, result))
                {
                    return null;
                }
                else
                {
                    return result;
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
            using (DbConnection connect = Connection)
            using (DbCommand command = CreateCommand(connect, strSQL, parameters, commandType))
            {
                DbDataAdapter dataAdapter = factory.CreateDataAdapter();
                dataAdapter.SelectCommand = command;
                DataSet dataSet = new DataSet();
                dataAdapter.Fill(dataSet);
                return dataSet;
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

        public DataRow ExecuteDataRow(string strSQL, List<CusDbParameter> parameters = null, CommandType commandType = CommandType.Text)
        {
            using (DbConnection conn = Connection)
            using (DbCommand command = CreateCommand(conn, strSQL, parameters, commandType))
            {
                DbDataReader reader = command.ExecuteReader();
                if (reader.HasRows && reader.Read())
                {
                    DataTable dataTable = new DataTable();
                    object[] values = new object[reader.FieldCount];
                    int fieldCount = reader.GetValues(values);
                    dataTable.LoadDataRow(values, false);
                    return dataTable.Rows[0];
                }
                return null;
            }
        }

        /*
         * @description: ???????????????-??????-In??????
         * @param {type} 
         * @return: 
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
    }
}