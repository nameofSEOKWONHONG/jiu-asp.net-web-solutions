namespace Chloe.Annotations
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TableAttribute : Attribute
    {
        public TableAttribute() { }
        public TableAttribute(string name) : this(name, null)
        {
        }
        public TableAttribute(string name, string schema)
        {
            this.Name = name;
            this.Schema = schema;
        }

        public string Name { get; set; }
        public string Schema { get; set; }
    }
}
