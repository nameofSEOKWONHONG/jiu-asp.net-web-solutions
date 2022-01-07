using System.Data;

namespace Chloe.DbExpressions
{
    [System.Diagnostics.DebuggerDisplay("Name = {Name}")]
    public class DbColumn
    {
        public DbColumn(string name, Type type)
            : this(name, type, null, null, null, null)
        {
        }
        public DbColumn(string name, Type type, DbType? dbType, int? size, byte? scale, byte? precision)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Column name could not be null or empty.");
            }

            this.Name = name;
            this.Type = type;
            this.DbType = dbType;
            this.Size = size;
            this.Scale = scale;
            this.Precision = precision;
        }

        public string Name { get; private set; }
        public Type Type { get; private set; }
        public DbType? DbType { get; private set; }
        public int? Size { get; private set; }
        public byte? Scale { get; private set; }
        public byte? Precision { get; private set; }

        public static DbColumn MakeColumn(DbExpression exp, string alias)
        {
            DbColumn column;
            DbColumnAccessExpression e = exp as DbColumnAccessExpression;
            if (e != null)
                column = new DbColumn(alias, e.Column.Type, e.Column.DbType, e.Column.Size, e.Column.Scale, e.Column.Precision);
            else
                column = new DbColumn(alias, exp.Type);

            return column;
        }
    }

}
