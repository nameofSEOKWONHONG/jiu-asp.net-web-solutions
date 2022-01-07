namespace Chloe.DbExpressions
{
    public class DbInsertExpression : DbExpression
    {
        public DbInsertExpression(DbTable table)
            : base(DbExpressionType.Insert, PublicConstants.TypeOfVoid)
        {
            PublicHelper.CheckNull(table);

            this.Table = table;
        }

        public DbTable Table { get; private set; }
        public Dictionary<DbColumn, DbExpression> InsertColumns { get; private set; } = new Dictionary<DbColumn, DbExpression>();
        public List<DbColumn> Returns { get; private set; } = new List<DbColumn>();
        public override T Accept<T>(DbExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
