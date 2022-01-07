using Chloe.DbExpressions;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace Chloe.Entity
{
    public class TypeDefinition
    {
        public TypeDefinition(Type entityType, DbTable table, IList<PrimitivePropertyDefinition> primitiveProperties, IList<ComplexPropertyDefinition> complexProperties, IList<CollectionPropertyDefinition> collectionProperties, IList<LambdaExpression> filters, IList<object> annotations)
        {
            PublicHelper.CheckNull(entityType, nameof(entityType));
            PublicHelper.CheckNull(table, nameof(table));
            PublicHelper.CheckNull(primitiveProperties, nameof(primitiveProperties));
            PublicHelper.CheckNull(complexProperties, nameof(complexProperties));
            PublicHelper.CheckNull(collectionProperties, nameof(collectionProperties));
            PublicHelper.CheckNull(annotations, nameof(annotations));

            this.Type = entityType;
            this.Table = table;
            this.PrimitiveProperties = primitiveProperties.Where(a => a != null).ToList().AsReadOnly();
            this.ComplexProperties = complexProperties.Where(a => a != null).ToList().AsReadOnly();
            this.CollectionProperties = collectionProperties.Where(a => a != null).ToList().AsReadOnly();
            this.Filters = filters.Where(a => a != null).ToList().AsReadOnly();
            this.Annotations = annotations.Where(a => a != null).ToList().AsReadOnly();
        }
        public Type Type { get; private set; }
        public DbTable Table { get; private set; }
        public ReadOnlyCollection<PrimitivePropertyDefinition> PrimitiveProperties { get; private set; }
        public ReadOnlyCollection<ComplexPropertyDefinition> ComplexProperties { get; private set; }
        public ReadOnlyCollection<CollectionPropertyDefinition> CollectionProperties { get; private set; }
        public ReadOnlyCollection<LambdaExpression> Filters { get; private set; }
        public ReadOnlyCollection<object> Annotations { get; private set; }
    }

}
