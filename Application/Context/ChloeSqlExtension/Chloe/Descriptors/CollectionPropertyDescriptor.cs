using Chloe.Entity;

namespace Chloe.Descriptors
{
    public class CollectionPropertyDescriptor : PropertyDescriptor
    {
        public CollectionPropertyDescriptor(CollectionPropertyDefinition definition, TypeDescriptor declaringTypeDescriptor) : base(definition, declaringTypeDescriptor)
        {
            this.Definition = definition;
        }

        public new CollectionPropertyDefinition Definition { get; private set; }
        public Type ElementType { get { return this.Definition.ElementType; } }
    }
}
