using Chloe.DbExpressions;

namespace Chloe.RDBMS
{
    public class SqlGeneratorBase : DbExpressionVisitor<DbExpression>
    {
        ISqlBuilder _sqlBuilder = new SqlBuilder();

        protected SqlGeneratorBase()
        {

        }

        public ISqlBuilder SqlBuilder { get { return this._sqlBuilder; } }
    }
}
