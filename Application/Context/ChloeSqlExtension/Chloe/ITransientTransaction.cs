using System.Data;

namespace Chloe
{
    /// <summary>
    /// 调用 ITransientTransaction.Dispose() 时，如果事务未提交，则会自动回滚
    /// </summary>
    public interface ITransientTransaction : IDisposable
    {
        IDbContext DbContext { get; }
        void Commit();
        void Rollback();
    }

    class TransientTransaction : ITransientTransaction
    {
        bool _disposed = false;
        bool _completed = false;
        public TransientTransaction(IDbContext dbContext)
        {
            this.DbContext = dbContext;
            this.DbContext.Session.BeginTransaction();
        }
        public TransientTransaction(IDbContext dbContext, IsolationLevel il)
        {
            this.DbContext = dbContext;
            this.DbContext.Session.BeginTransaction(il);
        }
        public void Commit()
        {
            if (this._completed)
                return;

            if (this.DbContext.Session.IsInTransaction)
                this.DbContext.Session.CommitTransaction();

            this._completed = true;
        }

        public IDbContext DbContext { get; private set; }

        public void Rollback()
        {
            if (this._completed)
                return;

            if (this.DbContext.Session.IsInTransaction)
                this.DbContext.Session.RollbackTransaction();

            this._completed = true;
        }

        public void Dispose()
        {
            if (this._disposed)
                return;

            this.Rollback();
            this._disposed = true;
        }
    }
}
