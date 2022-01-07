namespace Chloe.Core
{
    public class DbCommandInfo
    {
        public string CommandText { get; set; }
        public List<DbParam> Parameters { get; set; }

        public DbParam[] GetParameters()
        {
            return this.Parameters.ToArray();
        }
    }
}
