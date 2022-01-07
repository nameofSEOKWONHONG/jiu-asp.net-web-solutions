using Chloe.Data;
using System.Data;
using System.Threading;

namespace Chloe.SQLite
{
    /* 
     * ********************* Warning ********************* 
     * 支持读写并发控制
     * 由于内部使用了 ReaderWriterLockSlim 锁，所以，任何对 sqlite 的操作只能在创建该连接的线程内，不能跨线程操作。否则会出现无法释放锁的风险！！！！！
     * 同时，如果开启了事务后，必须保证事务最终被 Commit 或 Rollback，不然内部 ReaderWriterLockSlim 锁得不到释放，会导致锁一直被占用，使得整个程序持续阻塞！！！ 
     */
    public class ChloeSQLiteConcurrentConnection : DbConnectionDecorator, IDbConnection, IDisposable
    {
        ReaderWriterLockWrapper _rwLock;

        static readonly Dictionary<string, ReaderWriterLockSlim> _RWLocks = new Dictionary<string, ReaderWriterLockSlim>();

        public ChloeSQLiteConcurrentConnection(IDbConnection dbConnection) : base(dbConnection)
        {
            if (dbConnection.ConnectionString == null)
                throw new ArgumentException("The connectionString cannot be null.");

            ReaderWriterLockSlim rwLockSlim = GetReaderWriterLock(dbConnection.ConnectionString);

            this._rwLock = new ReaderWriterLockWrapper(rwLockSlim);
        }

        static ReaderWriterLockSlim GetReaderWriterLock(string connString)
        {
            ReaderWriterLockSlim rwLockSlim;
            if (!_RWLocks.TryGetValue(connString, out rwLockSlim))
            {
                lock (_RWLocks)
                {
                    if (!_RWLocks.TryGetValue(connString, out rwLockSlim))
                    {
                        rwLockSlim = new ReaderWriterLockSlim();
                        _RWLocks[connString] = rwLockSlim;
                    }
                }
            }

            return rwLockSlim;
        }

        internal ReaderWriterLockWrapper RWLock { get { return this._rwLock; } }

        public IDbConnection PersistedDbConnection { get { return this.PersistedConnection; } }

        public override IDbTransaction BeginTransaction()
        {
            if (this._rwLock.IsInTransaction)
                throw new Exception("Current connection has begun a transaction.");
            return new ChloeSQLiteTransaction(this);
        }
        public override IDbTransaction BeginTransaction(IsolationLevel il)
        {
            if (this._rwLock.IsInTransaction)
                throw new Exception("Current connection has begun a transaction.");
            return new ChloeSQLiteTransaction(this, il);
        }

        public override IDbCommand CreateCommand()
        {
            return new ChloeSQLiteCommand(this);
        }
    }
}
