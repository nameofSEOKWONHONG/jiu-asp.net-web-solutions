using Chloe.Infrastructure.Interception;
using System.Data;
using System.Threading.Tasks;

namespace Chloe.Core
{
    class DbSession : IDbSession
    {
        DbContext _dbContext;
        internal DbSession(DbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public IDbContext DbContext { get { return this._dbContext; } }
        public IDbConnection CurrentConnection { get { return this._dbContext.AdoSession.DbConnection; } }
        /// <summary>
        /// 如果未开启事务，则返回 null
        /// </summary>
        public IDbTransaction CurrentTransaction { get { return this._dbContext.AdoSession.DbTransaction; } }
        public bool IsInTransaction { get { return this._dbContext.AdoSession.IsInTransaction; } }
        public int CommandTimeout { get { return this._dbContext.AdoSession.CommandTimeout; } set { this._dbContext.AdoSession.CommandTimeout = value; } }

        public int ExecuteNonQuery(string cmdText, params DbParam[] parameters)
        {
            return this.ExecuteNonQuery(cmdText, CommandType.Text, parameters);
        }
        public int ExecuteNonQuery(string cmdText, CommandType cmdType, params DbParam[] parameters)
        {
            PublicHelper.CheckNull(cmdText, "cmdText");
            return this._dbContext.AdoSession.ExecuteNonQuery(cmdText, parameters, cmdType);
        }
        public int ExecuteNonQuery(string cmdText, object parameter)
        {
            return this.ExecuteNonQuery(cmdText, PublicHelper.BuildParams(this._dbContext, parameter));
        }
        public int ExecuteNonQuery(string cmdText, CommandType cmdType, object parameter)
        {
            return this.ExecuteNonQuery(cmdText, cmdType, PublicHelper.BuildParams(this._dbContext, parameter));
        }

        public async Task<int> ExecuteNonQueryAsync(string cmdText, params DbParam[] parameters)
        {
            return await this.ExecuteNonQueryAsync(cmdText, CommandType.Text, parameters);
        }
        public async Task<int> ExecuteNonQueryAsync(string cmdText, CommandType cmdType, params DbParam[] parameters)
        {
            PublicHelper.CheckNull(cmdText, "cmdText");
            return await this._dbContext.AdoSession.ExecuteNonQueryAsync(cmdText, parameters, cmdType);
        }
        public async Task<int> ExecuteNonQueryAsync(string cmdText, object parameter)
        {
            return await this.ExecuteNonQueryAsync(cmdText, PublicHelper.BuildParams(this._dbContext, parameter));
        }
        public async Task<int> ExecuteNonQueryAsync(string cmdText, CommandType cmdType, object parameter)
        {
            return await this.ExecuteNonQueryAsync(cmdText, cmdType, PublicHelper.BuildParams(this._dbContext, parameter));
        }

        public object ExecuteScalar(string cmdText, params DbParam[] parameters)
        {
            return this.ExecuteScalar(cmdText, CommandType.Text, parameters);
        }
        public object ExecuteScalar(string cmdText, CommandType cmdType, params DbParam[] parameters)
        {
            PublicHelper.CheckNull(cmdText, "cmdText");
            return this._dbContext.AdoSession.ExecuteScalar(cmdText, parameters, cmdType);
        }
        public object ExecuteScalar(string cmdText, object parameter)
        {
            return this.ExecuteScalar(cmdText, PublicHelper.BuildParams(this._dbContext, parameter));
        }
        public object ExecuteScalar(string cmdText, CommandType cmdType, object parameter)
        {
            return this.ExecuteScalar(cmdText, cmdType, PublicHelper.BuildParams(this._dbContext, parameter));
        }

        public async Task<object> ExecuteScalarAsync(string cmdText, params DbParam[] parameters)
        {
            return await this.ExecuteScalarAsync(cmdText, CommandType.Text, parameters);
        }
        public async Task<object> ExecuteScalarAsync(string cmdText, CommandType cmdType, params DbParam[] parameters)
        {
            PublicHelper.CheckNull(cmdText, "cmdText");
            return await this._dbContext.AdoSession.ExecuteScalarAsync(cmdText, parameters, cmdType);
        }
        public async Task<object> ExecuteScalarAsync(string cmdText, object parameter)
        {
            return await this.ExecuteScalarAsync(cmdText, PublicHelper.BuildParams(this._dbContext, parameter));
        }
        public async Task<object> ExecuteScalarAsync(string cmdText, CommandType cmdType, object parameter)
        {
            return await this.ExecuteScalarAsync(cmdText, cmdType, PublicHelper.BuildParams(this._dbContext, parameter));
        }

        public IDataReader ExecuteReader(string cmdText, params DbParam[] parameters)
        {
            return this.ExecuteReader(cmdText, CommandType.Text, parameters);
        }
        public IDataReader ExecuteReader(string cmdText, CommandType cmdType, params DbParam[] parameters)
        {
            PublicHelper.CheckNull(cmdText, "cmdText");
            return this._dbContext.AdoSession.ExecuteReader(cmdText, parameters, cmdType);
        }
        public IDataReader ExecuteReader(string cmdText, object parameter)
        {
            return this.ExecuteReader(cmdText, PublicHelper.BuildParams(this._dbContext, parameter));
        }
        public IDataReader ExecuteReader(string cmdText, CommandType cmdType, object parameter)
        {
            return this.ExecuteReader(cmdText, cmdType, PublicHelper.BuildParams(this._dbContext, parameter));
        }

        public async Task<IDataReader> ExecuteReaderAsync(string cmdText, params DbParam[] parameters)
        {
            return await this.ExecuteReaderAsync(cmdText, CommandType.Text, parameters);
        }
        public async Task<IDataReader> ExecuteReaderAsync(string cmdText, CommandType cmdType, params DbParam[] parameters)
        {
            PublicHelper.CheckNull(cmdText, "cmdText");
            return await this._dbContext.AdoSession.ExecuteReaderAsync(cmdText, parameters, cmdType);
        }
        public async Task<IDataReader> ExecuteReaderAsync(string cmdText, object parameter)
        {
            return await this.ExecuteReaderAsync(cmdText, PublicHelper.BuildParams(this._dbContext, parameter));
        }
        public async Task<IDataReader> ExecuteReaderAsync(string cmdText, CommandType cmdType, object parameter)
        {
            return await this.ExecuteReaderAsync(cmdText, cmdType, PublicHelper.BuildParams(this._dbContext, parameter));
        }

        public void UseTransaction(IDbTransaction dbTransaction)
        {
            this._dbContext.AdoSession.UseExternalTransaction(dbTransaction);
        }
        public void BeginTransaction()
        {
            this._dbContext.AdoSession.BeginTransaction(null);
        }
        public void BeginTransaction(IsolationLevel il)
        {
            this._dbContext.AdoSession.BeginTransaction(il);
        }
        public void CommitTransaction()
        {
            this._dbContext.AdoSession.CommitTransaction();
        }
        public void RollbackTransaction()
        {
            this._dbContext.AdoSession.RollbackTransaction();
        }

        public void AddInterceptor(IDbCommandInterceptor interceptor)
        {
            PublicHelper.CheckNull(interceptor, "interceptor");
            this._dbContext.AdoSession.SessionInterceptors.Add(interceptor);
        }
        public void RemoveInterceptor(IDbCommandInterceptor interceptor)
        {
            PublicHelper.CheckNull(interceptor, "interceptor");
            this._dbContext.AdoSession.SessionInterceptors.Remove(interceptor);
        }

        public void Dispose()
        {
            this._dbContext.Dispose();
        }
    }
}
