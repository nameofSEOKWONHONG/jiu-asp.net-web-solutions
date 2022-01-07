namespace Chloe.Infrastructure
{
    /// <summary>
    /// 数据库数据转换器。
    /// </summary>
    public interface IDbValueConverter
    {
        /// <summary>
        /// 从数据库读取数据时，当表字段类型与实体属性类型不一致时调用。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        object Convert(object value);
    }
    public class DbValueConverter : IDbValueConverter
    {
        Type _type;
        public DbValueConverter(Type type)
        {
            this._type = type;
        }
        public virtual object Convert(object value)
        {
            return System.Convert.ChangeType(value, this._type);
        }
    }
    public class DbValueConverter<T> : DbValueConverter, IDbValueConverter
    {
        public DbValueConverter() : base(typeof(T))
        {
        }
    }
    internal class InternalDbValueConverter : IDbValueConverter
    {
        Func<object, object> _converter;
        public InternalDbValueConverter(Func<object, object> converter)
        {
            this._converter = converter;
        }
        public object Convert(object value)
        {
            return this._converter(value);
        }
    }

    public class Byte_ValueConverter : DbValueConverter<Byte>
    {
        public override object Convert(object value)
        {
            return System.Convert.ToByte(value);
        }
    }
    public class SByte_ValueConverter : DbValueConverter<SByte>
    {
        public override object Convert(object value)
        {
            return System.Convert.ToSByte(value);
        }
    }
    public class Int16_ValueConverter : DbValueConverter<Int16>
    {
        public override object Convert(object value)
        {
            return System.Convert.ToInt16(value);
        }
    }
    public class UInt16_ValueConverter : DbValueConverter<UInt16>
    {
        public override object Convert(object value)
        {
            return System.Convert.ToUInt16(value);
        }
    }
    public class Int32_ValueConverter : DbValueConverter<Int32>
    {
        public override object Convert(object value)
        {
            return System.Convert.ToInt32(value);
        }
    }
    public class UInt32_ValueConverter : DbValueConverter<UInt32>
    {
        public override object Convert(object value)
        {
            return System.Convert.ToUInt32(value);
        }
    }
    public class Int64_ValueConverter : DbValueConverter<Int64>
    {
        public override object Convert(object value)
        {
            return System.Convert.ToInt64(value);
        }
    }
    public class UInt64_ValueConverter : DbValueConverter<UInt64>
    {
        public override object Convert(object value)
        {
            return System.Convert.ToUInt64(value);
        }
    }
    public class Single_ValueConverter : DbValueConverter<Single>
    {
        public override object Convert(object value)
        {
            return System.Convert.ToSingle(value);
        }
    }
    public class Double_ValueConverter : DbValueConverter<Double>
    {
        public override object Convert(object value)
        {
            return System.Convert.ToDouble(value);
        }
    }
    public class Decimal_ValueConverter : DbValueConverter<Decimal>
    {
        public override object Convert(object value)
        {
            return System.Convert.ToDecimal(value);
        }
    }
    public class Boolean_ValueConverter : DbValueConverter<Boolean>
    {
        public override object Convert(object value)
        {
            return System.Convert.ToBoolean(value);
        }
    }
    public class String_ValueConverter : DbValueConverter<String>
    {
        public override object Convert(object value)
        {
            return System.Convert.ToString(value);
        }
    }
    public class Guid_ValueConverter : DbValueConverter<Guid>
    {
        public override object Convert(object value)
        {
            Type valueType = value.GetType();
            if (valueType == typeof(string))
                return Guid.Parse((string)value);

            if (valueType == typeof(byte[]))
                return new Guid((byte[])value);

            return base.Convert(value);
        }
    }
    public class DateTime_ValueConverter : DbValueConverter<DateTime>
    {
        public override object Convert(object value)
        {
            return System.Convert.ToDateTime(value);
        }
    }
    public class DateTimeOffset_ValueConverter : DbValueConverter<DateTimeOffset>
    {
        public override object Convert(object value)
        {
            return base.Convert(value);
        }
    }
    public class TimeSpan_ValueConverter : DbValueConverter<TimeSpan>
    {
        public override object Convert(object value)
        {
            return base.Convert(value);
        }
    }
    public class Binary_ValueConverter : DbValueConverter<DateTimeOffset>
    {
        public override object Convert(object value)
        {
            return base.Convert(value);
        }
    }
}
