using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace Chloe.Data
{
    public class DbCommandDecorator : IDbCommand, IDisposable
    {
        IDbCommand _dbCommand;
        public DbCommandDecorator(IDbCommand dbCommand)
        {
            PublicHelper.CheckNull(dbCommand);
            this._dbCommand = dbCommand;
        }

        public IDbCommand PersistedCommand { get { return this._dbCommand; } }

        public virtual string CommandText
        {
            get
            {
                return this._dbCommand.CommandText;
            }
            set
            {
                this._dbCommand.CommandText = value;
            }
        }
        public virtual int CommandTimeout
        {
            get
            {
                return this._dbCommand.CommandTimeout;
            }
            set
            {
                this._dbCommand.CommandTimeout = value;
            }
        }
        public virtual CommandType CommandType
        {
            get
            {
                return this._dbCommand.CommandType;
            }
            set
            {
                this._dbCommand.CommandType = value;
            }
        }
        public virtual IDbConnection Connection
        {
            get
            {
                return this._dbCommand.Connection;
            }
            set
            {
                this._dbCommand.Connection = value;
            }
        }
        public virtual IDataParameterCollection Parameters
        {
            get
            {
                return this._dbCommand.Parameters;
            }
        }
        public virtual IDbTransaction Transaction
        {
            get
            {
                return this._dbCommand.Transaction;
            }
            set
            {
                this._dbCommand.Transaction = value;
            }
        }
        public virtual UpdateRowSource UpdatedRowSource
        {
            get
            {
                return this._dbCommand.UpdatedRowSource;
            }
            set
            {
                this._dbCommand.UpdatedRowSource = value;
            }
        }

        public virtual void Cancel()
        {
            this._dbCommand.Cancel();
        }
        public virtual IDbDataParameter CreateParameter()
        {
            return this._dbCommand.CreateParameter();
        }

        public virtual int ExecuteNonQuery()
        {
            return this._dbCommand.ExecuteNonQuery();
        }
        public virtual async Task<int> ExecuteNonQueryAsync()
        {
            DbCommand dbCommand = this._dbCommand as DbCommand;
            if (dbCommand != null)
            {
                return await dbCommand.ExecuteNonQueryAsync();
            }

            return this._dbCommand.ExecuteNonQuery();
        }

        public virtual IDataReader ExecuteReader()
        {
            return this._dbCommand.ExecuteReader();
        }
        public virtual IDataReader ExecuteReader(CommandBehavior behavior)
        {
            return this._dbCommand.ExecuteReader(behavior);
        }
        public virtual async Task<IDataReader> ExecuteReaderAsync()
        {
            DbCommand dbCommand = this._dbCommand as DbCommand;
            if (dbCommand != null)
            {
                return await dbCommand.ExecuteReaderAsync();
            }

            return this._dbCommand.ExecuteReader();
        }
        public virtual async Task<IDataReader> ExecuteReaderAsync(CommandBehavior behavior)
        {
            DbCommand dbCommand = this._dbCommand as DbCommand;
            if (dbCommand != null)
            {
                return await dbCommand.ExecuteReaderAsync(behavior);
            }

            return this._dbCommand.ExecuteReader(behavior);
        }

        public virtual object ExecuteScalar()
        {
            return this._dbCommand.ExecuteScalar();
        }
        public virtual async Task<object> ExecuteScalarAsync()
        {
            DbCommand dbCommand = this._dbCommand as DbCommand;
            if (dbCommand != null)
            {
                return await dbCommand.ExecuteScalarAsync();
            }

            return this._dbCommand.ExecuteScalar();
        }

        public virtual void Prepare()
        {
            this._dbCommand.Prepare();
        }
        public virtual void Dispose()
        {
            this._dbCommand.Dispose();
        }
    }
}
