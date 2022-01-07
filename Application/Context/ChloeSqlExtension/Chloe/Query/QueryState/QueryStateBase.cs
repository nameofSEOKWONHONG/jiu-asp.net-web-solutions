using Chloe.DbExpressions;
using Chloe.Query.Mapping;
using Chloe.Query.QueryExpressions;
using Chloe.Query.Visitors;
using Chloe.Utility;
using System.Linq.Expressions;

namespace Chloe.Query.QueryState
{
    abstract class QueryStateBase : IQueryState
    {
        QueryModel _queryModel;
        protected QueryStateBase(QueryModel queryModel)
        {
            this._queryModel = queryModel;
        }

        public QueryModel QueryModel { get { return this._queryModel; } }

        public virtual IQueryState Accept(WhereExpression exp)
        {
            ScopeParameterDictionary scopeParameters = this._queryModel.ScopeParameters.Clone(exp.Predicate.Parameters[0], this._queryModel.ResultModel);

            DbExpression whereCondition = FilterPredicateParser.Parse(exp.Predicate, scopeParameters, this._queryModel.ScopeTables);
            this._queryModel.AppendCondition(whereCondition);

            return this;
        }
        public virtual IQueryState Accept(OrderExpression exp)
        {
            if (exp.NodeType == QueryExpressionType.OrderBy || exp.NodeType == QueryExpressionType.OrderByDesc)
                this._queryModel.Orderings.Clear();

            DbOrdering ordering = ParseOrderExpression(exp);

            if (this._queryModel.InheritOrderings)
            {
                this._queryModel.Orderings.Clear();
                this._queryModel.InheritOrderings = false;
            }

            this._queryModel.Orderings.Add(ordering);

            return this;
        }
        public virtual IQueryState Accept(SelectExpression exp)
        {
            QueryModel result = this.CreateNewQueryModel(exp.Selector);
            return this.CreateQueryState(result);
        }
        public virtual IQueryState Accept(SkipExpression exp)
        {
            SkipQueryState state = new SkipQueryState(this.QueryModel, exp.Count);
            return state;
        }
        public virtual IQueryState Accept(TakeExpression exp)
        {
            TakeQueryState state = new TakeQueryState(this.QueryModel, exp.Count);
            return state;
        }
        public virtual IQueryState Accept(AggregateQueryExpression exp)
        {
            List<DbExpression> dbArguments = new List<DbExpression>(exp.Arguments.Count);
            foreach (Expression argument in exp.Arguments)
            {
                var arg = (LambdaExpression)argument;
                ScopeParameterDictionary scopeParameters = this._queryModel.ScopeParameters.Clone(arg.Parameters[0], this._queryModel.ResultModel);

                var dbArgument = GeneralExpressionParser.Parse(arg, scopeParameters, this._queryModel.ScopeTables);
                dbArguments.Add(dbArgument);
            }

            DbAggregateExpression dbAggregateExp = new DbAggregateExpression(exp.ElementType, exp.Method, dbArguments);
            PrimitiveObjectModel resultModel = new PrimitiveObjectModel(exp.ElementType, dbAggregateExp);

            QueryModel queryModel = new QueryModel(this._queryModel.ScopeParameters, this._queryModel.ScopeTables, this._queryModel.IgnoreFilters);

            queryModel.ResultModel = resultModel;
            queryModel.FromTable = this._queryModel.FromTable;
            queryModel.AppendCondition(this._queryModel.Condition);

            AggregateQueryState state = new AggregateQueryState(queryModel);
            return state;
        }
        public virtual IQueryState Accept(GroupingQueryExpression exp)
        {
            foreach (LambdaExpression item in exp.GroupKeySelectors)
            {
                var keySelector = (LambdaExpression)item;
                ScopeParameterDictionary scopeParameters = this._queryModel.ScopeParameters.Clone(keySelector.Parameters[0], this._queryModel.ResultModel);

                this._queryModel.GroupSegments.AddRange(GroupKeySelectorParser.Parse(keySelector, scopeParameters, this._queryModel.ScopeTables));
            }

            foreach (LambdaExpression havingPredicate in exp.HavingPredicates)
            {
                ScopeParameterDictionary scopeParameters = this._queryModel.ScopeParameters.Clone(havingPredicate.Parameters[0], this._queryModel.ResultModel);

                var havingCondition = FilterPredicateParser.Parse(havingPredicate, scopeParameters, this._queryModel.ScopeTables);
                this._queryModel.AppendHavingCondition(havingCondition);
            }

            if (exp.Orderings.Count > 0)
            {
                this._queryModel.Orderings.Clear();
                this._queryModel.InheritOrderings = false;

                for (int i = 0; i < exp.Orderings.Count; i++)
                {
                    GroupingQueryOrdering groupOrdering = exp.Orderings[i];

                    ScopeParameterDictionary scopeParameters = this._queryModel.ScopeParameters.Clone(groupOrdering.KeySelector.Parameters[0], this._queryModel.ResultModel);

                    DbExpression orderingDbExp = GeneralExpressionParser.Parse(groupOrdering.KeySelector, scopeParameters, this._queryModel.ScopeTables);

                    DbOrdering ordering = new DbOrdering(orderingDbExp, groupOrdering.OrderType);
                    this._queryModel.Orderings.Add(ordering);
                }
            }

            QueryModel newQueryModel = this.CreateNewQueryModel(exp.Selector);
            return new GroupingQueryState(newQueryModel);
        }
        public virtual IQueryState Accept(DistinctExpression exp)
        {
            DistinctQueryState state = new DistinctQueryState(this.QueryModel);
            return state;
        }
        public virtual IQueryState Accept(IncludeExpression exp)
        {
            throw new NotSupportedException("Cannot call 'Include' method now.");
        }
        public virtual IQueryState Accept(IgnoreAllFiltersExpression exp)
        {
            throw new NotSupportedException("Cannot call 'IgnoreAllFilters' method now.");
        }

        public virtual QueryModel CreateNewQueryModel(LambdaExpression selector)
        {
            QueryModel newQueryModel = this._queryModel.Clone();

            ComplexObjectModel complexObjectModel = this._queryModel.ResultModel as ComplexObjectModel;
            if (complexObjectModel != null)
                complexObjectModel.SetupFilters(this._queryModel.IgnoreFilters);

            ScopeParameterDictionary scopeParameters = this._queryModel.ScopeParameters.Clone(selector.Parameters[0], this._queryModel.ResultModel);
            IObjectModel newResultModel = SelectorResolver.Resolve(selector, scopeParameters, this._queryModel.ScopeTables);
            newQueryModel.ResultModel = newResultModel;

            return newQueryModel;
        }
        public virtual IQueryState CreateQueryState(QueryModel result)
        {
            return new GeneralQueryState(result);
        }

        public virtual MappingData GenerateMappingData()
        {
            MappingData data = new MappingData();

            ComplexObjectModel complexObjectModel = this._queryModel.ResultModel as ComplexObjectModel;
            if (complexObjectModel != null)
            {
                complexObjectModel.SetupCollection(this._queryModel);
                complexObjectModel.SetupFilters(this._queryModel.IgnoreFilters);
            }

            DbSqlQueryExpression sqlQuery = this.CreateSqlQuery();

            var objectActivatorCreator = this._queryModel.ResultModel.GenarateObjectActivatorCreator(sqlQuery);
            objectActivatorCreator.IsRoot = true;

            data.SqlQuery = sqlQuery;
            data.ObjectActivatorCreator = objectActivatorCreator;

            return data;
        }

        public virtual GeneralQueryState AsSubQueryState()
        {
            DbSqlQueryExpression sqlQuery = this.CreateSqlQuery();
            DbSubQueryExpression subQuery = new DbSubQueryExpression(sqlQuery);

            QueryModel newQueryModel = new QueryModel(this._queryModel.ScopeParameters, this._queryModel.ScopeTables, this._queryModel.IgnoreFilters);

            DbTableSegment tableSeg = new DbTableSegment(subQuery, newQueryModel.GenerateUniqueTableAlias(), LockType.Unspecified);
            DbFromTableExpression fromTable = new DbFromTableExpression(tableSeg);

            newQueryModel.FromTable = fromTable;

            DbTable aliasTable = new DbTable(tableSeg.Alias);

            //TODO 根据旧的生成新 ResultModel
            IObjectModel newResultModel = this.QueryModel.ResultModel.ToNewObjectModel(sqlQuery, aliasTable, fromTable, newQueryModel.IgnoreFilters);
            newQueryModel.ResultModel = newResultModel;

            //得将 subQuery.SqlQuery.Orders 告诉 以下创建的 result
            //将 orderPart 传递下去
            for (int i = 0; i < this.QueryModel.Orderings.Count; i++)
            {
                DbOrdering ordering = this.QueryModel.Orderings[i];
                DbExpression orderingExp = ordering.Expression;

                string alias = null;

                DbColumnSegment columnExpression = sqlQuery.ColumnSegments.Find(a => DbExpressionEqualityComparer.EqualsCompare(orderingExp, a.Body));

                // 对于重复的则不需要往 sqlQuery.Columns 重复添加了
                if (columnExpression != null)
                {
                    alias = columnExpression.Alias;
                }
                else
                {
                    alias = Utils.GenerateUniqueColumnAlias(sqlQuery);
                    DbColumnSegment columnSeg = new DbColumnSegment(orderingExp, alias);
                    sqlQuery.ColumnSegments.Add(columnSeg);
                }

                DbColumnAccessExpression columnAccessExpression = new DbColumnAccessExpression(aliasTable, DbColumn.MakeColumn(orderingExp, alias));
                newQueryModel.Orderings.Add(new DbOrdering(columnAccessExpression, ordering.OrderType));
            }

            newQueryModel.InheritOrderings = true;

            GeneralQueryState queryState = new GeneralQueryState(newQueryModel);
            return queryState;
        }
        public virtual DbSqlQueryExpression CreateSqlQuery()
        {
            DbSqlQueryExpression sqlQuery = this._queryModel.CreateSqlQuery();
            return sqlQuery;
        }

        protected DbOrdering ParseOrderExpression(OrderExpression orderExp)
        {
            ScopeParameterDictionary scopeParameters = this._queryModel.ScopeParameters.Clone(orderExp.KeySelector.Parameters[0], this._queryModel.ResultModel);

            DbExpression dbExpression = GeneralExpressionParser.Parse(orderExp.KeySelector, scopeParameters, this._queryModel.ScopeTables);
            DbOrderType orderType;
            if (orderExp.NodeType == QueryExpressionType.OrderBy || orderExp.NodeType == QueryExpressionType.ThenBy)
            {
                orderType = DbOrderType.Asc;
            }
            else if (orderExp.NodeType == QueryExpressionType.OrderByDesc || orderExp.NodeType == QueryExpressionType.ThenByDesc)
            {
                orderType = DbOrderType.Desc;
            }
            else
                throw new NotSupportedException(orderExp.NodeType.ToString());

            DbOrdering ordering = new DbOrdering(dbExpression, orderType);

            return ordering;
        }

        public virtual QueryModel ToFromQueryModel()
        {
            QueryModel newQueryModel = new QueryModel(this._queryModel.ScopeParameters, this._queryModel.ScopeTables, this._queryModel.IgnoreFilters);

            string alias = newQueryModel.GenerateUniqueTableAlias(UtilConstants.DefaultTableAlias);
            DbSqlQueryExpression sqlQuery = this.CreateSqlQuery();
            DbSubQueryExpression subQuery = new DbSubQueryExpression(sqlQuery);

            DbTableSegment tableSeg = new DbTableSegment(subQuery, alias, LockType.Unspecified);
            DbFromTableExpression fromTable = new DbFromTableExpression(tableSeg);

            DbTable aliasTable = new DbTable(tableSeg.Alias);
            IObjectModel newModel = this.QueryModel.ResultModel.ToNewObjectModel(sqlQuery, aliasTable, fromTable, newQueryModel.IgnoreFilters);

            newQueryModel.FromTable = fromTable;
            newQueryModel.ResultModel = newModel;
            return newQueryModel;
        }

        public virtual JoinQueryResult ToJoinQueryResult(JoinType joinType, LambdaExpression conditionExpression, ScopeParameterDictionary scopeParameters, StringSet scopeTables, Func<string, string> tableAliasGenerator)
        {
            DbSqlQueryExpression sqlQuery = this.CreateSqlQuery();
            DbSubQueryExpression subQuery = new DbSubQueryExpression(sqlQuery);

            string alias = tableAliasGenerator(UtilConstants.DefaultTableAlias);
            DbTableSegment tableSeg = new DbTableSegment(subQuery, alias, LockType.Unspecified);
            DbJoinTableExpression joinTable = new DbJoinTableExpression(joinType.AsDbJoinType(), tableSeg);

            DbTable aliasTable = new DbTable(tableSeg.Alias);
            IObjectModel newModel = this.QueryModel.ResultModel.ToNewObjectModel(sqlQuery, aliasTable, joinTable, this.QueryModel.IgnoreFilters);

            scopeParameters[conditionExpression.Parameters[conditionExpression.Parameters.Count - 1]] = newModel;

            DbExpression condition = GeneralExpressionParser.Parse(conditionExpression, scopeParameters, scopeTables);
            joinTable.Condition = condition;

            JoinQueryResult result = new JoinQueryResult();
            result.ResultModel = newModel;
            result.JoinTable = joinTable;
            return result;
        }
    }
}
