using Chloe.DbExpressions;
using Chloe.Query.QueryExpressions;
using Chloe.Query.QueryState;
using System.Linq.Expressions;

namespace Chloe.Query.Visitors
{
    class JoinQueryExpressionResolver : QueryExpressionVisitor<JoinQueryResult>
    {
        QueryModel _queryModel;
        JoinType _joinType;

        LambdaExpression _conditionExpression;
        ScopeParameterDictionary _scopeParameters;

        JoinQueryExpressionResolver(QueryModel queryModel, JoinType joinType, LambdaExpression conditionExpression, ScopeParameterDictionary scopeParameters)
        {
            this._queryModel = queryModel;
            this._joinType = joinType;
            this._conditionExpression = conditionExpression;
            this._scopeParameters = scopeParameters;
        }

        public static JoinQueryResult Resolve(JoinQueryInfo joinQueryInfo, QueryModel queryModel, ScopeParameterDictionary scopeParameters)
        {
            JoinQueryExpressionResolver resolver = new JoinQueryExpressionResolver(queryModel, joinQueryInfo.JoinType, joinQueryInfo.Condition, scopeParameters);
            return joinQueryInfo.Query.QueryExpression.Accept(resolver);
        }

        public override JoinQueryResult Visit(RootQueryExpression exp)
        {
            IQueryState queryState = new RootQueryState(exp, this._scopeParameters, this._queryModel.ScopeTables, a => { return this._queryModel.GenerateUniqueTableAlias(a); });
            JoinQueryResult result = queryState.ToJoinQueryResult(this._joinType, this._conditionExpression, this._scopeParameters, this._queryModel.ScopeTables, null);
            return result;
        }
        public override JoinQueryResult Visit(WhereExpression exp)
        {
            JoinQueryResult ret = this.Visit(exp);
            return ret;
        }
        public override JoinQueryResult Visit(OrderExpression exp)
        {
            JoinQueryResult ret = this.Visit(exp);
            return ret;
        }
        public override JoinQueryResult Visit(SelectExpression exp)
        {
            JoinQueryResult ret = this.Visit(exp);
            return ret;
        }
        public override JoinQueryResult Visit(SkipExpression exp)
        {
            JoinQueryResult ret = this.Visit(exp);
            return ret;
        }
        public override JoinQueryResult Visit(TakeExpression exp)
        {
            JoinQueryResult ret = this.Visit(exp);
            return ret;
        }
        public override JoinQueryResult Visit(AggregateQueryExpression exp)
        {
            JoinQueryResult ret = this.Visit(exp);
            return ret;
        }
        public override JoinQueryResult Visit(JoinQueryExpression exp)
        {
            JoinQueryResult ret = this.Visit(exp);
            return ret;
        }
        public override JoinQueryResult Visit(GroupingQueryExpression exp)
        {
            JoinQueryResult ret = this.Visit(exp);
            return ret;
        }
        public override JoinQueryResult Visit(DistinctExpression exp)
        {
            JoinQueryResult ret = this.Visit(exp);
            return ret;
        }
        public override JoinQueryResult Visit(IncludeExpression exp)
        {
            JoinQueryResult ret = this.Visit(exp);
            return ret;
        }

        JoinQueryResult Visit(QueryExpression exp)
        {
            IQueryState state = QueryExpressionResolver.Resolve(exp, this._scopeParameters, this._queryModel.ScopeTables);
            JoinQueryResult ret = state.ToJoinQueryResult(this._joinType, this._conditionExpression, this._scopeParameters, this._queryModel.ScopeTables, a => { return this._queryModel.GenerateUniqueTableAlias(a); });
            return ret;
        }
        static DbTableSegment CreateTableSegment(DbTable table, string alias, LockType @lock)
        {
            DbTableExpression tableExp = new DbTableExpression(table);
            DbTableSegment tableSeg = new DbTableSegment(tableExp, alias, @lock);
            return tableSeg;
        }
    }
}
