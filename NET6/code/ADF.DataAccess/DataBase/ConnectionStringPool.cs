using ADF.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADF.DataAccess
{
    /// <summary>
    /// 获取数据库连接字符串（策略）
    /// </summary>
    public class ConnectionStringPool
    {
        public enum DBOperateType
        {
            Read,
            Write
        }

        /// <summary>
        /// 使用在数组中，使用权重设置，不同的从库承担任务量不同
        /// 3个从库的权重分别是  1   3   6
        /// </summary>
        private static string[] _connAll = new string[]
        {
             ConfigHelper.SQLConnectionStrRead[0],
             ConfigHelper.SQLConnectionStrRead[1],
             ConfigHelper.SQLConnectionStrRead[1],
             ConfigHelper.SQLConnectionStrRead[1],
             ConfigHelper.SQLConnectionStrRead[2],
             ConfigHelper.SQLConnectionStrRead[2],
             ConfigHelper.SQLConnectionStrRead[2],
             ConfigHelper.SQLConnectionStrRead[2],
             ConfigHelper.SQLConnectionStrRead[2],
             ConfigHelper.SQLConnectionStrRead[2]
        };

        private static int _iSeed = 0;

        /// <summary>
        /// 按照负载均衡策略，选择一个读取的从库
        /// 
        /// 若要根据数据库的压力来分配，一定得有个实时获取压力的途径：
        /// a 可以根据查询的响应时间  
        /// b 还得途径去获取硬件信息  
        /// c 实时获取下数据库连接数
        /// </summary>
        /// <returns></returns>
        private static string DispatcherConn(int type = 0)
        {
            int readCount = ConfigHelper.SQLConnectionStrRead.Length;
            switch (type)
            {
                // 轮询策略
                case 0:
                    return ConfigHelper.SQLConnectionStrRead[_iSeed++ % readCount];
                //随机（平均）策略
                case 1:
                    return ConfigHelper.SQLConnectionStrRead[new Random().Next(0, readCount)];
                //随机（权重）策略
                case 2:
                    return _connAll[new Random(_iSeed++).Next(0, _connAll.Length)];
                default:
                    throw new Exception("wrong dbOperateType");
            }
        }

        /// <summary>
        /// 获取字符串
        /// </summary>
        /// <param name="opera">读或写</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        internal static string GetConnectionString(DBOperateType opera
            )
        {
            string connStr = null;
            switch (opera)
            {
                case DBOperateType.Write:
                    connStr = ConfigHelper.SqlConnectionStrWrite;
                    break;
                case DBOperateType.Read:
                    connStr = DispatcherConn();
                    break;
                default:
                    throw new Exception("wrong dbOperateType");
            }

            return connStr;
        }
    }
}
