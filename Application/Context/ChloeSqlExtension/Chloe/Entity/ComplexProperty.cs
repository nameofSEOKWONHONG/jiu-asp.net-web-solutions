using System.Reflection;

namespace Chloe.Entity
{
    public class ComplexProperty : PropertyBase
    {
        public ComplexProperty(PropertyInfo property) : base(property)
        {
        }

        public string ForeignKey { get; set; }

        public ComplexPropertyDefinition MakeDefinition()
        {
            ComplexPropertyDefinition definition = new ComplexPropertyDefinition(this.Property, this.Annotations, this.ForeignKey);
            return definition;
        }
    }
}
