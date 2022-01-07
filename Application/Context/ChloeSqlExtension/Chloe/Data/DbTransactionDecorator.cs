using System.Data;

namespace Chloe.Data
{
    public class DbTransactionDecorator : IDbTransaction
    {
        IDbTransaction _transaction;
        public DbTransactionDecorator(IDbTransaction transaction)
        {
            PublicHelper.CheckNull(transaction);
            this._transaction = transaction;
        }

        public virtual IDbTransaction PersistedTransaction { get { return this._transaction; } }

        public virtual IDbConnection Connection { get { return this._transaction.Connection; } }
        public virtual IsolationLevel IsolationLevel { get { return this._transaction.IsolationLevel; } }

        public virtual void Commit()
        {
            this._transaction.Commit();
        }
        public virtual void Rollback()
        {
            this._transaction.Rollback();
        }

        public virtual void Dispose()
        {
            this._transaction.Dispose();
        }
    }
}
