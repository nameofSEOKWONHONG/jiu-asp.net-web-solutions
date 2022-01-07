using Chloe.Data;
using Chloe.DbExpressions;
using Chloe.Descriptors;
using Chloe.Exceptions;
using System.Data;

namespace Chloe.PostgreSQL
{
    public partial class PostgreSQLContext : DbContext
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

        string AppendInsertRangeSqlTemplate(DbTable table, List<PrimitivePropertyDescriptor> mappingPropertyDescriptors)
        {
            StringBuilder sqlBuilder = new StringBuilder();

            sqlBuilder.Append("INSERT INTO ");
            sqlBuilder.Append(this.AppendTableName(table));
            sqlBuilder.Append("(");

            for (int i = 0; i < mappingPropertyDescriptors.Count; i++)
            {
                PrimitivePropertyDescriptor mappingPropertyDescriptor = mappingPropertyDescriptors[i];
                if (i > 0)
                    sqlBuilder.Append(",");
                sqlBuilder.Append(Utils.QuoteName(mappingPropertyDescriptor.Column.Name, this.ConvertToLowercase));
            }

            sqlBuilder.Append(") VALUES");

            string sqlTemplate = sqlBuilder.ToString();
            return sqlTemplate;
        }
        string AppendTableName(DbTable table)
        {
            if (string.IsNullOrEmpty(table.Schema))
                return Utils.QuoteName(table.Name, this.ConvertToLowercase);

            return string.Format("{0}.{1}", Utils.QuoteName(table.Schema, this.ConvertToLowercase), Utils.QuoteName(table.Name, this.ConvertToLowercase));
        }
    }
}
