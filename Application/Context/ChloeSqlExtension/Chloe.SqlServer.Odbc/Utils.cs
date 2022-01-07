using Chloe.Reflection;

namespace Chloe.SqlServer.Odbc
{
    internal static class Utils
    {
        static readonly HashSet<Type> ToStringableNumericTypes;

        static Utils()
        {
            ToStringableNumericTypes = new HashSet<Type>();
            ToStringableNumericTypes.Add(typeof(byte));
            ToStringableNumericTypes.Add(typeof(sbyte));
            ToStringableNumericTypes.Add(typeof(short));
            ToStringableNumericTypes.Add(typeof(ushort));
            ToStringableNumericTypes.Add(typeof(int));
            ToStringableNumericTypes.Add(typeof(uint));
            ToStringableNumericTypes.Add(typeof(long));
            ToStringableNumericTypes.Add(typeof(ulong));
            ToStringableNumericTypes.TrimExcess();
        }

        public static string QuoteName(string name)
        {
            return string.Concat("[", name, "]");
        }

        public static bool IsToStringableNumericType(Type type)
        {
            type = ReflectionExtension.GetUnderlyingType(type);
            return ToStringableNumericTypes.Contains(type);
        }
    }
}
