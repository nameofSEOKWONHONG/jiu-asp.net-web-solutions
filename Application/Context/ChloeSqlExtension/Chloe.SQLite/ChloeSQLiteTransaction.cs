using Chloe.Data;
using System.Data;

namespace Chloe.SQLite
{
    public class ChloeSQLiteTransaction : DbTransactionDecorator, IDbTransaction
    {
        ChloeSQLiteConcurrentConnection _connection;
        bool _hasFinished = false;
        public ChloeSQLiteTransaction(ChloeSQLiteConcurrentConnection connection) : base(connection.PersistedConnection.BeginTransaction())
        {
            this._connection = connection;
            this._connection.RWLock.BeginTransaction();
        }
        public ChloeSQLiteTransaction(ChloeSQLiteConcurrentConnection connection, IsolationLevel il) : base(connection.PersistedConnection.BeginTransaction(il))
        {
            this._connection = connection;
            this._connection.RWLock.BeginTransaction();
        }

        ~ChloeSQLiteTransaction()
        {
            this.Dispose();
        }

        void EndTransaction()
        {
            if (this._hasFinished == false)
            {
                this._connection.RWLock.EndTransaction();
                this._hasFinished = true;
            }
        }


        public override IDbConnection Connection { get { return this._connection; } }

        public override void Commit()
        {
            try
            {
                base.Commit();
            }
            finally
            {
                this.EndTransaction();
            }
        }
        public override void Rollback()
        {
            try
            {
                base.Rollback();
            }
            finally
            {
                this.EndTransaction();
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            this.EndTransaction();
            GC.SuppressFinalize(this);
        }
    }
}
