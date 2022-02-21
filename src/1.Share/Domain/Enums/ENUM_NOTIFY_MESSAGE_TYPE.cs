using System.ComponentModel;
using System.Text.Json.Serialization;
using eXtensionSharp;

namespace Domain.Enums
{
    [TypeConverter(typeof (XEnumBaseTypeConverter<ENUM_NOTIFY_MESSAGE_TYPE>))]
    [JsonConverter(typeof(XEnumBaseJsonConverter<ENUM_NOTIFY_MESSAGE_TYPE>))]
    public sealed class ENUM_NOTIFY_MESSAGE_TYPE : XEnumBase<ENUM_NOTIFY_MESSAGE_TYPE>
    {
        public static readonly ENUM_NOTIFY_MESSAGE_TYPE SMS = Define("SMS");
        public static readonly ENUM_NOTIFY_MESSAGE_TYPE EMAIL = Define("EMAIL");
        public static readonly ENUM_NOTIFY_MESSAGE_TYPE KAKAO = Define("KAKAO");
        public static readonly ENUM_NOTIFY_MESSAGE_TYPE LINE  = Define("LINE");
        public static readonly ENUM_NOTIFY_MESSAGE_TYPE ALL   = Define("ALL");
    }
}