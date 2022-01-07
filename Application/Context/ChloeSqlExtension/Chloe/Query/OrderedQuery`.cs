using Chloe.Query.QueryExpressions;
using System.Linq.Expressions;

namespace Chloe.Query
{
    class OrderedQuery<T> : Query<T>, IOrderedQuery<T>
    {
        public OrderedQuery(DbContext dbContext, QueryExpression exp, bool trackEntity)
            : base(dbContext, exp, trackEntity)
        {

        }
        public IOrderedQuery<T> ThenBy<K>(Expression<Func<T, K>> keySelector)
        {
            OrderExpression e = new OrderExpression(typeof(T), this.QueryExpression, QueryExpressionType.ThenBy, keySelector);
            return new OrderedQuery<T>(this.DbContext, e, this._trackEntity);
        }
        public IOrderedQuery<T> ThenByDesc<K>(Expression<Func<T, K>> keySelector)
        {
            OrderExpression e = new OrderExpression(typeof(T), this.QueryExpression, QueryExpressionType.ThenByDesc, keySelector);
            return new OrderedQuery<T>(this.DbContext, e, this._trackEntity);
        }
    }
}
