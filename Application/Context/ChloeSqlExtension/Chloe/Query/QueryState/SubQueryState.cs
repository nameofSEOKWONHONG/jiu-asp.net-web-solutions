﻿using Chloe.Query.Mapping;
using Chloe.Query.QueryExpressions;

namespace Chloe.Query.QueryState
{
    internal abstract class SubQueryState : QueryStateBase
    {
        protected SubQueryState(QueryModel queryModel)
            : base(queryModel)
        {
        }

        public override IQueryState Accept(WhereExpression exp)
        {
            IQueryState state = this.AsSubQueryState();
            return state.Accept(exp);
        }
        public override IQueryState Accept(OrderExpression exp)
        {
            IQueryState state = this.AsSubQueryState();
            return state.Accept(exp);
        }
        public override IQueryState Accept(SkipExpression exp)
        {
            GeneralQueryState subQueryState = this.AsSubQueryState();
            SkipQueryState state = new SkipQueryState(subQueryState.QueryModel, exp.Count);
            return state;
        }
        public override IQueryState Accept(TakeExpression exp)
        {
            GeneralQueryState subQueryState = this.AsSubQueryState();
            TakeQueryState state = new TakeQueryState(subQueryState.QueryModel, exp.Count);
            return state;
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
        public override IQueryState Accept(DistinctExpression exp)
        {
            IQueryState state = this.AsSubQueryState();
            return state.Accept(exp);
        }

        public override MappingData GenerateMappingData()
        {
            ComplexObjectModel complexObjectModel = this.QueryModel.ResultModel as ComplexObjectModel;

            if (complexObjectModel == null)
                return base.GenerateMappingData();

            if (complexObjectModel.HasMany())
            {
                return base.AsSubQueryState().GenerateMappingData();
            }

            return base.GenerateMappingData();
        }
    }
}
