﻿namespace Chloe.DbExpressions
{
    public class DbBitOrExpression : DbBinaryExpression
    {
        public DbBitOrExpression(Type type, DbExpression left, DbExpression right)
            : base(DbExpressionType.BitOr, type, left, right)
        {
        }

        public override T Accept<T>(DbExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

}
