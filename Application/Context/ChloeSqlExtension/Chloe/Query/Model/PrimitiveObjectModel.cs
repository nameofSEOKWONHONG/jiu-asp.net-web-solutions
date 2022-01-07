using Chloe.DbExpressions;
using Chloe.Extensions;
using Chloe.Query.Mapping;
using System.Linq.Expressions;
using System.Reflection;

namespace Chloe.Query
{
    public class PrimitiveObjectModel : ObjectModelBase
    {
        public PrimitiveObjectModel(Type primitiveType, DbExpression exp) : base(primitiveType)
        {
            this.Expression = exp;
        }

        public override TypeKind TypeKind { get { return TypeKind.Primitive; } }
        public DbExpression Expression { get; private set; }

        public DbExpression NullChecking { get; set; }

        public override DbExpression GetDbExpression(MemberExpression memberExpressionDeriveParameter)
        {
            Stack<MemberExpression> memberExpressions = ExpressionExtension.Reverse(memberExpressionDeriveParameter);

            if (memberExpressions.Count == 0)
                throw new Exception();

            DbExpression ret = this.Expression;

            foreach (MemberExpression memberExpression in memberExpressions)
            {
                MemberInfo member = memberExpression.Member;
                ret = DbExpression.MemberAccess(member, ret);
            }

            if (ret == null)
                throw new Exception(memberExpressionDeriveParameter.ToString());

            return ret;
        }

        public override IObjectActivatorCreator GenarateObjectActivatorCreator(DbSqlQueryExpression sqlQuery)
        {
            int ordinal;
            ordinal = ObjectModelHelper.TryGetOrAddColumn(sqlQuery, this.Expression).Value;

            PrimitiveObjectActivatorCreator activatorCreator = new PrimitiveObjectActivatorCreator(this.ObjectType, ordinal);

            activatorCreator.CheckNullOrdinal = ObjectModelHelper.TryGetOrAddColumn(sqlQuery, this.NullChecking);

            return activatorCreator;
        }

        public override IObjectModel ToNewObjectModel(DbSqlQueryExpression sqlQuery, DbTable table, DbMainTableExpression dependentTable, bool ignoreFilters)
        {
            DbColumnAccessExpression cae = ObjectModelHelper.ParseColumnAccessExpression(sqlQuery, table, this.Expression);

            PrimitiveObjectModel objectModel = new PrimitiveObjectModel(this.ObjectType, cae);

            objectModel.NullChecking = ObjectModelHelper.TryGetOrAddNullChecking(sqlQuery, table, this.NullChecking);

            return objectModel;
        }

        public override void SetNullChecking(DbExpression exp)
        {
            if (this.NullChecking == null)
                this.NullChecking = exp;
        }
    }
}
