using Chloe.Infrastructure;
using System.Data;

#if net5
using Microsoft.Data.SqlClient;
#else
using System.Data.SqlClient;
#endif

namespace Chloe.SqlServer
{
    public class DefaultDbConnectionFactory : IDbConnectionFactory
    {
        string _connString;
        public DefaultDbConnectionFactory(string connString)
        {
            PublicHelper.CheckNull(connString, "connString");

            this._connString = connString;
        }
        public IDbConnection CreateConnection()
        {
            SqlConnection conn = new SqlConnection(this._connString);
            return conn;
        }
    }
}
