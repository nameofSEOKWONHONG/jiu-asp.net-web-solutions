using Chloe.InternalExtensions;

namespace Chloe.DbExpressions
{
    public class DbJoinTableExpression : DbMainTableExpression
    {
        DbJoinType _joinType;

        public DbJoinTableExpression(DbJoinType joinType, DbTableSegment table)
          : this(joinType, table, null)
        {
        }
        public DbJoinTableExpression(DbJoinType joinType, DbTableSegment table, DbExpression condition)
            : base(DbExpressionType.JoinTable, table)
        {
            this._joinType = joinType;
            this.Condition = condition;
        }

        public DbJoinType JoinType { get { return this._joinType; } }
        public DbExpression Condition { get; set; }
        public override T Accept<T>(DbExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        internal void AppendCondition(DbExpression condition)
        {
            this.Condition = this.Condition.And(condition);
        }
    }
}
