using Chloe.DDL;
using Chloe.Descriptors;
using Chloe.Reflection;
using System.Xml.Linq;

namespace Chloe.PostgreSQL.DDL
{
    public class PostgreSQLTableGenerator : TableGenerator
    {
        public PostgreSQLTableGenerator(IDbContext dbContext) : base(dbContext)
        {

        }

        string QuoteName(string name)
        {
            PostgreSQLContext dbContext = (this.DbContext as PostgreSQLContext);
            return Utils.QuoteName(name, dbContext.ConvertToLowercase);
        }

        public override List<string> GenCreateTableScript(TypeDescriptor typeDescriptor, TableCreateMode createMode = TableCreateMode.CreateIfNotExists)
        {
            string tableName = typeDescriptor.Table.Name;
            string schema = typeDescriptor.Table.Schema ?? "public";

            StringBuilder sb = new StringBuilder();

            if (createMode == TableCreateMode.CreateIfNotExists)
            {
                sb.Append($"CREATE TABLE IF NOT EXISTS {this.QuoteName(schema)}.{this.QuoteName(tableName)}(");
            }
            else if (createMode == TableCreateMode.CreateNew)
            {
                sb.AppendLine($"DROP TABLE IF EXISTS {this.QuoteName(schema)}.{this.QuoteName(tableName)};");
                sb.Append($"CREATE TABLE {this.QuoteName(schema)}.{this.QuoteName(tableName)}(");
            }
            else
            {
                sb.Append($"CREATE TABLE {this.QuoteName(schema)}.{this.QuoteName(tableName)}(");
            }


            string c = "";
            foreach (var propertyDescriptor in typeDescriptor.PrimitivePropertyDescriptors.OrderBy(a => GetTypeInheritLayer(a.Property.DeclaringType)))
            {
                sb.AppendLine(c);
                sb.Append($"  {this.BuildColumnPart(propertyDescriptor)}");
                c = ",";
            }

            if (typeDescriptor.PrimaryKeys.Count > 0)
            {
                string key = typeDescriptor.PrimaryKeys.First().Column.Name;
                sb.AppendLine(c);
                sb.Append($"  PRIMARY KEY ({this.QuoteName(key)})");
            }

            sb.AppendLine();
            sb.Append(");");

            List<string> scripts = new List<string>();
            scripts.Add(sb.ToString());

            XDocument commentDoc = GetAssemblyCommentDoc(typeDescriptor.Definition.Type.Assembly);
            scripts.AddRange(this.GenColumnCommentScripts(typeDescriptor, commentDoc));

            return scripts;
        }

        string BuildColumnPart(PrimitivePropertyDescriptor propertyDescriptor)
        {
            string part = $"{this.QuoteName(propertyDescriptor.Column.Name)} {GetDataTypeName(propertyDescriptor)}";

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
                return $"varchar({stringLength})";
            }

            if (type == typeof(int))
            {
                if (propertyDescriptor.IsAutoIncrement)
                    return "serial4";

                return "int4";
            }

            if (type == typeof(byte))
            {
                return "int2";
            }

            if (type == typeof(Int16))
            {
                return "int2";
            }

            if (type == typeof(long))
            {
                if (propertyDescriptor.IsAutoIncrement)
                    return "serial8";

                return "int8";
            }

            if (type == typeof(float))
            {
                return "float4";
            }

            if (type == typeof(double))
            {
                return "float8";
            }

            if (type == typeof(decimal))
            {
                return "decimal(18,4)";
            }

            if (type == typeof(bool))
            {
                return "boolean";
            }

            if (type == typeof(DateTime))
            {
                return "timestamp";
            }

            if (type == typeof(Guid))
            {
                return "uuid";
            }

            throw new NotSupportedException(type.FullName);
        }

        List<string> GenColumnCommentScripts(TypeDescriptor typeDescriptor, XDocument commentDoc)
        {
            return typeDescriptor.PrimitivePropertyDescriptors.Select(a => this.GenCommentScript(a, commentDoc)).Where(a => !string.IsNullOrEmpty(a)).ToList();
        }
        string GenCommentScript(PrimitivePropertyDescriptor propertyDescriptor, XDocument commentDoc)
        {
            string comment = FindComment(propertyDescriptor, commentDoc);
            if (string.IsNullOrEmpty(comment))
                return null;

            string schema = propertyDescriptor.DeclaringTypeDescriptor.Table.Schema ?? "public";
            string tableName = propertyDescriptor.DeclaringTypeDescriptor.Table.Name;
            string columnName = propertyDescriptor.Column.Name;
            string str = $"COMMENT ON COLUMN {this.QuoteName(schema)}.{this.QuoteName(tableName)}.{this.QuoteName(columnName)} IS '{comment}';";

            return str;
        }
    }
}
