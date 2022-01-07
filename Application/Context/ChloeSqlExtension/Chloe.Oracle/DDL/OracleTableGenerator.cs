using Chloe.DDL;
using Chloe.Descriptors;
using Chloe.Reflection;
using System.Xml.Linq;

namespace Chloe.Oracle.DDL
{
    public class OracleTableGenerator : TableGenerator
    {
        public OracleTableGenerator(IDbContext dbContext) : base(dbContext)
        {

        }

        string SqlName(string name)
        {
            OracleContext dbContext = (this.DbContext as OracleContext);
            if (dbContext.ConvertToUppercase)
                return name.ToUpper();

            return name;
        }
        string QuoteName(string name)
        {
            OracleContext dbContext = (this.DbContext as OracleContext);
            return Utils.QuoteName(name, dbContext.ConvertToUppercase);
        }

        bool ExistsTable(string tableName)
        {
            bool tableExists = this.DbContext.SqlQuery<int>($"select count(1) from user_tables where TABLE_NAME = '{this.SqlName(tableName)}'").First() > 0;
            return tableExists;
        }

        public override List<string> GenCreateTableScript(TypeDescriptor typeDescriptor, TableCreateMode createMode = TableCreateMode.CreateIfNotExists)
        {
            string tableName = typeDescriptor.Table.Name;

            List<string> sqlList = new List<string>();

            if (createMode == TableCreateMode.CreateIfNotExists)
            {
                bool tableExists = this.ExistsTable(tableName);
                if (tableExists)
                {
                    return sqlList;
                }
            }
            else if (createMode == TableCreateMode.CreateNew)
            {
                bool tableExists = this.ExistsTable(tableName);
                if (tableExists)
                    sqlList.Add($"DROP TABLE {this.QuoteName(tableName)}");
            }

            StringBuilder sb = new StringBuilder();
            sb.Append($"CREATE TABLE {this.QuoteName(tableName)}(");

            string c = "";
            foreach (var propertyDescriptor in typeDescriptor.PrimitivePropertyDescriptors.OrderBy(a => GetTypeInheritLayer(a.Property.DeclaringType)))
            {
                sb.AppendLine(c);
                sb.Append($"  {this.BuildColumnPart(propertyDescriptor)}");
                c = ",";
            }

            sb.AppendLine();
            sb.Append(")");

            sqlList.Add(sb.ToString());

            if (typeDescriptor.PrimaryKeys.Count > 0)
            {
                string key = typeDescriptor.PrimaryKeys.First().Column.Name;
                sqlList.Add($"ALTER TABLE {this.QuoteName(tableName)} ADD CHECK ({this.QuoteName(key)} IS NOT NULL)");

                sqlList.Add($"ALTER TABLE {this.QuoteName(tableName)} ADD PRIMARY KEY ({this.QuoteName(key)})");
            }

            if (typeDescriptor.AutoIncrement != null)
            {
                string seqName = $"{tableName.ToUpper()}_{typeDescriptor.AutoIncrement.Column.Name.ToUpper()}_SEQ".ToUpper();
                bool seqExists = this.DbContext.SqlQuery<int>($"select count(*) from dba_sequences where SEQUENCE_NAME='{seqName}'").First() > 0;
                if (!seqExists)
                {
                    string seqScript = $"CREATE SEQUENCE {this.QuoteName(seqName)} INCREMENT BY 1 MINVALUE 1 MAXVALUE 9999999999999999999999999999 START WITH 1 CACHE 20";

                    sqlList.Add(seqScript);
                }

                string triggerName = $"{seqName.ToUpper()}_TRIGGER";
                string createTrigger = $@"create or replace trigger {triggerName} before insert on {tableName.ToUpper()} for each row 
begin 
select {seqName.ToUpper()}.nextval into :new.{typeDescriptor.AutoIncrement.Column.Name} from dual;
end;";

                sqlList.Add(createTrigger);
            }

            var seqProperties = typeDescriptor.PrimitivePropertyDescriptors.Where(a => a.HasSequence());
            foreach (var seqProperty in seqProperties)
            {
                if (seqProperty == typeDescriptor.AutoIncrement)
                {
                    continue;
                }

                string seqName = seqProperty.Definition.SequenceName;
                bool seqExists = this.DbContext.SqlQuery<int>($"select count(*) from dba_sequences where SEQUENCE_NAME='{seqName}'").First() > 0;

                if (!seqExists)
                {
                    string seqScript = $"CREATE SEQUENCE {this.QuoteName(seqName)} INCREMENT BY 1 MINVALUE 1 MAXVALUE 9999999999999999999999999999 START WITH 1 CACHE 20";
                    sqlList.Add(seqScript);
                }
            }

            XDocument commentDoc = GetAssemblyCommentDoc(typeDescriptor.Definition.Type.Assembly);
            sqlList.AddRange(this.GenColumnCommentScripts(typeDescriptor, commentDoc));

            return sqlList;
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
                int stringLength = propertyDescriptor.Column.Size ?? 2000;
                return $"NVARCHAR2({stringLength})";
            }

            if (type == typeof(int))
            {
                return "NUMBER(9,0)";
            }

            if (type == typeof(byte))
            {
                return "NUMBER(3,0)";
            }

            if (type == typeof(Int16))
            {
                return "NUMBER(4,0)";
            }

            if (type == typeof(long))
            {
                return "NUMBER(18,0)";
            }

            if (type == typeof(float))
            {
                return "BINARY_FLOAT";
            }

            if (type == typeof(double))
            {
                return "BINARY_DOUBLE";
            }

            if (type == typeof(decimal))
            {
                return "NUMBER";
            }

            if (type == typeof(bool))
            {
                return "NUMBER(9,0)";
            }

            if (type == typeof(DateTime))
            {
                return "DATE";
            }

            if (type == typeof(Guid))
            {
                return "BLOB";
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

            string tableName = propertyDescriptor.DeclaringTypeDescriptor.Table.Name;
            string columnName = propertyDescriptor.Column.Name;
            string str = $"COMMENT ON COLUMN {this.QuoteName(tableName)}.{this.QuoteName(columnName)} IS '{comment}'";

            return str;
        }
    }
}
