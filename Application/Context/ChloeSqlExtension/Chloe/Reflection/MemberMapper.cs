using System.Data;

namespace Chloe.Reflection
{
    public delegate void MemberMapper(object instance, IDataReader dataReader, int ordinal);
}
