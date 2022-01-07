using Chloe.Infrastructure;
using System.Data;

namespace Chloe.SqlServer
{
    class DatabaseProvider : IDatabaseProvider
    {
        IDbConnectionFactory _dbConnectionFactory;
        MsSqlContext _msSqlContext;

        public string DatabaseType { get { return "SqlServer"; } }

        public DatabaseProvider(IDbConnectionFactory dbConnectionFactory, MsSqlContext msSqlContext)
        {
            this._dbConnectionFactory = dbConnectionFactory;
            this._msSqlContext = msSqlContext;
        }
        public IDbConnection CreateConnection()
        {
            return this._dbConnectionFactory.CreateConnection();
        }
        public IDbExpressionTranslator CreateDbExpressionTranslator()
        {
            if (this._msSqlContext.PagingMode == PagingMode.ROW_NUMBER)
            {
                return DbExpressionTranslator.Instance;
            }
            else if (this._msSqlContext.PagingMode == PagingMode.OFFSET_FETCH)
            {
                return DbExpressionTranslator_OffsetFetch.Instance;
            }

            throw new NotSupportedException();
        }
        public string CreateParameterName(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            if (name[0] == UtilConstants.ParameterNamePlaceholer[0])
            {
                return name;
            }

            return UtilConstants.ParameterNamePlaceholer + name;
        }
    }
}
