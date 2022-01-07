using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace Chloe.Data
{
    public class DbConnectionDecorator : IDbConnection
    {
        IDbConnection _dbConnection;
        public DbConnectionDecorator(IDbConnection dbConnection)
        {
            PublicHelper.CheckNull(dbConnection);
            this._dbConnection = dbConnection;
        }

        public IDbConnection PersistedConnection { get { return this._dbConnection; } }

        public virtual string ConnectionString
        {
            get { return this._dbConnection.ConnectionString; }
            set { this._dbConnection.ConnectionString = value; }
        }
        public virtual int ConnectionTimeout
        {
            get { return this._dbConnection.ConnectionTimeout; }
        }
        public virtual string Database
        {
            get { return this._dbConnection.Database; }
        }
        public virtual ConnectionState State
        {
            get { return this._dbConnection.State; }
        }

        public virtual IDbTransaction BeginTransaction()
        {
            return this._dbConnection.BeginTransaction();
        }
        public virtual IDbTransaction BeginTransaction(IsolationLevel il)
        {
            return this._dbConnection.BeginTransaction(il);
        }
        public virtual void ChangeDatabase(string databaseName)
        {
            this._dbConnection.ChangeDatabase(databaseName);
        }
        public virtual void Close()
        {
            this._dbConnection.Close();
        }
        public virtual IDbCommand CreateCommand()
        {
            return this._dbConnection.CreateCommand();
        }
        public virtual async Task OpenAsync()
        {
            DbConnection dbConnection = this._dbConnection as DbConnection;
            if (dbConnection != null)
            {
                await dbConnection.OpenAsync();
                return;
            }

            this.Open();
        }
        public virtual void Open()
        {
            this._dbConnection.Open();
        }

        public virtual void Dispose()
        {
            this._dbConnection.Dispose();
        }
    }
}
