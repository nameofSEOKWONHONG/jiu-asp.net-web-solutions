using Chloe.DbExpressions;

namespace Chloe.SqlServer
{
    class DbValueExpressionTransformer : DbExpressionVisitor
    {
        static DbValueExpressionTransformer _transformer = new DbValueExpressionTransformer();

        public static DbExpression Transform(DbExpression exp)
        {
            return exp.Accept(_transformer);
        }

        DbExpression ConvertDbBooleanExpression(DbExpression exp)
        {
            DbCaseWhenExpression caseWhenExpression = SqlGenerator.ConstructReturnCSharpBooleanCaseWhenExpression(exp);
            return caseWhenExpression;
        }

        public override DbExpression Visit(DbEqualExpression exp)
        {
            return this.ConvertDbBooleanExpression(exp);
        }
        public override DbExpression Visit(DbNotEqualExpression exp)
        {
            return this.ConvertDbBooleanExpression(exp);
        }
        public override DbExpression Visit(DbNotExpression exp)
        {
            return this.ConvertDbBooleanExpression(exp);
        }

        public override DbExpression Visit(DbBitAndExpression exp)
        {
            return exp;
        }
        public override DbExpression Visit(DbAndExpression exp)
        {
            return this.ConvertDbBooleanExpression(exp);
        }
        public override DbExpression Visit(DbBitOrExpression exp)
        {
            return exp;
        }
        public override DbExpression Visit(DbOrExpression exp)
        {
            return this.ConvertDbBooleanExpression(exp);
        }

        public override DbExpression Visit(DbConvertExpression exp)
        {
            return exp;
        }
        // +
        public override DbExpression Visit(DbAddExpression exp)
        {
            return exp;
        }
        // -
        public override DbExpression Visit(DbSubtractExpression exp)
        {
            return exp;
        }
        // *
        public override DbExpression Visit(DbMultiplyExpression exp)
        {
            return exp;
        }
        // /
        public override DbExpression Visit(DbDivideExpression exp)
        {
            return exp;
        }
        // %
        public override DbExpression Visit(DbModuloExpression exp)
        {
            return exp;
        }
        public override DbExpression Visit(DbNegateExpression exp)
        {
            return exp;
        }
        // <
        public override DbExpression Visit(DbLessThanExpression exp)
        {
            return this.ConvertDbBooleanExpression(exp);
        }
        // <=
        public override DbExpression Visit(DbLessThanOrEqualExpression exp)
        {
            return this.ConvertDbBooleanExpression(exp);
        }
        // >
        public override DbExpression Visit(DbGreaterThanExpression exp)
        {
            return this.ConvertDbBooleanExpression(exp);
        }
        // >=
        public override DbExpression Visit(DbGreaterThanOrEqualExpression exp)
        {
            return this.ConvertDbBooleanExpression(exp);
        }

        public override DbExpression Visit(DbConstantExpression exp)
        {
            return exp;
        }

        public override DbExpression Visit(DbCoalesceExpression exp)
        {
            return exp;
        }

        public override DbExpression Visit(DbCaseWhenExpression exp)
        {
            return exp;
        }

        public override DbExpression Visit(DbTableExpression exp)
        {
            return exp;
        }

        public override DbExpression Visit(DbColumnAccessExpression exp)
        {
            return exp;
        }

        public override DbExpression Visit(DbMemberExpression exp)
        {
            return exp;
        }
        public override DbExpression Visit(DbParameterExpression exp)
        {
            return exp;
        }

        public override DbExpression Visit(DbSubQueryExpression exp)
        {
            return exp;
        }
        public override DbExpression Visit(DbSqlQueryExpression exp)
        {
            return exp;
        }

        public override DbExpression Visit(DbMethodCallExpression exp)
        {
            if (exp.Type == PublicConstants.TypeOfBoolean || exp.Type == PublicConstants.TypeOfBoolean_Nullable)
                return this.ConvertDbBooleanExpression(exp);
            else
                return exp;
        }

        public override DbExpression Visit(DbFromTableExpression exp)
        {
            return exp;
        }

        public override DbExpression Visit(DbJoinTableExpression exp)
        {
            return exp;
        }
        public override DbExpression Visit(DbAggregateExpression exp)
        {
            return exp;
        }

        public override DbExpression Visit(DbInsertExpression exp)
        {
            return exp;
        }
        public override DbExpression Visit(DbUpdateExpression exp)
        {
            return exp;
        }
        public override DbExpression Visit(DbDeleteExpression exp)
        {
            return exp;
        }

        public override DbExpression Visit(DbExistsExpression exp)
        {
            return this.ConvertDbBooleanExpression(exp);
        }
    }
}
