﻿using System.Linq.Expressions;

namespace Chloe.Query.QueryExpressions
{
    class JoinQueryExpression : QueryExpression
    {
        List<JoinQueryInfo> _joinedQueries;
        LambdaExpression _selector;
        public JoinQueryExpression(Type elementType, QueryExpression prevExpression, List<JoinQueryInfo> joinedQueries, LambdaExpression selector)
            : base(QueryExpressionType.JoinQuery, elementType, prevExpression)
        {
            this._joinedQueries = new List<JoinQueryInfo>(joinedQueries.Count);
            this._joinedQueries.AddRange(joinedQueries);
            this._selector = selector;
        }

        public List<JoinQueryInfo> JoinedQueries { get { return this._joinedQueries; } }
        public LambdaExpression Selector { get { return this._selector; } }

        public override T Accept<T>(QueryExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
