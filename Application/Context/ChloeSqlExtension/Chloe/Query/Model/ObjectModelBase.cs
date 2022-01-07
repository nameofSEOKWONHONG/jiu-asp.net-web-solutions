using Chloe.DbExpressions;
using Chloe.Query.Mapping;
using System.Linq.Expressions;
using System.Reflection;

namespace Chloe.Query
{
    public abstract class ObjectModelBase : IObjectModel
    {
        protected ObjectModelBase(Type objectType)
        {
            this.ObjectType = objectType;
        }

        public Type ObjectType { get; private set; }
        public abstract TypeKind TypeKind { get; }

        public virtual void AddConstructorParameter(ParameterInfo p, DbExpression primitiveExp)
        {
            throw new NotSupportedException();
        }
        public virtual void AddConstructorParameter(ParameterInfo p, ComplexObjectModel complexModel)
        {
            throw new NotSupportedException();
        }
        public virtual void AddPrimitiveMember(MemberInfo p, DbExpression exp)
        {
            throw new NotSupportedException();
        }
        public virtual DbExpression GetPrimitiveMember(MemberInfo memberInfo)
        {
            throw new NotSupportedException();
        }

        public virtual void AddComplexMember(MemberInfo p, ComplexObjectModel model)
        {
            throw new NotSupportedException();
        }
        public virtual ComplexObjectModel GetComplexMember(MemberInfo memberInfo)
        {
            throw new NotSupportedException();
        }

        public virtual void AddCollectionMember(MemberInfo p, CollectionObjectModel model)
        {
            throw new NotSupportedException();
        }
        public virtual CollectionObjectModel GetCollectionMember(MemberInfo memberInfo)
        {
            throw new NotSupportedException();
        }

        public virtual DbExpression GetDbExpression(MemberExpression memberExpressionDeriveParameter)
        {
            throw new NotSupportedException();
        }
        public virtual IObjectModel GetComplexMember(MemberExpression exp)
        {
            throw new NotSupportedException();
        }

        public virtual IObjectActivatorCreator GenarateObjectActivatorCreator(DbSqlQueryExpression sqlQuery)
        {
            throw new NotSupportedException();
        }

        public virtual IObjectModel ToNewObjectModel(DbSqlQueryExpression sqlQuery, DbTable table, DbMainTableExpression dependentTable, bool ignoreFilters)
        {
            throw new NotImplementedException();
        }

        public virtual void SetNullChecking(DbExpression exp)
        {

        }
    }
}
