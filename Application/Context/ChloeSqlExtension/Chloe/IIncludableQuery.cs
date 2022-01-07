using System.Linq.Expressions;

namespace Chloe
{
    public interface IIncludableQuery<TEntity, TNavigation> : IQuery<TEntity>
    {
        /// <summary>
        /// dbContext.Query&lt;City&gt;().IncludeMany(a =&gt; a.Users).AndWhere(a =&gt; a.Age &gt;= 18) --&gt; select ... from City left join User on City.Id=User.CityId and User.Age &gt;= 18
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IIncludableQuery<TEntity, TNavigation> AndWhere(Expression<Func<TNavigation, bool>> predicate);
        IIncludableQuery<TEntity, TProperty> ThenInclude<TProperty>(Expression<Func<TNavigation, TProperty>> navigationPath);
        IIncludableQuery<TEntity, TCollectionItem> ThenIncludeMany<TCollectionItem>(Expression<Func<TNavigation, IEnumerable<TCollectionItem>>> navigationPath);
    }
}
