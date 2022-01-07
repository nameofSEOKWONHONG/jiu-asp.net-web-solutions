using System.Reflection;

namespace Chloe.Entity
{
    public class ComplexPropertyDefinition : PropertyDefinition
    {
        public ComplexPropertyDefinition(PropertyInfo property, IList<object> annotations, string foreignKey) : base(property, annotations)
        {
            if (string.IsNullOrEmpty(foreignKey))
                throw new ArgumentException("'foreignKey' can not be null.");

            this.ForeignKey = foreignKey;
        }
        public override TypeKind Kind { get { return TypeKind.Complex; } }
        public string ForeignKey { get; private set; }
    }
}
