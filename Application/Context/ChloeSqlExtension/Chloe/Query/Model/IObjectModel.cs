using Chloe.DbExpressions;
using Chloe.Query.Mapping;
using System.Linq.Expressions;
using System.Reflection;

namespace Chloe.Query
{
    public interface IObjectModel
    {
        Type ObjectType { get; }
        TypeKind TypeKind { get; }
        IObjectActivatorCreator GenarateObjectActivatorCreator(DbSqlQueryExpression sqlQuery);
        IObjectModel ToNewObjectModel(DbSqlQueryExpression sqlQuery, DbTable table, DbMainTableExpression dependentTable, bool ignoreFilters);
        void AddConstructorParameter(ParameterInfo p, DbExpression primitiveExp);
        void AddConstructorParameter(ParameterInfo p, ComplexObjectModel complexModel);

        void AddPrimitiveMember(MemberInfo memberInfo, DbExpression exp);
        DbExpression GetPrimitiveMember(MemberInfo memberInfo);

        void AddComplexMember(MemberInfo memberInfo, ComplexObjectModel model);
        ComplexObjectModel GetComplexMember(MemberInfo memberInfo);

        void AddCollectionMember(MemberInfo memberInfo, CollectionObjectModel model);
        CollectionObjectModel GetCollectionMember(MemberInfo memberInfo);

        DbExpression GetDbExpression(MemberExpression memberExpressionDeriveParameter);
        IObjectModel GetComplexMember(MemberExpression exp);

        void SetNullChecking(DbExpression exp);
    }

    public static class ObjectModelHelper
    {
        public static DbExpression TryGetOrAddNullChecking(DbSqlQueryExpression sqlQuery, DbTable table, DbExpression exp)
        {
            if (exp == null)
                return null;

            List<DbColumnSegment> columnList = sqlQuery.ColumnSegments;
            DbColumnSegment columnSeg = null;

            columnSeg = columnList.Where(a => DbExpressionEqualityComparer.EqualsCompare(a.Body, exp)).FirstOrDefault();

            if (columnSeg == null)
            {
                string alias = Utils.GenerateUniqueColumnAlias(sqlQuery);
                columnSeg = new DbColumnSegment(exp, alias);

                columnList.Add(columnSeg);
            }

            DbColumnAccessExpression cae = new DbColumnAccessExpression(table, DbColumn.MakeColumn(columnSeg.Body, columnSeg.Alias));
            return cae;
        }
        public static int? TryGetOrAddColumn(DbSqlQueryExpression sqlQuery, DbExpression exp, string addDefaultAlias = UtilConstants.DefaultColumnAlias)
        {
            if (exp == null)
                return null;

            List<DbColumnSegment> columnList = sqlQuery.ColumnSegments;
            DbColumnSegment columnSeg = null;

            int? ordinal = null;
            for (int i = 0; i < columnList.Count; i++)
            {
                var item = columnList[i];
                if (DbExpressionEqualityComparer.EqualsCompare(item.Body, exp))
                {
                    ordinal = i;
                    columnSeg = item;
                    break;
                }
            }

            if (ordinal == null)
            {
                string alias = Utils.GenerateUniqueColumnAlias(sqlQuery, addDefaultAlias);
                columnSeg = new DbColumnSegment(exp, alias);

                columnList.Add(columnSeg);
                ordinal = columnList.Count - 1;
            }

            return ordinal.Value;
        }
        public static DbColumnAccessExpression ParseColumnAccessExpression(DbSqlQueryExpression sqlQuery, DbTable table, DbExpression exp, string defaultAlias = UtilConstants.DefaultColumnAlias)
        {
            string alias = Utils.GenerateUniqueColumnAlias(sqlQuery, defaultAlias);
            DbColumnSegment columnSeg = new DbColumnSegment(exp, alias);

            sqlQuery.ColumnSegments.Add(columnSeg);

            DbColumnAccessExpression cae = new DbColumnAccessExpression(table, DbColumn.MakeColumn(exp, alias));
            return cae;
        }
    }
}
