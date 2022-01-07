﻿using Chloe.DbExpressions;
using Chloe.Query.QueryExpressions;

namespace Chloe.Query.QueryState
{
    internal sealed class SkipQueryState : SubQueryState
    {
        int _count;
        public SkipQueryState(QueryModel queryModel, int count)
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
                throw new ArgumentException("The skip count could not less than 0.");
            }
        }

        public override IQueryState Accept(SkipExpression exp)
        {
            if (exp.Count < 1)
            {
                return this;
            }

            this.Count += exp.Count;

            return this;
        }
        public override IQueryState Accept(TakeExpression exp)
        {
            var state = new LimitQueryState(this.QueryModel, this.Count, exp.Count);
            return state;
        }
        public override IQueryState CreateQueryState(QueryModel result)
        {
            return new SkipQueryState(result, this.Count);
        }

        public override DbSqlQueryExpression CreateSqlQuery()
        {
            DbSqlQueryExpression sqlQuery = base.CreateSqlQuery();

            sqlQuery.TakeCount = null;
            sqlQuery.SkipCount = this.Count;

            return sqlQuery;
        }
    }
}
