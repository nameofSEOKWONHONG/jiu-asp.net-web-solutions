using System.Linq.Expressions;

namespace Chloe
{
    /// <summary>
    /// Usage: joinQuery.Select((user, city) =&gt; new { User = user, City = city }).Where(a => a.User.Age &gt;= 18).OrderBy(a => a.User.Id).Select(a =&gt; new { UserId = a.User.Id, UserName = a.User.Name, CityName = a.City.Name }).TakePage(1, 20)
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public interface IJoinQuery<T1, T2>
    {
        IJoinQuery<T1, T2> Where(Expression<Func<T1, T2, bool>> predicate);

        IJoinQuery<T1, T2, T3> Join<T3>(JoinType joinType, Expression<Func<T1, T2, T3, bool>> on);
        IJoinQuery<T1, T2, T3> Join<T3>(IQuery<T3> q, JoinType joinType, Expression<Func<T1, T2, T3, bool>> on);

        IJoinQuery<T1, T2, T3> InnerJoin<T3>(Expression<Func<T1, T2, T3, bool>> on);
        IJoinQuery<T1, T2, T3> LeftJoin<T3>(Expression<Func<T1, T2, T3, bool>> on);
        IJoinQuery<T1, T2, T3> RightJoin<T3>(Expression<Func<T1, T2, T3, bool>> on);
        IJoinQuery<T1, T2, T3> FullJoin<T3>(Expression<Func<T1, T2, T3, bool>> on);

        IJoinQuery<T1, T2, T3> InnerJoin<T3>(IQuery<T3> q, Expression<Func<T1, T2, T3, bool>> on);
        IJoinQuery<T1, T2, T3> LeftJoin<T3>(IQuery<T3> q, Expression<Func<T1, T2, T3, bool>> on);
        IJoinQuery<T1, T2, T3> RightJoin<T3>(IQuery<T3> q, Expression<Func<T1, T2, T3, bool>> on);
        IJoinQuery<T1, T2, T3> FullJoin<T3>(IQuery<T3> q, Expression<Func<T1, T2, T3, bool>> on);

        /// <summary>
        /// Usage: joinQuery.Select((user, city) =&gt; new { User = user, City = city }).Where(a => a.User.Age &gt;= 18).OrderBy(a => a.User.Id).Select(a =&gt; new { UserId = a.User.Id, UserName = a.User.Name, CityName = a.City.Name }).TakePage(1, 20)
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="selector"></param>
        /// <returns></returns>
        IQuery<TResult> Select<TResult>(Expression<Func<T1, T2, TResult>> selector);
    }

    /// <summary>
    /// Usage: joinQuery.Select((user, city) =&gt; new { User = user, City = city }).Where(a => a.User.Age &gt;= 18).OrderBy(a => a.User.Id).Select(a =&gt; new { UserId = a.User.Id, UserName = a.User.Name, CityName = a.City.Name }).TakePage(1, 20)
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    public interface IJoinQuery<T1, T2, T3>
    {
        IJoinQuery<T1, T2, T3> Where(Expression<Func<T1, T2, T3, bool>> predicate);

        IJoinQuery<T1, T2, T3, T4> Join<T4>(JoinType joinType, Expression<Func<T1, T2, T3, T4, bool>> on);
        IJoinQuery<T1, T2, T3, T4> Join<T4>(IQuery<T4> q, JoinType joinType, Expression<Func<T1, T2, T3, T4, bool>> on);

        IJoinQuery<T1, T2, T3, T4> InnerJoin<T4>(Expression<Func<T1, T2, T3, T4, bool>> on);
        IJoinQuery<T1, T2, T3, T4> LeftJoin<T4>(Expression<Func<T1, T2, T3, T4, bool>> on);
        IJoinQuery<T1, T2, T3, T4> RightJoin<T4>(Expression<Func<T1, T2, T3, T4, bool>> on);
        IJoinQuery<T1, T2, T3, T4> FullJoin<T4>(Expression<Func<T1, T2, T3, T4, bool>> on);

        IJoinQuery<T1, T2, T3, T4> InnerJoin<T4>(IQuery<T4> q, Expression<Func<T1, T2, T3, T4, bool>> on);
        IJoinQuery<T1, T2, T3, T4> LeftJoin<T4>(IQuery<T4> q, Expression<Func<T1, T2, T3, T4, bool>> on);
        IJoinQuery<T1, T2, T3, T4> RightJoin<T4>(IQuery<T4> q, Expression<Func<T1, T2, T3, T4, bool>> on);
        IJoinQuery<T1, T2, T3, T4> FullJoin<T4>(IQuery<T4> q, Expression<Func<T1, T2, T3, T4, bool>> on);
        IQuery<TResult> Select<TResult>(Expression<Func<T1, T2, T3, TResult>> selector);
    }

    /// <summary>
    /// Usage: joinQuery.Select((user, city) =&gt; new { User = user, City = city }).Where(a => a.User.Age &gt;= 18).OrderBy(a => a.User.Id).Select(a =&gt; new { UserId = a.User.Id, UserName = a.User.Name, CityName = a.City.Name }).TakePage(1, 20)
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    public interface IJoinQuery<T1, T2, T3, T4>
    {
        IJoinQuery<T1, T2, T3, T4> Where(Expression<Func<T1, T2, T3, T4, bool>> predicate);

        IJoinQuery<T1, T2, T3, T4, T5> Join<T5>(JoinType joinType, Expression<Func<T1, T2, T3, T4, T5, bool>> on);
        IJoinQuery<T1, T2, T3, T4, T5> Join<T5>(IQuery<T5> q, JoinType joinType, Expression<Func<T1, T2, T3, T4, T5, bool>> on);

        IJoinQuery<T1, T2, T3, T4, T5> InnerJoin<T5>(Expression<Func<T1, T2, T3, T4, T5, bool>> on);
        IJoinQuery<T1, T2, T3, T4, T5> LeftJoin<T5>(Expression<Func<T1, T2, T3, T4, T5, bool>> on);
        IJoinQuery<T1, T2, T3, T4, T5> RightJoin<T5>(Expression<Func<T1, T2, T3, T4, T5, bool>> on);
        IJoinQuery<T1, T2, T3, T4, T5> FullJoin<T5>(Expression<Func<T1, T2, T3, T4, T5, bool>> on);

        IJoinQuery<T1, T2, T3, T4, T5> InnerJoin<T5>(IQuery<T5> q, Expression<Func<T1, T2, T3, T4, T5, bool>> on);
        IJoinQuery<T1, T2, T3, T4, T5> LeftJoin<T5>(IQuery<T5> q, Expression<Func<T1, T2, T3, T4, T5, bool>> on);
        IJoinQuery<T1, T2, T3, T4, T5> RightJoin<T5>(IQuery<T5> q, Expression<Func<T1, T2, T3, T4, T5, bool>> on);
        IJoinQuery<T1, T2, T3, T4, T5> FullJoin<T5>(IQuery<T5> q, Expression<Func<T1, T2, T3, T4, T5, bool>> on);
        IQuery<TResult> Select<TResult>(Expression<Func<T1, T2, T3, T4, TResult>> selector);
    }

    /// <summary>
    /// Usage: joinQuery.Select((user, city) =&gt; new { User = user, City = city }).Where(a => a.User.Age &gt;= 18).OrderBy(a => a.User.Id).Select(a =&gt; new { UserId = a.User.Id, UserName = a.User.Name, CityName = a.City.Name }).TakePage(1, 20)
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    public interface IJoinQuery<T1, T2, T3, T4, T5>
    {
        //IJoinQuery<T1, T2, T3, T4, T5> InnerJoin<T5>(IQuery<T5> q, Expression<Func<T1, T2, T3, T4, T5, bool>> on);
        //IJoinQuery<T1, T2, T3, T4, T5> LeftJoin<T5>(IQuery<T5> q, Expression<Func<T1, T2, T3, T4, T5, bool>> on);
        //IJoinQuery<T1, T2, T3, T4, T5> RightJoin<T5>(IQuery<T5> q, Expression<Func<T1, T2, T3, T4, T5, bool>> on);

        IJoinQuery<T1, T2, T3, T4, T5> Where(Expression<Func<T1, T2, T3, T4, T5, bool>> predicate);

        IQuery<TResult> Select<TResult>(Expression<Func<T1, T2, T3, T4, T5, TResult>> selector);
    }
}
