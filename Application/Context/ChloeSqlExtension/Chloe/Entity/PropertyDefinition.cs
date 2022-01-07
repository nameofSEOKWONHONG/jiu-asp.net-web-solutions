using System.Collections.ObjectModel;
using System.Reflection;

namespace Chloe.Entity
{
    public abstract class PropertyDefinition
    {
        protected PropertyDefinition(PropertyInfo property, IList<object> annotations)
        {
            PublicHelper.CheckNull(property, nameof(property));
            PublicHelper.CheckNull(annotations, nameof(annotations));

            this.Property = property;
            this.Annotations = annotations.Where(a => a != null).ToList().AsReadOnly();
        }
        public abstract TypeKind Kind { get; }
        public PropertyInfo Property { get; private set; }
        public ReadOnlyCollection<object> Annotations { get; private set; }
    }
}
