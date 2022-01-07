namespace Chloe.Annotations
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class DbFunctionAttribute : Attribute
    {
        public DbFunctionAttribute()
        {
        }
        public DbFunctionAttribute(string name)
        {
            this.Name = name;
        }
        public string Name { get; set; }
        public string Schema { get; set; }
    }
}
