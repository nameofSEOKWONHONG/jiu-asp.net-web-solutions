using Chloe.DbExpressions;
using Chloe.Query.QueryExpressions;
using Chloe.Query.QueryState;
using Chloe.Utility;
using System.Linq.Expressions;

namespace Chloe.Query.Visitors
{
    class QueryExpressionResolver : QueryExpressionVisitor<IQueryState>
    {
        ScopeParameterDictionary _scopeParameters;
        StringSet _scopeTables;
        QueryExpressionResolver(ScopeParameterDictionary scopeParameters, StringSet scopeTables)
        {
            this._scopeParameters = scopeParameters;
            this._scopeTables = scopeTables;
        }
        public static IQueryState Resolve(QueryExpression queryExpression, ScopeParameterDictionary scopeParameters, StringSet scopeTables)
        {
            QueryExpressionResolver resolver = new QueryExpressionResolver(scopeParameters, scopeTables);
            return queryExpression.Accept(resolver);
        }

        public override IQueryState Visit(RootQueryExpression exp)
        {
            IQueryState queryState = new RootQueryState(exp, this._scopeParameters, this._scopeTables);
            return queryState;
        }
        public override IQueryState Visit(WhereExpression exp)
        {
            IQueryState prevState = exp.PrevExpression.Accept(this);
            IQueryState state = prevState.Accept(exp);
            return state;
        }
        public override IQueryState Visit(OrderExpression exp)
        {
            IQueryState prevState = exp.PrevExpression.Accept(this);
            IQueryState state = prevState.Accept(exp);
            return state;
        }
        public override IQueryState Visit(SelectExpression exp)
        {
            IQueryState prevState = exp.PrevExpression.Accept(this);
            IQueryState state = prevState.Accept(exp);
            return state;
        }
        public override IQueryState Visit(SkipExpression exp)
        {
            IQueryState prevState = exp.PrevExpression.Accept(this);
            IQueryState state = prevState.Accept(exp);
            return state;
        }
        public override IQueryState Visit(TakeExpression exp)
        {
            IQueryState prevState = exp.PrevExpression.Accept(this);
            IQueryState state = prevState.Accept(exp);
            return state;
        }
        public override IQueryState Visit(AggregateQueryExpression exp)
        {
            IQueryState prevState = exp.PrevExpression.Accept(this);
            IQueryState state = prevState.Accept(exp);
            return state;
        }
        public override IQueryState Visit(JoinQueryExpression exp)
        {
            IQueryState qs = QueryExpressionResolver.Resolve(exp.PrevExpression, this._scopeParameters, this._scopeTables);

            QueryModel queryModel = qs.ToFromQueryModel();

            List<IObjectModel> modelList = new List<IObjectModel>();
            modelList.Add(queryModel.ResultModel);

            foreach (JoinQueryInfo joinQueryInfo in exp.JoinedQueries)
            {
                ScopeParameterDictionary scopeParameters = queryModel.ScopeParameters.Clone(queryModel.ScopeParameters.Count + modelList.Count);
                for (int i = 0; i < modelList.Count; i++)
                {
                    ParameterExpression p = joinQueryInfo.Condition.Parameters[i];
                    scopeParameters[p] = modelList[i];
                }

                JoinQueryResult joinQueryResult = JoinQueryExpressionResolver.Resolve(joinQueryInfo, queryModel, scopeParameters);

                var nullChecking = DbExpression.CaseWhen(new DbCaseWhenExpression.WhenThenExpressionPair(joinQueryResult.JoinTable.Condition, DbConstantExpression.One), DbConstantExpression.Null, DbConstantExpression.One.Type);

                if (joinQueryInfo.JoinType == JoinType.LeftJoin)
                {
                    joinQueryResult.ResultModel.SetNullChecking(nullChecking);
                }
                else if (joinQueryInfo.JoinType == JoinType.RightJoin)
                {
                    foreach (IObjectModel item in modelList)
                    {
                        item.SetNullChecking(nullChecking);
                    }
                }
                else if (joinQueryInfo.JoinType == JoinType.FullJoin)
                {
                    joinQueryResult.ResultModel.SetNullChecking(nullChecking);
                    foreach (IObjectModel item in modelList)
                    {
                        item.SetNullChecking(nullChecking);
                    }
                }

                queryModel.FromTable.JoinTables.Add(joinQueryResult.JoinTable);
                modelList.Add(joinQueryResult.ResultModel);
            }

            ScopeParameterDictionary scopeParameters1 = queryModel.ScopeParameters.Clone(queryModel.ScopeParameters.Count + modelList.Count);
            for (int i = 0; i < modelList.Count; i++)
            {
                ParameterExpression p = exp.Selector.Parameters[i];
                scopeParameters1[p] = modelList[i];
            }
            IObjectModel model = SelectorResolver.Resolve(exp.Selector, scopeParameters1, queryModel.ScopeTables);
            queryModel.ResultModel = model;

            GeneralQueryState queryState = new GeneralQueryState(queryModel);
            return queryState;
        }
        public override IQueryState Visit(GroupingQueryExpression exp)
        {
            IQueryState prevState = exp.PrevExpression.Accept(this);
            IQueryState state = prevState.Accept(exp);
            return state;
        }
        public override IQueryState Visit(DistinctExpression exp)
        {
            IQueryState prevState = exp.PrevExpression.Accept(this);
            IQueryState state = prevState.Accept(exp);
            return state;
        }
        public override IQueryState Visit(IncludeExpression exp)
        {
            IQueryState prevState = exp.PrevExpression.Accept(this);
            IQueryState state = prevState.Accept(exp);
            return state;
        }
        public override IQueryState Visit(IgnoreAllFiltersExpression exp)
        {
            IQueryState prevState = exp.PrevExpression.Accept(this);
            IQueryState state = prevState.Accept(exp);
            return state;
        }
    }
}
