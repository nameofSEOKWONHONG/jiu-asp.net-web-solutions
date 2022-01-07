using Chloe.Descriptors;
using Chloe.Extension;
using Chloe.Infrastructure;
using Chloe.Threading.Tasks;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Chloe
{
    public static partial class DbContextExtension
    {
        public static IQuery<T> Query<T>(this IDbContext dbContext, Expression<Func<T, bool>> predicate)
        {
            return dbContext.Query<T>().Where(predicate);
        }

        /// <summary>
        /// dbContext.Update&lt;User&gt;(user, a =&gt; a.Id == 1)
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbContext"></param>
        /// <param name="entity"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static int Update<TEntity>(this IDbContext dbContext, TEntity entity, Expression<Func<TEntity, bool>> condition)
        {
            return Update(dbContext, entity, condition, false).GetResult();
        }
        public static async Task<int> UpdateAsync<TEntity>(this IDbContext dbContext, TEntity entity, Expression<Func<TEntity, bool>> condition)
        {
            return await Update(dbContext, entity, condition, true);
        }
        static async Task<int> Update<TEntity>(IDbContext dbContext, TEntity entity, Expression<Func<TEntity, bool>> condition, bool @async)
        {
            PublicHelper.CheckNull(dbContext);
            PublicHelper.CheckNull(entity);
            PublicHelper.CheckNull(condition);

            Type entityType = typeof(TEntity);
            TypeDescriptor typeDescriptor = EntityTypeContainer.GetDescriptor(entityType);

            List<MemberBinding> bindings = new List<MemberBinding>();

            ConstantExpression entityConstantExp = Expression.Constant(entity);
            foreach (PrimitivePropertyDescriptor propertyDescriptor in typeDescriptor.PrimitivePropertyDescriptors)
            {
                if (propertyDescriptor.CannotUpdate())
                {
                    continue;
                }

                Expression entityMemberAccess = Expression.MakeMemberAccess(entityConstantExp, propertyDescriptor.Property);
                MemberAssignment bind = Expression.Bind(propertyDescriptor.Property, entityMemberAccess);

                bindings.Add(bind);
            }

            return await UpdateOnly(dbContext, condition, bindings, @async);
        }

        /// <summary>
        /// dbContext.UpdateOnly&lt;User&gt;(user, a =&gt; new { a.Name, a.Age })
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbContext"></param>
        /// <param name="entity"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public static int UpdateOnly<TEntity>(this IDbContext dbContext, TEntity entity, Expression<Func<TEntity, object>> fields)
        {
            PublicHelper.CheckNull(fields);

            List<string> fieldList = FieldsResolver.Resolve(fields);
            return dbContext.UpdateOnly(entity, fieldList.ToArray());
        }
        /// <summary>
        /// dbContext.UpdateOnly&lt;User&gt;(user, "Name,Age", "NickName")
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbContext"></param>
        /// <param name="entity"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public static int UpdateOnly<TEntity>(this IDbContext dbContext, TEntity entity, params string[] fields)
        {
            return UpdateOnly(dbContext, entity, fields, false).GetResult();
        }
        public static async Task<int> UpdateOnlyAsync<TEntity>(this IDbContext dbContext, TEntity entity, Expression<Func<TEntity, object>> fields)
        {
            PublicHelper.CheckNull(fields);

            List<string> fieldList = FieldsResolver.Resolve(fields);
            return await UpdateOnlyAsync(dbContext, entity, fieldList.ToArray());
        }
        public static async Task<int> UpdateOnlyAsync<TEntity>(IDbContext dbContext, TEntity entity, params string[] fields)
        {
            return await UpdateOnly(dbContext, entity, fields, true);
        }
        static async Task<int> UpdateOnly<TEntity>(IDbContext dbContext, TEntity entity, string[] fields, bool @async)
        {
            PublicHelper.CheckNull(dbContext);
            PublicHelper.CheckNull(entity);
            PublicHelper.CheckNull(fields);

            /* 支持 context.UpdateOnly<User>(user, "Name,Age", "NickName"); */
            fields = fields.SelectMany(a => a.Split(',')).Select(a => a.Trim()).ToArray();

            if (fields.Length == 0)
                throw new ArgumentException("fields");

            Type entityType = typeof(TEntity);
            TypeDescriptor typeDescriptor = EntityTypeContainer.GetDescriptor(entityType);

            List<MemberBinding> bindings = new List<MemberBinding>();

            ConstantExpression entityConstantExp = Expression.Constant(entity);
            foreach (string field in fields)
            {
                MemberInfo memberInfo = entityType.GetMember(field)[0];
                var propertyDescriptor = typeDescriptor.FindPrimitivePropertyDescriptor(memberInfo);

                if (propertyDescriptor == null)
                    throw new ArgumentException(string.Format("Could not find the member '{0}' from entity.", propertyDescriptor.Column.Name));

                Expression entityMemberAccess = Expression.MakeMemberAccess(entityConstantExp, memberInfo);
                MemberAssignment bind = Expression.Bind(memberInfo, entityMemberAccess);

                bindings.Add(bind);
            }

            ParameterExpression parameter = Expression.Parameter(entityType, "a");
            Expression conditionBody = null;
            foreach (PrimitivePropertyDescriptor primaryKey in typeDescriptor.PrimaryKeys)
            {
                Expression propOrField = Expression.PropertyOrField(parameter, primaryKey.Property.Name);
                Expression keyValue = Expression.MakeMemberAccess(entityConstantExp, primaryKey.Property);
                Expression e = Expression.Equal(propOrField, keyValue);
                conditionBody = conditionBody == null ? e : Expression.AndAlso(conditionBody, e);
            }

            Expression<Func<TEntity, bool>> condition = Expression.Lambda<Func<TEntity, bool>>(conditionBody, parameter);

            return await UpdateOnly(dbContext, condition, bindings, @async);
        }

        static async Task<int> UpdateOnly<TEntity>(IDbContext dbContext, Expression<Func<TEntity, bool>> condition, List<MemberBinding> bindings, bool @async)
        {
            Type entityType = typeof(TEntity);
            NewExpression newExp = Expression.New(entityType);

            ParameterExpression parameter = Expression.Parameter(entityType, "a");
            Expression lambdaBody = Expression.MemberInit(newExp, bindings);

            Expression<Func<TEntity, TEntity>> lambda = Expression.Lambda<Func<TEntity, TEntity>>(lambdaBody, parameter);

            if (@async)
                return await dbContext.UpdateAsync(condition, lambda);

            return dbContext.Update(condition, lambda);
        }

        public static DbActionBag CreateActionBag(this IDbContext dbContext)
        {
            DbActionBag bag = new DbActionBag(dbContext);
            return bag;
        }
    }
}
