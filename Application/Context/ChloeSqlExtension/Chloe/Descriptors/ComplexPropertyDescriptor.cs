using Chloe.Entity;
using Chloe.Exceptions;

namespace Chloe.Descriptors
{
    public class ComplexPropertyDescriptor : PropertyDescriptor
    {
        public ComplexPropertyDescriptor(ComplexPropertyDefinition definition, TypeDescriptor declaringTypeDescriptor) : base(definition, declaringTypeDescriptor)
        {
            this.Definition = definition;

            PrimitivePropertyDescriptor foreignKeyProperty = declaringTypeDescriptor.PrimitivePropertyDescriptors.Where(a => a.Property.Name == definition.ForeignKey).FirstOrDefault();

            if (foreignKeyProperty == null)
                throw new ChloeException($"Can not find property named '{definition.ForeignKey}'");

            this.ForeignKeyProperty = foreignKeyProperty;
        }

        public new ComplexPropertyDefinition Definition { get; private set; }
        public PrimitivePropertyDescriptor ForeignKeyProperty { get; private set; }
    }
}
