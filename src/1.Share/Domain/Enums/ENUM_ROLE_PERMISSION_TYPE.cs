using System.ComponentModel;
using System.Text.Json.Serialization;
using eXtensionSharp;

namespace Domain.Enums
{
    [TypeConverter(typeof (XEnumBaseTypeConverter<ENUM_ROLE_PERMISSION_TYPE>))]
    [JsonConverter(typeof(XEnumBaseJsonConverter<ENUM_ROLE_PERMISSION_TYPE>))]
    public class ENUM_ROLE_PERMISSION_TYPE : XEnumBase<ENUM_ROLE_PERMISSION_TYPE>
    {
        public static readonly ENUM_ROLE_PERMISSION_TYPE VIEW = Define("VIEW"); //readonly
        public static readonly ENUM_ROLE_PERMISSION_TYPE CREATE = Define("CREATE");
        public static readonly ENUM_ROLE_PERMISSION_TYPE UPDATE = Define("UPDATE");
        public static readonly ENUM_ROLE_PERMISSION_TYPE DELETE = Define("DELETE");
    }
}