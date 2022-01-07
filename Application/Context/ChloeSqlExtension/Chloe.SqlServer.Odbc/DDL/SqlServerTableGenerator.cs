using Chloe.DDL;
using Chloe.Descriptors;
using Chloe.Reflection;
using System.Xml.Linq;

namespace Chloe.SqlServer.Odbc.DDL
{
    public class SqlServerTableGenerator : TableGenerator
    {
        public SqlServerTableGenerator(IDbContext dbContext) : base(dbContext)
        {

        }

        bool ExistsTable(string tableName)
        {
            bool tableExists = this.DbContext.SqlQuery<int>($"select count(1) from sysobjects where name='{tableName}' and xtype='U'").First() > 0;
            return tableExists;
        }

        public override List<string> GenCreateTableScript(TypeDescriptor typeDescriptor, TableCreateMode createMode = TableCreateMode.CreateIfNotExists)
        {
            string tableName = typeDescriptor.Table.Name;

            StringBuilder sb = new StringBuilder();

            if (createMode == TableCreateMode.CreateIfNotExists)
            {
                bool tableExists = this.ExistsTable(tableName);
                if (tableExists)
                {
                    return new List<string>();
                }
            }
            else if (createMode == TableCreateMode.CreateNew)
            {
                //this command needs sqlserver 2016 or up
                //sb.AppendLine($"DROP TABLE IF EXISTS {Utils.QuoteName(tableName)};");
                sb.AppendLine($"IF OBJECT_ID(N'dbo.{Utils.QuoteName(tableName)}', N'U') IS NOT NULL DROP TABLE {Utils.QuoteName(tableName)};");
            }

            sb.Append($"CREATE TABLE {Utils.QuoteName(tableName)}(");

            string c = "";
            foreach (var propertyDescriptor in typeDescriptor.PrimitivePropertyDescriptors.OrderBy(a => GetTypeInheritLayer(a.Property.DeclaringType)))
            {
                sb.AppendLine(c);
                sb.Append($"  {BuildColumnPart(propertyDescriptor)}");
                c = ",";
            }

            if (typeDescriptor.PrimaryKeys.Count > 0)
            {
                string key = typeDescriptor.PrimaryKeys.First().Column.Name;
                string constraintName = $"PK_{tableName}";
                sb.AppendLine(c);
                sb.Append($"CONSTRAINT {Utils.QuoteName(constraintName)} PRIMARY KEY CLUSTERED ({Utils.QuoteName(key)} ASC)");
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
            string part = $"{Utils.QuoteName(propertyDescriptor.Column.Name)} {GetDataTypeName(propertyDescriptor)}";

            if (propertyDescriptor.IsAutoIncrement)
            {
                part += " IDENTITY(1,1)";
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
                return "int";
            }

            if (type == typeof(byte))
            {
                return "tinyint";
            }

            if (type == typeof(Int16))
            {
                return "smallint";
            }

            if (type == typeof(long))
            {
                return "bigint";
            }

            if (type == typeof(float))
            {
                return "real";
            }

            if (type == typeof(double))
            {
                return "float";
            }

            if (type == typeof(decimal))
            {
                int scale = propertyDescriptor.Column.Scale ?? 18;
                int precision = propertyDescriptor.Column.Precision ?? 2;
                return $"decimal({scale},{precision})";
            }

            if (type == typeof(bool))
            {
                return "bit";
            }

            if (type == typeof(DateTime))
            {
                return "datetime";
            }

            if (type == typeof(Guid))
            {
                return "uniqueidentifier";
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

            string schema = propertyDescriptor.DeclaringTypeDescriptor.Table.Schema ?? "dbo";
            string tableName = propertyDescriptor.DeclaringTypeDescriptor.Table.Name;
            string columnName = propertyDescriptor.Column.Name;
            string str = $"EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'{comment}', @level0type=N'SCHEMA',@level0name=N'{schema}', @level1type=N'TABLE',@level1name=N'{tableName}', @level2type=N'COLUMN',@level2name=N'{columnName}'";

            return str;
        }
    }
}
