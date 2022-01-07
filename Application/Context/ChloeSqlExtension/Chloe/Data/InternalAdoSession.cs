using System.Data;

namespace Chloe.Data
{
    class InternalAdoSession : AdoSession, IAdoSession, IDisposable
    {
        IDbConnection _dbConnection;

        public InternalAdoSession(IDbConnection conn)
        {
            this._dbConnection = conn;
        }

        public override IDbConnection DbConnection { get { return this._dbConnection; } }
    }
}
