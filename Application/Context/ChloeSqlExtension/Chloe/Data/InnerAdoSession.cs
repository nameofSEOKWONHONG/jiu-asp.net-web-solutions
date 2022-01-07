using Chloe.Infrastructure.Interception;
using System.Data;
using System.Threading.Tasks;

namespace Chloe.Data
{
    class InnerAdoSession : IDisposable
    {
        IAdoSession _adoSession;
        IAdoSession _internalAdoSession;
        IAdoSession _externalAdoSession;

        List<IDbCommandInterceptor> _sessionInterceptors;
        IDbCommandInterceptor[] _globalInterceptors;

        public InnerAdoSession(IDbConnection conn)
        {
            this._internalAdoSession = new InternalAdoSession(conn);
            this._adoSession = this._internalAdoSession;
            this.InitEvents(this._internalAdoSession);
        }
        void InitEvents(IAdoSession adoSession)
        {
            adoSession.OnReaderExecuting += this.OnReaderExecuting;
            adoSession.OnReaderExecuted += this.OnReaderExecuted;
            adoSession.OnNonQueryExecuting += this.OnNonQueryExecuting;
            adoSession.OnNonQueryExecuted += this.OnNonQueryExecuted;
            adoSession.OnScalarExecuting += this.OnScalarExecuting;
            adoSession.OnScalarExecuted += this.OnScalarExecuted;
        }

        public IDbConnection DbConnection { get { return this._adoSession.DbConnection; } }
        public IDbTransaction DbTransaction { get { return this._adoSession.DbTransaction; } }
        public bool IsInTransaction { get { return this._adoSession.IsInTransaction; } }
        public int CommandTimeout { get { return this._adoSession.CommandTimeout; } set { this._adoSession.CommandTimeout = value; } }
        public List<IDbCommandInterceptor> SessionInterceptors
        {
            get
            {
                if (this._sessionInterceptors == null)
                    this._sessionInterceptors = new List<IDbCommandInterceptor>(1);

                return this._sessionInterceptors;
            }
        }
        IDbCommandInterceptor[] GlobalInterceptors
        {
            get
            {
                if (this._globalInterceptors == null)
                    this._globalInterceptors = DbInterception.GetInterceptors();

                return this._globalInterceptors;
            }
        }

        /// <summary>
        /// 使用外部事务。
        /// </summary>
        /// <param name="dbTransaction"></param>
        public void UseExternalTransaction(IDbTransaction dbTransaction)
        {
            if (dbTransaction == null)
            {
                this._adoSession = this._internalAdoSession;
                this._externalAdoSession = null;
                return;
            }


            if (this._adoSession == this._internalAdoSession && this._internalAdoSession.IsInTransaction)
            {
                throw new NotSupportedException("当前回话已经开启事务，已开启的事务未提交或回滚前无法使用外部事务。");
            }
            if (this._externalAdoSession != null)
            {
                throw new NotSupportedException("当前回话已经使用了一个外部事务，无法再次使用另一个外部事务。");
            }

            ExternalAdoSession externalAdoSession = new ExternalAdoSession(dbTransaction);
            this.InitEvents(externalAdoSession);

            this._externalAdoSession = externalAdoSession;
            this._adoSession = externalAdoSession;
        }

        public void BeginTransaction(IsolationLevel? il)
        {
            this._adoSession.BeginTransaction(il);
        }
        public void CommitTransaction()
        {
            this._adoSession.CommitTransaction();
        }
        public void RollbackTransaction()
        {
            this._adoSession.RollbackTransaction();
        }

        public IDataReader ExecuteReader(string cmdText, DbParam[] parameters, CommandType cmdType)
        {
            return this._adoSession.ExecuteReader(cmdText, parameters, cmdType);
        }
        public IDataReader ExecuteReader(string cmdText, DbParam[] parameters, CommandType cmdType, CommandBehavior behavior)
        {
            return this._adoSession.ExecuteReader(cmdText, parameters, cmdType, behavior);
        }
        public async Task<IDataReader> ExecuteReaderAsync(string cmdText, DbParam[] parameters, CommandType cmdType)
        {
            return await this._adoSession.ExecuteReaderAsync(cmdText, parameters, cmdType);
        }
        public async Task<IDataReader> ExecuteReaderAsync(string cmdText, DbParam[] parameters, CommandType cmdType, CommandBehavior behavior)
        {
            return await this._adoSession.ExecuteReaderAsync(cmdText, parameters, cmdType, behavior);
        }

        public async Task<IDataReader> ExecuteReader(string cmdText, DbParam[] parameters, CommandType cmdType, bool @async)
        {
            if (@async)
            {
                return await this.ExecuteReaderAsync(cmdText, parameters, cmdType);
            }

            return this.ExecuteReader(cmdText, parameters, cmdType);
        }
        public async Task<IDataReader> ExecuteReader(string cmdText, DbParam[] parameters, CommandType cmdType, CommandBehavior behavior, bool @async)
        {
            if (@async)
            {
                return await this.ExecuteReaderAsync(cmdText, parameters, cmdType, behavior);
            }

            return this.ExecuteReader(cmdText, parameters, cmdType, behavior);
        }

        public int ExecuteNonQuery(string cmdText, DbParam[] parameters, CommandType cmdType)
        {
            return this._adoSession.ExecuteNonQuery(cmdText, parameters, cmdType);
        }
        public async Task<int> ExecuteNonQueryAsync(string cmdText, DbParam[] parameters, CommandType cmdType)
        {
            return await this._adoSession.ExecuteNonQueryAsync(cmdText, parameters, cmdType);
        }
        public async Task<int> ExecuteNonQuery(string cmdText, DbParam[] parameters, CommandType cmdType, bool @async)
        {
            if (@async)
            {
                return await this.ExecuteNonQueryAsync(cmdText, parameters, cmdType);
            }

            return this.ExecuteNonQuery(cmdText, parameters, cmdType);
        }

        public object ExecuteScalar(string cmdText, DbParam[] parameters, CommandType cmdType)
        {
            return this._adoSession.ExecuteScalar(cmdText, parameters, cmdType);
        }
        public async Task<object> ExecuteScalarAsync(string cmdText, DbParam[] parameters, CommandType cmdType)
        {
            return await this._adoSession.ExecuteScalarAsync(cmdText, parameters, cmdType);
        }
        public async Task<object> ExecuteScalar(string cmdText, DbParam[] parameters, CommandType cmdType, bool @async)
        {
            if (@async)
            {
                return await this.ExecuteScalarAsync(cmdText, parameters, cmdType);
            }

            return this.ExecuteScalar(cmdText, parameters, cmdType);
        }

        public void Dispose()
        {
            this._internalAdoSession.Dispose();
        }

        #region DbInterception
        void OnReaderExecuting(IDbCommand cmd, DbCommandInterceptionContext<IDataReader> dbCommandInterceptionContext)
        {
            this.ExecuteDbCommandInterceptors((dbCommandInterceptor) =>
            {
                dbCommandInterceptor.ReaderExecuting(cmd, dbCommandInterceptionContext);
            });
        }
        void OnReaderExecuted(IDbCommand cmd, DbCommandInterceptionContext<IDataReader> dbCommandInterceptionContext)
        {
            this.ExecuteDbCommandInterceptors((dbCommandInterceptor) =>
            {
                dbCommandInterceptor.ReaderExecuted(cmd, dbCommandInterceptionContext);
            });
        }
        void OnNonQueryExecuting(IDbCommand cmd, DbCommandInterceptionContext<int> dbCommandInterceptionContext)
        {
            this.ExecuteDbCommandInterceptors((dbCommandInterceptor) =>
            {
                dbCommandInterceptor.NonQueryExecuting(cmd, dbCommandInterceptionContext);
            });
        }
        void OnNonQueryExecuted(IDbCommand cmd, DbCommandInterceptionContext<int> dbCommandInterceptionContext)
        {
            this.ExecuteDbCommandInterceptors((dbCommandInterceptor) =>
            {
                dbCommandInterceptor.NonQueryExecuted(cmd, dbCommandInterceptionContext);
            });
        }
        void OnScalarExecuting(IDbCommand cmd, DbCommandInterceptionContext<object> dbCommandInterceptionContext)
        {
            this.ExecuteDbCommandInterceptors((dbCommandInterceptor) =>
            {
                dbCommandInterceptor.ScalarExecuting(cmd, dbCommandInterceptionContext);
            });
        }
        void OnScalarExecuted(IDbCommand cmd, DbCommandInterceptionContext<object> dbCommandInterceptionContext)
        {
            this.ExecuteDbCommandInterceptors((dbCommandInterceptor) =>
            {
                dbCommandInterceptor.ScalarExecuted(cmd, dbCommandInterceptionContext);
            });
        }

        void ExecuteDbCommandInterceptors(Action<IDbCommandInterceptor> act)
        {
            IDbCommandInterceptor[] globalInterceptors = this.GlobalInterceptors;
            for (int i = 0; i < globalInterceptors.Length; i++)
            {
                act(globalInterceptors[i]);
            }

            if (this._sessionInterceptors != null)
            {
                for (int i = 0; i < this._sessionInterceptors.Count; i++)
                {
                    act(this._sessionInterceptors[i]);
                }
            }
        }
        #endregion
    }
}
