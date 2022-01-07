using Chloe.Core;
using Chloe.DbExpressions;
using Chloe.Infrastructure;

namespace Chloe.PostgreSQL
{
    class DbExpressionTranslator : IDbExpressionTranslator
    {
        public static readonly DbExpressionTranslator Instance = new DbExpressionTranslator();
        public virtual DbCommandInfo Translate(DbExpression expression)
        {
            SqlGenerator generator = this.CreateSqlGenerator();
            expression = EvaluableDbExpressionTransformer.Transform(expression);
            expression.Accept(generator);

            DbCommandInfo dbCommandInfo = new DbCommandInfo();
            dbCommandInfo.Parameters = generator.Parameters;
            dbCommandInfo.CommandText = generator.SqlBuilder.ToSql();

            return dbCommandInfo;
        }
        public virtual SqlGenerator CreateSqlGenerator()
        {
            return SqlGenerator.CreateInstance();
        }
    }

    class DbExpressionTranslator_ConvertToLowercase : DbExpressionTranslator
    {
        public static readonly new DbExpressionTranslator_ConvertToLowercase Instance = new DbExpressionTranslator_ConvertToLowercase();
        public override SqlGenerator CreateSqlGenerator()
        {
            return new SqlGenerator_ConvertToLowercase();
        }
    }
}
