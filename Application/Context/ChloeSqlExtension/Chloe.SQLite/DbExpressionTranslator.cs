using Chloe.Core;
using Chloe.DbExpressions;
using Chloe.Infrastructure;

namespace Chloe.SQLite
{
    class DbExpressionTranslator : IDbExpressionTranslator
    {
        public static readonly DbExpressionTranslator Instance = new DbExpressionTranslator();
        public DbCommandInfo Translate(DbExpression expression)
        {
            SqlGenerator generator = SqlGenerator.CreateInstance();
            expression = EvaluableDbExpressionTransformer.Transform(expression);
            expression.Accept(generator);

            DbCommandInfo result = new DbCommandInfo();
            result.Parameters = generator.Parameters;
            result.CommandText = generator.SqlBuilder.ToSql();

            return result;
        }
    }
}
