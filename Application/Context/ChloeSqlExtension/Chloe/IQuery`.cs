using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Chloe
{
    public interface IQuery<T> : IQuery
    {
        IQuery<T> AsTracking();
        IEnumerable<T> AsEnumerable();
        IQuery<TResult> Select<TResult>(Expression<Func<T, TResult>> selector);

        IQuery<T> IncludeAll();
        IIncludableQuery<T, TProperty> Include<TProperty>(Expression<Func<T, TProperty>> p);
        IIncludableQuery<T, TCollectionItem> IncludeMany<TCollectionItem>(Expression<Func<T, IEnumerable<TCollectionItem>>> p);

        IQuery<T> Where(Expression<Func<T, bool>> predicate);
        IOrderedQuery<T> OrderBy<K>(Expression<Func<T, K>> keySelector);
        IOrderedQuery<T> OrderByDesc<K>(Expression<Func<T, K>> keySelector);
        IQuery<T> Skip(int count);
        IQuery<T> Take(int count);
        IQuery<T> TakePage(int pageNumber, int pageSize);

        IGroupingQuery<T> GroupBy<K>(Expression<Func<T, K>> keySelector);
        IQuery<T> Distinct();
        IQuery<T> IgnoreAllFilters();

        IJoinQuery<T, TOther> Join<TOther>(JoinType joinType, Expression<Func<T, TOther, bool>> on);
        IJoinQuery<T, TOther> Join<TOther>(IQuery<TOther> q, JoinType joinType, Expression<Func<T, TOther, bool>> on);

        IJoinQuery<T, TOther> InnerJoin<TOther>(Expression<Func<T, TOther, bool>> on);
        IJoinQuery<T, TOther> LeftJoin<TOther>(Expression<Func<T, TOther, bool>> on);
        IJoinQuery<T, TOther> RightJoin<TOther>(Expression<Func<T, TOther, bool>> on);
        IJoinQuery<T, TOther> FullJoin<TOther>(Expression<Func<T, TOther, bool>> on);

        IJoinQuery<T, TOther> InnerJoin<TOther>(IQuery<TOther> q, Expression<Func<T, TOther, bool>> on);
        IJoinQuery<T, TOther> LeftJoin<TOther>(IQuery<TOther> q, Expression<Func<T, TOther, bool>> on);
        IJoinQuery<T, TOther> RightJoin<TOther>(IQuery<TOther> q, Expression<Func<T, TOther, bool>> on);
        IJoinQuery<T, TOther> FullJoin<TOther>(IQuery<TOther> q, Expression<Func<T, TOther, bool>> on);

        T First();
        Task<T> FirstAsync();

        T First(Expression<Func<T, bool>> predicate);
        Task<T> FirstAsync(Expression<Func<T, bool>> predicate);

        T FirstOrDefault();
        Task<T> FirstOrDefaultAsync();

        T FirstOrDefault(Expression<Func<T, bool>> predicate);
        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

        List<T> ToList();
        Task<List<T>> ToListAsync();

        bool Any();
        Task<bool> AnyAsync();

        bool Any(Expression<Func<T, bool>> predicate);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

        int Count();
        Task<int> CountAsync();

        long LongCount();
        Task<long> LongCountAsync();

        /// <summary>
        /// 求最大值。考虑到满足条件的数据个数为零的情况，为避免报错，可在 lambda 中将相应字段强转成可空类型，如 query.Max(a => (double?)a.Price)。
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="selector"></param>
        /// <returns></returns>
        TResult Max<TResult>(Expression<Func<T, TResult>> selector);
        Task<TResult> MaxAsync<TResult>(Expression<Func<T, TResult>> selector);

        /// <summary>
        /// 求最小值。考虑到满足条件的数据个数为零的情况，为避免报错，可在 lambda 中将相应字段强转成可空类型，如 query.Min(a => (double?)a.Price)。
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="selector"></param>
        /// <returns></returns>
        TResult Min<TResult>(Expression<Func<T, TResult>> selector);
        Task<TResult> MinAsync<TResult>(Expression<Func<T, TResult>> selector);


        int Sum(Expression<Func<T, int>> selector);
        Task<int> SumAsync(Expression<Func<T, int>> selector);

        int? Sum(Expression<Func<T, int?>> selector);
        Task<int?> SumAsync(Expression<Func<T, int?>> selector);

        long Sum(Expression<Func<T, long>> selector);
        Task<long> SumAsync(Expression<Func<T, long>> selector);

        long? Sum(Expression<Func<T, long?>> selector);
        Task<long?> SumAsync(Expression<Func<T, long?>> selector);

        decimal Sum(Expression<Func<T, decimal>> selector);
        Task<decimal> SumAsync(Expression<Func<T, decimal>> selector);

        decimal? Sum(Expression<Func<T, decimal?>> selector);
        Task<decimal?> SumAsync(Expression<Func<T, decimal?>> selector);

        double Sum(Expression<Func<T, double>> selector);
        Task<double> SumAsync(Expression<Func<T, double>> selector);

        double? Sum(Expression<Func<T, double?>> selector);
        Task<double?> SumAsync(Expression<Func<T, double?>> selector);

        float Sum(Expression<Func<T, float>> selector);
        Task<float> SumAsync(Expression<Func<T, float>> selector);

        float? Sum(Expression<Func<T, float?>> selector);
        Task<float?> SumAsync(Expression<Func<T, float?>> selector);


        double? Average(Expression<Func<T, int>> selector);
        Task<double?> AverageAsync(Expression<Func<T, int>> selector);

        double? Average(Expression<Func<T, int?>> selector);
        Task<double?> AverageAsync(Expression<Func<T, int?>> selector);

        double? Average(Expression<Func<T, long>> selector);
        Task<double?> AverageAsync(Expression<Func<T, long>> selector);

        double? Average(Expression<Func<T, long?>> selector);
        Task<double?> AverageAsync(Expression<Func<T, long?>> selector);

        decimal? Average(Expression<Func<T, decimal>> selector);
        Task<decimal?> AverageAsync(Expression<Func<T, decimal>> selector);

        decimal? Average(Expression<Func<T, decimal?>> selector);
        Task<decimal?> AverageAsync(Expression<Func<T, decimal?>> selector);

        double? Average(Expression<Func<T, double>> selector);
        Task<double?> AverageAsync(Expression<Func<T, double>> selector);

        double? Average(Expression<Func<T, double?>> selector);
        Task<double?> AverageAsync(Expression<Func<T, double?>> selector);

        float? Average(Expression<Func<T, float>> selector);
        Task<float?> AverageAsync(Expression<Func<T, float>> selector);

        float? Average(Expression<Func<T, float?>> selector);
        Task<float?> AverageAsync(Expression<Func<T, float?>> selector);
    }
}
