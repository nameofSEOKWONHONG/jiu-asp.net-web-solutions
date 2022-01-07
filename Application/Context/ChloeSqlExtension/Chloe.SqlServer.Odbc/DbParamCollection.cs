using System.Data;

namespace Chloe.SqlServer.Odbc
{
    /// <remarks>
    /// ODBC驱动不支持命名参数，只支持参数占位符。因此严格要求实际参数的顺序和个数，
    /// 这里取消参数复用逻辑。
    /// </remarks>
    class DbParamCollection
    {
        List<DbParam> _dbParams = new List<DbParam>();

        public int Count { get => _dbParams.Count; }
        public DbParam Find(object value, Type paramType, DbType? dbType)
        {
            return null;
        }

        public void Add(DbParam param)
        {
            this._dbParams.Add(param);
        }

        public List<DbParam> ToParameterList()
        {
            return this._dbParams;
        }
    }
}
