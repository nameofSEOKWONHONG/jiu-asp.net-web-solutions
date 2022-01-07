namespace Chloe.PostgreSQL
{
    class SqlGenerator_ConvertToLowercase : SqlGenerator
    {
        internal override void QuoteName(string name)
        {
            base.QuoteName(name.ToLower());
        }
    }
}
