using System.Data;

namespace Chloe.Annotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ColumnAttribute : Attribute
    {
        public ColumnAttribute()
        {
        }
        public ColumnAttribute(string name)
        {
            this.Name = name;
        }
        public string Name { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsRowVersion { get; set; }
        /// <summary>
        /// -1 表示未指定确切的值，用该属性的时候务必做 -1 判断。
        /// </summary>
        public DbType DbType { get; set; } = (DbType)(-1); /* -1=Unspecified */
        /// <summary>
        /// -1 表示未指定确切的值，用该属性的时候务必做 -1 判断。
        /// </summary>
        public int Size { get; set; } = -1; /* -1=Unspecified */
        /// <summary>
        /// -1 表示未指定确切的值，用该属性的时候务必做 -1 判断。
        /// </summary>
        public int Scale { get; set; } = -1; /* -1=Unspecified */
        /// <summary>
        /// -1 表示未指定确切的值，用该属性的时候务必做 -1 判断。
        /// </summary>
        public int Precision { get; set; } = -1; /* -1=Unspecified */

        public bool HasDbType()
        {
            return (int)this.DbType != -1;
        }
        public DbType? GetDbType()
        {
            if (this.HasDbType())
                return this.DbType;

            return null;
        }

        public bool HasSize()
        {
            return this.Size != -1;
        }
        public int? GetSize()
        {
            if (this.HasSize())
                return this.Size;

            return null;
        }

        public bool HasScale()
        {
            return this.Scale != -1;
        }
        public byte? GetScale()
        {
            if (this.HasScale())
                return Convert.ToByte(this.Scale);

            return null;
        }

        public bool HasPrecision()
        {
            return this.Precision != -1;
        }
        public byte? GetPrecision()
        {
            if (this.HasPrecision())
                return Convert.ToByte(this.Precision);

            return null;
        }
    }
}
