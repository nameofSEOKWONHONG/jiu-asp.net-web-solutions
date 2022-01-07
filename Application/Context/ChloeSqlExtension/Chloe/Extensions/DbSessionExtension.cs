using System.Data;
using System.Threading.Tasks;

namespace Chloe
{
    public static class DbSessionExtension
    {
        public static Task<IDataReader> ExecuteReader(this IDbSession dbSession, string cmdText, DbParam[] parameters, bool @async)
        {
            if (@async)
                return dbSession.ExecuteReaderAsync(cmdText, parameters);

            IDataReader dataReader = dbSession.ExecuteReader(cmdText, parameters);
            return Task.FromResult(dataReader);
        }
        public static Task<IDataReader> ExecuteReader(this IDbSession dbSession, string cmdText, CommandType cmdType, DbParam[] parameters, bool @async)
        {
            if (@async)
                return dbSession.ExecuteReaderAsync(cmdText, cmdType, parameters);

            IDataReader dataReader = dbSession.ExecuteReader(cmdText, cmdType, parameters);
            return Task.FromResult(dataReader);
        }

        public static Task<int> ExecuteNonQuery(this IDbSession dbSession, string cmdText, DbParam[] parameters, bool @async)
        {
            if (@async)
                return dbSession.ExecuteNonQueryAsync(cmdText, parameters);

            int rowsAffected = dbSession.ExecuteNonQuery(cmdText, parameters);
            return Task.FromResult(rowsAffected);
        }
        public static Task<int> ExecuteNonQuery(this IDbSession dbSession, string cmdText, CommandType cmdType, DbParam[] parameters, bool @async)
        {
            if (@async)
                return dbSession.ExecuteNonQueryAsync(cmdText, cmdType, parameters);

            int rowsAffected = dbSession.ExecuteNonQuery(cmdText, cmdType, parameters);
            return Task.FromResult(rowsAffected);
        }

        public static Task<object> ExecuteScalar(this IDbSession dbSession, string cmdText, DbParam[] parameters, bool @async)
        {
            if (@async)
                return dbSession.ExecuteScalarAsync(cmdText, parameters);

            object scalar = dbSession.ExecuteScalar(cmdText, parameters);
            return Task.FromResult(scalar);
        }
        public static Task<object> ExecuteScalar(this IDbSession dbSession, string cmdText, CommandType cmdType, DbParam[] parameters, bool @async)
        {
            if (@async)
                return dbSession.ExecuteScalarAsync(cmdText, cmdType, parameters);

            object scalar = dbSession.ExecuteScalar(cmdText, cmdType, parameters);
            return Task.FromResult(scalar);
        }
    }
}
