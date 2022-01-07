using Chloe.DDL;
using Chloe.Descriptors;
using Chloe.Reflection;
using System.Xml.Linq;

namespace Chloe.MySql.DDL
{
    public class MySqlTableGenerator : TableGenerator
    {
        public MySqlTableGenerator(IDbContext dbContext) : base(dbContext)
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

            XDocument commentDoc = GetAssemblyCommentDoc(typeDescriptor.Definition.Type.Assembly);

            string c = "";
            foreach (var propertyDescriptor in typeDescriptor.PrimitivePropertyDescriptors.OrderBy(a => GetTypeInheritLayer(a.Property.DeclaringType)))
            {
                sb.AppendLine(c);
                sb.Append($"  {BuildColumnPart(propertyDescriptor, commentDoc)}");
                c = ",";
            }

            if (typeDescriptor.PrimaryKeys.Count > 0)
            {
                string key = typeDescriptor.PrimaryKeys.First().Column.Name;
                sb.AppendLine(c);
                sb.Append($"  PRIMARY KEY ({Utils.QuoteName(key)}) USING BTREE");
            }

            sb.AppendLine();
            sb.Append(");");

            return new List<string>() { sb.ToString() };
        }

        string BuildColumnPart(PrimitivePropertyDescriptor propertyDescriptor, XDocument commentDoc)
        {
            string part = $"{Utils.QuoteName(propertyDescriptor.Column.Name)} { GetDataTypeName(propertyDescriptor)}";

            if (propertyDescriptor.IsAutoIncrement)
            {
                part += " AUTO_INCREMENT";
            }

            if (!propertyDescriptor.IsNullable)
            {
                part += " NOT NULL";
            }
            else
            {
                part += " NULL";
            }

            string comment = FindComment(propertyDescriptor, commentDoc);
            if (!string.IsNullOrEmpty(comment))
                part += $" COMMENT '{comment}'";

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
                return $"varchar({stringLength})";
            }

            if (type == typeof(int))
            {
                return "int(11)";
            }

            if (type == typeof(byte))
            {
                return "int(11)";
            }

            if (type == typeof(Int16))
            {
                return "int(11)";
            }

            if (type == typeof(long))
            {
                return "bigint";
            }

            if (type == typeof(float))
            {
                return "float(10, 4)";
            }

            if (type == typeof(double))
            {
                return "double(10, 4)";
            }

            if (type == typeof(decimal))
            {
                return "decimal(10, 4)";
            }

            if (type == typeof(bool))
            {
                return "int(11)";
            }

            if (type == typeof(DateTime))
            {
                return "datetime(0)";
            }

            if (type == typeof(Guid))
            {
                return "varchar(50)";
            }

            throw new NotSupportedException(type.FullName);
        }
    }
}
