using Chloe.Data;
using Chloe.DbExpressions;
using Chloe.Descriptors;
using Chloe.Exceptions;
using System.Data;

namespace Chloe.SqlServer.Odbc
{
    public partial class MsSqlContext : DbContext
    {
        static Action<TEntity, IDataReader> GetMapper<TEntity>(PrimitivePropertyDescriptor propertyDescriptor, int ordinal)
        {
            var dbValueReader = DataReaderConstant.GetDbValueReader(propertyDescriptor.PropertyType);

            Action<TEntity, IDataReader> mapper = (TEntity entity, IDataReader reader) =>
            {
                object value = dbValueReader.GetValue(reader, ordinal);
                if (value == null || value == DBNull.Value)
                    throw new ChloeException($"Unable to get the {propertyDescriptor.Property.Name} value from data reader.");

                propertyDescriptor.SetValue(entity, value);
            };

            return mapper;
        }

        static string AppendInsertRangeSqlTemplate(DbTable table, List<PrimitivePropertyDescriptor> mappingPropertyDescriptors)
        {
            StringBuilder sqlBuilder = new StringBuilder();

            sqlBuilder.Append("INSERT INTO ");
            sqlBuilder.Append(AppendTableName(table));
            sqlBuilder.Append("(");

            for (int i = 0; i < mappingPropertyDescriptors.Count; i++)
            {
                PrimitivePropertyDescriptor mappingPropertyDescriptor = mappingPropertyDescriptors[i];
                if (i > 0)
                    sqlBuilder.Append(",");
                sqlBuilder.Append(Utils.QuoteName(mappingPropertyDescriptor.Column.Name));
            }

            sqlBuilder.Append(") VALUES");

            string sqlTemplate = sqlBuilder.ToString();
            return sqlTemplate;
        }
        static string AppendTableName(DbTable table)
        {
            if (string.IsNullOrEmpty(table.Schema))
                return Utils.QuoteName(table.Name);

            return string.Format("{0}.{1}", Utils.QuoteName(table.Schema), Utils.QuoteName(table.Name));
        }
    }
}
