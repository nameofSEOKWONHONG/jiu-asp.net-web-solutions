using Chloe.Infrastructure.Interception;
using System.Data;
using System.Threading.Tasks;

namespace Chloe
{
    public interface IDbSession : IDisposable
    {
        IDbContext DbContext { get; }
        IDbConnection CurrentConnection { get; }
        /// <summary>
        /// 如果未开启事务，则返回 null
        /// </summary>
        IDbTransaction CurrentTransaction { get; }
        bool IsInTransaction { get; }
        int CommandTimeout { get; set; }

        int ExecuteNonQuery(string cmdText, params DbParam[] parameters);
        int ExecuteNonQuery(string cmdText, CommandType cmdType, params DbParam[] parameters);
        int ExecuteNonQuery(string cmdText, object parameter);
        /// <summary>
        /// dbSession.ExecuteNonQuery("update Users set Age=18 where Id=@Id", CommandType.Text, new { Id = 1 })
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="cmdType"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        int ExecuteNonQuery(string cmdText, CommandType cmdType, object parameter);

        Task<int> ExecuteNonQueryAsync(string cmdText, params DbParam[] parameters);
        Task<int> ExecuteNonQueryAsync(string cmdText, CommandType cmdType, params DbParam[] parameters);
        Task<int> ExecuteNonQueryAsync(string cmdText, object parameter);
        /// <summary>
        /// dbSession.ExecuteNonQueryAsync("update Users set Age=18 where Id=@Id", CommandType.Text, new { Id = 1 })
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="cmdType"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        Task<int> ExecuteNonQueryAsync(string cmdText, CommandType cmdType, object parameter);

        object ExecuteScalar(string cmdText, params DbParam[] parameters);
        object ExecuteScalar(string cmdText, CommandType cmdType, params DbParam[] parameters);
        object ExecuteScalar(string cmdText, object parameter);
        /// <summary>
        /// dbSession.ExecuteScalar("select Age from Users where Id=@Id", CommandType.Text, new { Id = 1 })
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="cmdType"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        object ExecuteScalar(string cmdText, CommandType cmdType, object parameter);

        Task<object> ExecuteScalarAsync(string cmdText, params DbParam[] parameters);
        Task<object> ExecuteScalarAsync(string cmdText, CommandType cmdType, params DbParam[] parameters);
        Task<object> ExecuteScalarAsync(string cmdText, object parameter);
        /// <summary>
        /// dbSession.ExecuteScalarAsync("select Age from Users where Id=@Id", CommandType.Text, new { Id = 1 })
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="cmdType"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        Task<object> ExecuteScalarAsync(string cmdText, CommandType cmdType, object parameter);

        IDataReader ExecuteReader(string cmdText, params DbParam[] parameters);
        IDataReader ExecuteReader(string cmdText, CommandType cmdType, params DbParam[] parameters);
        IDataReader ExecuteReader(string cmdText, object parameter);
        /// <summary>
        /// dbSession.ExecuteReader("select * from Users where Id=@Id", CommandType.Text, new { Id = 1 })
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="cmdType"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        IDataReader ExecuteReader(string cmdText, CommandType cmdType, object parameter);

        Task<IDataReader> ExecuteReaderAsync(string cmdText, params DbParam[] parameters);
        Task<IDataReader> ExecuteReaderAsync(string cmdText, CommandType cmdType, params DbParam[] parameters);
        Task<IDataReader> ExecuteReaderAsync(string cmdText, object parameter);
        /// <summary>
        /// dbSession.ExecuteReaderAsync("select * from Users where Id=@Id", CommandType.Text, new { Id = 1 })
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="cmdType"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        Task<IDataReader> ExecuteReaderAsync(string cmdText, CommandType cmdType, object parameter);

        /// <summary>
        /// 使用外部事务。传 null 则取消使用外部事务。
        /// </summary>
        /// <param name="dbTransaction"></param>
        void UseTransaction(IDbTransaction dbTransaction);
        void BeginTransaction();
        void BeginTransaction(IsolationLevel il);
        void CommitTransaction();
        void RollbackTransaction();

        /// <summary>
        /// 添加拦截器。注：仅对当前上下文起作用。
        /// </summary>
        /// <param name="interceptor"></param>
        void AddInterceptor(IDbCommandInterceptor interceptor);
        void RemoveInterceptor(IDbCommandInterceptor interceptor);
    }
}
