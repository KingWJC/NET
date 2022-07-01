using System.Data;

namespace ADF.DataAccess
{
    /// <summary>
    /// 事务处理接口
    /// </summary>
    public interface ITransaction
    {
        ITransaction BeginTransaction();
        ITransaction BeginTransaction(IsolationLevel level);
        void CommitTransaction();
        void RollbackTransaction();
        (bool success, Exception exception) EndTransaction();
    }
}
