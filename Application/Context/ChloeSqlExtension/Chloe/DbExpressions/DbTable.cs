namespace Chloe.DbExpressions
{
    [System.Diagnostics.DebuggerDisplay("Name = {Name}")]
    public class DbTable
    {
        string _name;
        string _schema;
        public DbTable(string name)
            : this(name, null)
        {
        }
        public DbTable(string name, string schema)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Table name could not be null or empty.");
            }

            this._name = name;
            this._schema = schema;
        }

        public string Name { get { return this._name; } }
        public string Schema { get { return this._schema; } }
    }
}
