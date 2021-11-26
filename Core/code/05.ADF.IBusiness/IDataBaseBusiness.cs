using System.Collections.Generic;
using ADF.Entity;

namespace ADF.IBusiness
{
    public interface IDataBaseBusiness
    {
        /// <summary>
        /// 获取表空间数据
        /// </summary>
        /// <returns></returns>
        List<TableSpace> GetTableSpaces();

        /// <summary>
        /// 表空间扩容
        /// </summary>
        /// <param name="spaceName"></param>
        int ExtendTableSpace(string spaceName);

        /// <summary>
        /// 获取死锁的表信息
        /// </summary>
        /// <returns></returns>
        List<LockData> GetLockDatas();

        /// <summary>
        /// 解锁
        /// </summary>
        /// <param name="slist"></param>
        int UnlockTable(List<string> slist);
    }
}