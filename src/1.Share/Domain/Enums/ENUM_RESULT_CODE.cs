using System.ComponentModel;
using System.Text.Json.Serialization;
using eXtensionSharp;

namespace Domain.Enums
{
    [TypeConverter(typeof (XEnumBaseTypeConverter<ENUM_RESULT_CODE>))]
    [JsonConverter(typeof(XEnumBaseJsonConverter<ENUM_RESULT_CODE>))]
    public class ENUM_RESULT_CODE : XEnumBase<ENUM_RESULT_CODE>
    {
        public static ENUM_RESULT_CODE OK = Define("0");
        public static ENUM_RESULT_CODE FAIL = Define("-1");
    }
}