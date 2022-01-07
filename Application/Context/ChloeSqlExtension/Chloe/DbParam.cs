using System.Data;

namespace Chloe
{
    public class DbParam
    {
        object _value;
        public DbParam()
        {
        }
        public DbParam(string name, object value)
        {
            this.Name = name;
            this.Value = value;
        }
        public DbParam(string name, object value, Type type) : this(name, value)
        {
            this.Type = type;
        }

        public string Name { get; set; }
        public object Value
        {
            get
            {
                return this._value;
            }
            set
            {
                this._value = value;
                if (value != null && value != DBNull.Value)
                    this.Type = value.GetType();
            }
        }
        public DbType? DbType { get; set; }
        public byte? Precision { get; set; }
        public byte? Scale { get; set; }
        public int? Size { get; set; }
        public Type Type { get; set; } = PublicConstants.TypeOfObject;
        public ParamDirection Direction { get; set; } = ParamDirection.Input;
        /// <summary>
        /// 如果设置了该自定义参数，框架内部就会忽视 DbParam 类的其他属性，使用该属性值
        /// </summary>
        public IDbDataParameter ExplicitParameter { get; set; }

        public static DbParam Create<T>(string name, T value)
        {
            var param = new DbParam(name, value);
            if (value == null)
                param.Type = typeof(T);
            return param;
        }
        public static DbParam Create(string name, object value)
        {
            return new DbParam(name, value);
        }
        public static DbParam Create(string name, object value, Type type)
        {
            return new DbParam(name, value, type);
        }
    }
}
