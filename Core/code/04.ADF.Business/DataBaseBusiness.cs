using System;
using System.Text;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml;
using System.Collections.Generic;
using ADF.IBusiness;
using ADF.Entity;
using ADF.Utility;
using ADF.DataAccess.ORM;

namespace ADF.Business
{
    public class DataBaseBusiness : BaseBusiness<TableSpace>, IDataBaseBusiness
    {
        /// <summary>
        /// 获取表空间数据
        /// </summary>
        /// <returns></returns>
        public List<TableSpace> GetTableSpaces()
        {
            string sql = @"SELECT TABLESPACE_NAME as TsName,
       To_char(Round(BYTES / 1024, 2), '99990.00')
       || 'GB' as AllBytes,
       To_char(Round(FREE / 1024, 2), '99990.00')
       || 'GB' as FreeBytes,
       To_char(Round(( BYTES - FREE ) / 1024, 2), '99990.00')
       || 'GB' as UsedBytes,
       To_char(Round(10000 * USED / BYTES) / 100, '99990.00')
       || '%' as Scale  
       FROM   (SELECT A.TABLESPACE_NAME TABLESPACE_NAME,
               Floor(A.BYTES / ( 1024 * 1024 )) BYTES,
               Floor(B.FREE / ( 1024 * 1024 )) FREE,
               Floor(( A.BYTES - B.FREE ) / ( 1024 * 1024 )) USED 
        FROM   (SELECT TABLESPACE_NAME TABLESPACE_NAME,
                       Sum(BYTES) BYTES
                FROM   DBA_DATA_FILES
                GROUP  BY TABLESPACE_NAME) A,
               (SELECT TABLESPACE_NAME TABLESPACE_NAME,
                       Sum(BYTES) FREE
                FROM   DBA_FREE_SPACE
                GROUP  BY TABLESPACE_NAME) B
        WHERE  A.TABLESPACE_NAME = B.TABLESPACE_NAME) 
ORDER  BY Floor(10000 * USED / BYTES) DESC".Replace("\n", " ").Replace("\r", " ");
            return GetDataTableWithSql(sql).ToList<TableSpace>();
        }

        /// <summary>
        /// 表空间扩容
        /// </summary>
        /// <param name="fileName"></param>
        public int ExtendTableSpace(string fileName)
        {
            string sql = $"ALTER TABLESPACE JHICU_KM ADD DATAFILE '{fileName}' size 4167M autoextend on";
            return ExecuteSql(sql);
        }

        /// <summary>
        /// 获取死锁的表信息
        /// </summary>
        /// <returns></returns>
        public List<LockData> GetLockDatas()
        {
            string sql = @"select b.sid,b.serial#,a.os_user_name,b.username,c.object_name,a.locked_mode,logon_time from v$locked_object a,v$session b,dba_objects c where a.session_id = b.sid and c.object_id=a.object_id order by b.logon_time";
            return GetDataTableWithSql(sql).ToList<LockData>();
        }

        /// <summary>
        /// 解锁
        /// </summary>
        /// <param name="slist"></param>
        public int UnlockTable(List<string> slist)
        {
            StringBuilder stringBuilder = new StringBuilder();
            slist.ForEach(ss => stringBuilder.Append($"alter system kill sessin '{ss}';"));
            return ExecuteSql(stringBuilder.ToString());
        }
    }
}