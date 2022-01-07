namespace Chloe.Annotations
{
    /// <summary>
    /// Marks a property as navigation property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class NavigationAttribute : Attribute
    {
        public NavigationAttribute()
        {
        }
        public NavigationAttribute(string foreignKey)
        {
            this.ForeignKey = foreignKey;
        }

        public string ForeignKey { get; private set; }
    }
}
