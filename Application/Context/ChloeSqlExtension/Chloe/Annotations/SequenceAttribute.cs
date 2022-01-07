namespace Chloe.Annotations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class SequenceAttribute : Attribute
    {
        public SequenceAttribute(string name) : this(name, null)
        {
        }
        public SequenceAttribute(string name, string schema)
        {
            this.Name = name;
            this.Schema = schema;
        }
        public string Name { get; private set; }
        public string Schema { get; set; }
    }
}
