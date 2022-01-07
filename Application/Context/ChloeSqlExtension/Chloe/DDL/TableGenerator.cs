using Chloe.Descriptors;
using Chloe.Infrastructure;
using System.Reflection;
using System.Xml.Linq;

namespace Chloe.DDL
{
    public abstract class TableGenerator
    {
        public TableGenerator(IDbContext dbContext)
        {
            this.DbContext = dbContext;
        }

        public IDbContext DbContext { get; }

        public void CreateTables(TableCreateMode createMode = TableCreateMode.CreateIfNotExists)
        {
            TypeDescriptor[] typeDescriptors = EntityTypeContainer.GetRegisteredTypeDescriptors();
            this.CreateTables(typeDescriptors, createMode);
        }
        public void CreateTables(IDbContext dbContext, Type[] entityTypes, TableCreateMode createMode = TableCreateMode.CreateIfNotExists)
        {
            var typeDescriptors = entityTypes.Select(a => EntityTypeContainer.GetDescriptor(a));
            CreateTables(typeDescriptors);
        }
        void CreateTables(IEnumerable<TypeDescriptor> typeDescriptors, TableCreateMode createMode = TableCreateMode.CreateIfNotExists)
        {
            List<string> createTableScripts = this.GenCreateTableScripts(typeDescriptors, createMode);
            foreach (var createTableScript in createTableScripts)
            {
                this.DbContext.Session.ExecuteNonQuery(createTableScript);
            }
        }
        List<string> GenCreateTableScripts(IEnumerable<TypeDescriptor> typeDescriptors, TableCreateMode createMode = TableCreateMode.CreateIfNotExists)
        {
            return typeDescriptors.SelectMany(a => this.GenCreateTableScript(a, createMode)).ToList();
        }

        public abstract List<string> GenCreateTableScript(TypeDescriptor typeDescriptor, TableCreateMode createMode = TableCreateMode.CreateIfNotExists);

        public static XDocument GetAssemblyCommentDoc(Assembly assembly)
        {
            FileInfo assemblyFileInfo = new FileInfo(assembly.Location);

            string commentFileName = $"{assembly.GetName().Name}.xml";
            var commentFileFullName = Path.Combine(assemblyFileInfo.DirectoryName, commentFileName);

            if (!File.Exists(commentFileFullName))
            {
                return null;
            }

            XDocument xmlDoc = XDocument.Load(commentFileFullName);
            return xmlDoc;
        }
        public static string FindComment(PrimitivePropertyDescriptor propertyDescriptor, XDocument doc)
        {
            if (doc == null)
                return "";

            var root = doc.Root;

            string attrName = $"P:{propertyDescriptor.Definition.Property.DeclaringType.FullName}.{propertyDescriptor.Definition.Property.Name}";

            var memberNode = root.Element("members").Elements("member").Where(a => a.Attribute("name").Value == attrName).FirstOrDefault();
            if (memberNode == null)
                return "";

            var summaryNode = memberNode.Element("summary");
            if (summaryNode == null)
                return "";

            var comment = summaryNode.Value.Trim('\n', ' ');
            return comment;
        }
        public static int GetTypeInheritLayer(Type type)
        {
            if (type.BaseType != null)
            {
                return GetTypeInheritLayer(type.BaseType) + 1;
            }

            return 0;
        }
    }
}
