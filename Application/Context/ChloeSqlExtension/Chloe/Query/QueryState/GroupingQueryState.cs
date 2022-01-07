﻿using Chloe.Query.QueryExpressions;

namespace Chloe.Query.QueryState
{
    class GroupingQueryState : QueryStateBase
    {
        public GroupingQueryState(QueryModel queryModel)
            : base(queryModel)
        {
        }


        public override IQueryState Accept(WhereExpression exp)
        {
            IQueryState state = this.AsSubQueryState();
            return state.Accept(exp);
        }
        public override IQueryState Accept(AggregateQueryExpression exp)
        {
            IQueryState state = this.AsSubQueryState();
            return state.Accept(exp);
        }
        public override IQueryState Accept(GroupingQueryExpression exp)
        {
            IQueryState state = this.AsSubQueryState();
            return state.Accept(exp);
        }
    }
}
