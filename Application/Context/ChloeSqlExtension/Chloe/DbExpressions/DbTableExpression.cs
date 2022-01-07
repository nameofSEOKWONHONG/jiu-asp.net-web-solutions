namespace Chloe.DbExpressions
{
    public class DbTableExpression : DbExpression
    {
        DbTable _table;
        public DbTableExpression(DbTable table)
            : base(DbExpressionType.Table, PublicConstants.TypeOfVoid)
        {
            this._table = table;
        }

        public DbTable Table { get { return this._table; } }

        public override T Accept<T>(DbExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
