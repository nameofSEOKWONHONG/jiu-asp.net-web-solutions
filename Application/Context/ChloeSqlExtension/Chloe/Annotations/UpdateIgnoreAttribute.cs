namespace Chloe.Annotations
{
    /// <summary>
    /// 更新忽略
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class UpdateIgnoreAttribute : Attribute
    {
    }
}
