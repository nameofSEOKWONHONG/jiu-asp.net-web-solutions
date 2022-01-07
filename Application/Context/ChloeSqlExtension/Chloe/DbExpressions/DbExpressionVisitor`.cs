namespace Chloe.DbExpressions
{
    public abstract class DbExpressionVisitor<T>
    {
        public virtual T Visit(DbEqualExpression exp)
        {
            throw new NotImplementedException();
        }
        public virtual T Visit(DbNotEqualExpression exp)
        {
            throw new NotImplementedException();
        }
        // +
        public virtual T Visit(DbAddExpression exp)
        {
            throw new NotImplementedException();
        }
        // -
        public virtual T Visit(DbSubtractExpression exp)
        {
            throw new NotImplementedException();
        }
        // *
        public virtual T Visit(DbMultiplyExpression exp)
        {
            throw new NotImplementedException();
        }
        // /
        public virtual T Visit(DbDivideExpression exp)
        {
            throw new NotImplementedException();
        }
        // %
        public virtual T Visit(DbModuloExpression exp)
        {
            throw new NotImplementedException();
        }

        public virtual T Visit(DbNegateExpression exp)
        {
            throw new NotImplementedException();
        }

        // <
        public virtual T Visit(DbLessThanExpression exp)
        {
            throw new NotImplementedException();
        }
        // <=
        public virtual T Visit(DbLessThanOrEqualExpression exp)
        {
            throw new NotImplementedException();
        }
        // >
        public virtual T Visit(DbGreaterThanExpression exp)
        {
            throw new NotImplementedException();
        }
        // >=
        public virtual T Visit(DbGreaterThanOrEqualExpression exp)
        {
            throw new NotImplementedException();
        }
        public virtual T Visit(DbBitAndExpression exp)
        {
            throw new NotImplementedException();
        }
        public virtual T Visit(DbAndExpression exp)
        {
            throw new NotImplementedException();
        }
        public virtual T Visit(DbBitOrExpression exp)
        {
            throw new NotImplementedException();
        }
        public virtual T Visit(DbOrExpression exp)
        {
            throw new NotImplementedException();
        }
        public virtual T Visit(DbConstantExpression exp)
        {
            throw new NotImplementedException();
        }
        public virtual T Visit(DbMemberExpression exp)
        {
            throw new NotImplementedException();
        }
        public virtual T Visit(DbNotExpression exp)
        {
            throw new NotImplementedException();
        }
        public virtual T Visit(DbConvertExpression exp)
        {
            throw new NotImplementedException();
        }
        public virtual T Visit(DbCoalesceExpression exp)
        {
            throw new NotImplementedException();
        }
        public virtual T Visit(DbCaseWhenExpression exp)
        {
            throw new NotImplementedException();
        }
        public virtual T Visit(DbMethodCallExpression exp)
        {
            throw new NotImplementedException();
        }

        public virtual T Visit(DbTableExpression exp)
        {
            throw new NotImplementedException();
        }
        public virtual T Visit(DbColumnAccessExpression exp)
        {
            throw new NotImplementedException();
        }

        public virtual T Visit(DbParameterExpression exp)
        {
            throw new NotImplementedException();
        }
        public virtual T Visit(DbSubQueryExpression exp)
        {
            throw new NotImplementedException();
        }
        public virtual T Visit(DbSqlQueryExpression exp)
        {
            throw new NotImplementedException();
        }
        public virtual T Visit(DbFromTableExpression exp)
        {
            throw new NotImplementedException();
        }
        public virtual T Visit(DbJoinTableExpression exp)
        {
            throw new NotImplementedException();
        }
        public virtual T Visit(DbAggregateExpression exp)
        {
            throw new NotImplementedException();
        }

        public virtual T Visit(DbInsertExpression exp)
        {
            throw new NotImplementedException();
        }
        public virtual T Visit(DbUpdateExpression exp)
        {
            throw new NotImplementedException();
        }
        public virtual T Visit(DbDeleteExpression exp)
        {
            throw new NotImplementedException();
        }

        public virtual T Visit(DbExistsExpression exp)
        {
            throw new NotImplementedException();
        }
    }
}
