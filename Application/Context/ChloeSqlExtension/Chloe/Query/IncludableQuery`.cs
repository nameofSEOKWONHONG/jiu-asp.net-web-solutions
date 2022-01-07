using Chloe.Extensions;
using Chloe.Query.QueryExpressions;
using Chloe.Reflection;
using System.Linq.Expressions;
using System.Reflection;

namespace Chloe.Query
{
    class IncludableQuery<TEntity, TNavigation> : Query<TEntity>, IIncludableQuery<TEntity, TNavigation>
    {
        public IncludableQuery(DbContext dbContext, bool trackEntity, QueryExpression prevExpression, LambdaExpression navigationPath)
            : base(dbContext, BuildIncludeExpression(dbContext, prevExpression, navigationPath), trackEntity)
        {

        }
        IncludableQuery(DbContext dbContext, bool trackEntity, QueryExpression exp)
         : base(dbContext, exp, trackEntity)
        {

        }

        static List<MemberExpression> ExtractMemberAccessChain(LambdaExpression navigationPath)
        {
            List<MemberExpression> members = new List<MemberExpression>();

            Expression exp = navigationPath.Body;
            while (exp != null && exp.NodeType == ExpressionType.MemberAccess)
            {
                MemberExpression member = exp as MemberExpression;
                members.Add(member);
                exp = member.Expression;
            }

            if (exp != navigationPath.Parameters[0] || members.Count == 0)
            {
                throw new ArgumentException($"Not support inclue navigation path {navigationPath.Body.ToString()}");
            }

            members.Reverse();
            return members;
        }
        static QueryExpression BuildIncludeExpression(DbContext dbContext, QueryExpression prevExpression, LambdaExpression navigationPath)
        {
            List<MemberExpression> memberExps = ExtractMemberAccessChain(navigationPath);

            NavigationNode startNavigation = null;
            NavigationNode lastNavigation = null;
            for (int i = 0; i < memberExps.Count; i++)
            {
                PropertyInfo member = memberExps[i].Member as PropertyInfo;
                NavigationNode navigation = InitNavigationNode(member, dbContext);

                if (startNavigation == null)
                {
                    startNavigation = navigation;
                    lastNavigation = navigation;
                    continue;
                }

                lastNavigation.Next = navigation;
                lastNavigation = navigation;
            }

            IncludeExpression ret = new IncludeExpression(typeof(TEntity), prevExpression, startNavigation);

            return ret;
        }
        static NavigationNode InitNavigationNode(PropertyInfo member, DbContext dbContext)
        {
            NavigationNode navigation = new NavigationNode(member);

            Type elementType = member.PropertyType;
            if (member.PropertyType.IsGenericCollection())
            {
                elementType = member.PropertyType.GetGenericArguments()[0];
            }

            List<LambdaExpression> filters = dbContext.QueryFilters.FindValue(elementType);
            if (filters != null)
                navigation.ContextFilters.AddRange(filters);

            return navigation;
        }

        IncludeExpression BuildThenIncludeExpression(LambdaExpression navigationPath)
        {
            IncludeExpression prevIncludeExpression = this.QueryExpression as IncludeExpression;
            NavigationNode startNavigation = prevIncludeExpression.NavigationNode.Clone();
            NavigationNode lastNavigation = startNavigation.GetLast();

            List<MemberExpression> memberExps = ExtractMemberAccessChain(navigationPath);

            for (int i = 0; i < memberExps.Count; i++)
            {
                PropertyInfo member = memberExps[i].Member as PropertyInfo;
                NavigationNode navigation = InitNavigationNode(member, this.DbContext);

                lastNavigation.Next = navigation;
                lastNavigation = navigation;
            }

            IncludeExpression includeExpression = new IncludeExpression(typeof(TEntity), prevIncludeExpression.PrevExpression, startNavigation);
            return includeExpression;
        }

        public IIncludableQuery<TEntity, TProperty> ThenInclude<TProperty>(Expression<Func<TNavigation, TProperty>> navigationPath)
        {
            IncludeExpression includeExpression = this.BuildThenIncludeExpression(navigationPath);
            return new IncludableQuery<TEntity, TProperty>(this.DbContext, this.TrackEntity, includeExpression);
        }

        public IIncludableQuery<TEntity, TCollectionItem> ThenIncludeMany<TCollectionItem>(Expression<Func<TNavigation, IEnumerable<TCollectionItem>>> navigationPath)
        {
            IncludeExpression includeExpression = this.BuildThenIncludeExpression(navigationPath);
            return new IncludableQuery<TEntity, TCollectionItem>(this.DbContext, this.TrackEntity, includeExpression);
        }

        public IIncludableQuery<TEntity, TNavigation> AndWhere(Expression<Func<TNavigation, bool>> predicate)
        {
            IncludeExpression prevIncludeExpression = this.QueryExpression as IncludeExpression;
            NavigationNode startNavigation = prevIncludeExpression.NavigationNode.Clone();
            NavigationNode lastNavigation = startNavigation.GetLast();
            lastNavigation.Condition = lastNavigation.Condition.And(predicate);

            IncludeExpression includeExpression = new IncludeExpression(typeof(TEntity), prevIncludeExpression.PrevExpression, startNavigation);

            return new IncludableQuery<TEntity, TNavigation>(this.DbContext, this.TrackEntity, includeExpression);
        }
    }
}
