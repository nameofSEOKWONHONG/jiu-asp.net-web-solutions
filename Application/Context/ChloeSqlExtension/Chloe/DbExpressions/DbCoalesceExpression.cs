namespace Chloe.DbExpressions
{
    public class DbCoalesceExpression : DbExpression
    {
        public DbCoalesceExpression(DbExpression checkExpression, DbExpression replacementValue)
            : base(DbExpressionType.Coalesce, replacementValue.Type)
        {
            PublicHelper.CheckNull(checkExpression);
            PublicHelper.CheckNull(replacementValue);

            this.CheckExpression = checkExpression;
            this.ReplacementValue = replacementValue;
        }

        public DbExpression CheckExpression { get; private set; }
        public DbExpression ReplacementValue { get; private set; }

        public override T Accept<T>(DbExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
