using Chloe.Infrastructure.Interception;
using System.Data;
using System.Threading.Tasks;

namespace Chloe.Data
{
    public delegate void AdoEventHandler<TResult>(IDbCommand command, DbCommandInterceptionContext<TResult> interceptionContext);

    public interface IAdoSession : IDisposable
    {
        IDbConnection DbConnection { get; }
        /// <summary>
        /// 如果未开启事务，则返回 null
        /// </summary>
        IDbTransaction DbTransaction { get; }
        bool IsInTransaction { get; }
        int CommandTimeout { get; set; }

        event AdoEventHandler<IDataReader> OnReaderExecuting;
        event AdoEventHandler<IDataReader> OnReaderExecuted;
        event AdoEventHandler<int> OnNonQueryExecuting;
        event AdoEventHandler<int> OnNonQueryExecuted;
        event AdoEventHandler<object> OnScalarExecuting;
        event AdoEventHandler<object> OnScalarExecuted;

        void Activate();
        Task ActivateAsync();
        /// <summary>
        /// 表示一次查询完成。在事务中的话不关闭连接，交给 CommitTransaction() 或者 RollbackTransaction() 控制，否则调用 IDbConnection.Close() 关闭连接
        /// </summary>
        void Complete();

        void BeginTransaction(IsolationLevel? il);
        void CommitTransaction();
        void RollbackTransaction();

        IDataReader ExecuteReader(string cmdText, DbParam[] parameters, CommandType cmdType);
        IDataReader ExecuteReader(string cmdText, DbParam[] parameters, CommandType cmdType, CommandBehavior behavior);

        Task<IDataReader> ExecuteReaderAsync(string cmdText, DbParam[] parameters, CommandType cmdType);
        Task<IDataReader> ExecuteReaderAsync(string cmdText, DbParam[] parameters, CommandType cmdType, CommandBehavior behavior);

        int ExecuteNonQuery(string cmdText, DbParam[] parameters, CommandType cmdType);
        Task<int> ExecuteNonQueryAsync(string cmdText, DbParam[] parameters, CommandType cmdType);

        object ExecuteScalar(string cmdText, DbParam[] parameters, CommandType cmdType);
        Task<object> ExecuteScalarAsync(string cmdText, DbParam[] parameters, CommandType cmdType);
    }
}
