using Chloe.DbExpressions;
using Chloe.Query.QueryExpressions;

namespace Chloe.Query.QueryState
{
    internal sealed class TakeQueryState : SubQueryState
    {
        int _count;
        public TakeQueryState(QueryModel queryModel, int count)
            : base(queryModel)
        {
            this.Count = count;
        }

        public int Count
        {
            get
            {
                return this._count;
            }
            set
            {
                this.CheckInputCount(value);
                this._count = value;
            }
        }

        void CheckInputCount(int count)
        {
            if (count < 0)
            {
                throw new ArgumentException("The take count could not less than 0.");
            }
        }

        public override IQueryState Accept(TakeExpression exp)
        {
            if (exp.Count < this.Count)
                this.Count = exp.Count;

            return this;
        }

        public override IQueryState CreateQueryState(QueryModel result)
        {
            return new TakeQueryState(result, this.Count);
        }

        public override DbSqlQueryExpression CreateSqlQuery()
        {
            DbSqlQueryExpression sqlQuery = base.CreateSqlQuery();

            sqlQuery.TakeCount = this.Count;
            sqlQuery.SkipCount = null;

            return sqlQuery;
        }
    }
}
