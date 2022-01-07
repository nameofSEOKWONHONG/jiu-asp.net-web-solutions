using Chloe.DDL;
using Chloe.Descriptors;
using Chloe.Reflection;

namespace Chloe.SQLite.DDL
{
    public class SQLiteTableGenerator : TableGenerator
    {
        public SQLiteTableGenerator(IDbContext dbContext) : base(dbContext)
        {

        }

        public override List<string> GenCreateTableScript(TypeDescriptor typeDescriptor, TableCreateMode createMode = TableCreateMode.CreateIfNotExists)
        {
            string tableName = typeDescriptor.Table.Name;

            StringBuilder sb = new StringBuilder();

            if (createMode == TableCreateMode.CreateIfNotExists)
            {
                sb.Append($"CREATE TABLE IF NOT EXISTS {Utils.QuoteName(tableName)}(");
            }
            else if (createMode == TableCreateMode.CreateNew)
            {
                sb.AppendLine($"DROP TABLE IF EXISTS {Utils.QuoteName(tableName)};");
                sb.Append($"CREATE TABLE {Utils.QuoteName(tableName)}(");
            }
            else
            {
                sb.Append($"CREATE TABLE {Utils.QuoteName(tableName)}(");
            }

            string c = "";
            foreach (var propertyDescriptor in typeDescriptor.PrimitivePropertyDescriptors.OrderBy(a => GetTypeInheritLayer(a.Property.DeclaringType)))
            {
                sb.AppendLine(c);
                sb.Append($"  {this.BuildColumnPart(propertyDescriptor)}");
                c = ",";
            }

            sb.AppendLine();
            sb.Append(");");

            return new List<string>() { sb.ToString() };
        }

        string BuildColumnPart(PrimitivePropertyDescriptor propertyDescriptor)
        {
            string part = $"{Utils.QuoteName(propertyDescriptor.Column.Name)} {GetDataTypeName(propertyDescriptor)}";

            if (propertyDescriptor.IsPrimaryKey)
            {
                part += " PRIMARY KEY";
            }

            if (propertyDescriptor.IsAutoIncrement)
            {
                part += " AUTOINCREMENT";
            }

            if (!propertyDescriptor.IsNullable)
            {
                part += " NOT NULL";
            }
            else
            {
                part += " NULL";
            }

            return part;
        }
        static string GetDataTypeName(PrimitivePropertyDescriptor propertyDescriptor)
        {
            if (propertyDescriptor.TryGetAnnotation(typeof(DataTypeAttribute), out var annotation))
            {
                return (annotation as DataTypeAttribute).Name;
            }

            Type type = propertyDescriptor.PropertyType.GetUnderlyingType();
            if (type.IsEnum)
            {
                type = type.GetEnumUnderlyingType();
            }

            if (type == typeof(string))
            {
                int stringLength = propertyDescriptor.Column.Size ?? 4000;
                return $"NVARCHAR({stringLength})";
            }

            if (type == typeof(int))
            {
                if (propertyDescriptor.IsAutoIncrement)
                {
                    return "INTEGER";
                }

                return "INT";
            }

            if (type == typeof(byte))
            {
                return "TINYINT";
            }

            if (type == typeof(Int16))
            {
                return "SMALLINT";
            }

            if (type == typeof(long))
            {
                return "INT64";
            }

            if (type == typeof(float))
            {
                return "FLOAT";
            }

            if (type == typeof(double))
            {
                return "DOUBLE";
            }

            if (type == typeof(decimal))
            {
                return "NUMERIC";
            }

            if (type == typeof(bool))
            {
                return "BOOL";
            }

            if (type == typeof(DateTime))
            {
                return "DATETIME";
            }

            if (type == typeof(Guid))
            {
                return "GUID";
            }

            throw new NotSupportedException(type.FullName);
        }
    }
}
