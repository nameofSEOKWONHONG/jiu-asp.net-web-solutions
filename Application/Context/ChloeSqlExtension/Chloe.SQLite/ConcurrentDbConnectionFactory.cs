using Chloe.Infrastructure;
using System.Data;

namespace Chloe.SQLite
{
    class ConcurrentDbConnectionFactory : IDbConnectionFactory
    {
        IDbConnectionFactory _dbConnectionFactory;
        public ConcurrentDbConnectionFactory(IDbConnectionFactory dbConnectionFactory)
        {
            this._dbConnectionFactory = dbConnectionFactory;
        }
        public IDbConnection CreateConnection()
        {
            IDbConnection conn = new ChloeSQLiteConcurrentConnection(this._dbConnectionFactory.CreateConnection());
            return conn;
        }
    }
}
