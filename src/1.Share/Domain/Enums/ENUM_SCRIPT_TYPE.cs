using System.ComponentModel;
using System.Text.Json.Serialization;
using eXtensionSharp;

namespace Domain.Enums;
[JsonConverter(typeof(XEnumBaseJsonConverter<ENUM_SCRIPT_TYPE>))]
[TypeConverter(typeof (XEnumBaseTypeConverter<ENUM_SCRIPT_TYPE>))]
public class ENUM_SCRIPT_TYPE : XEnumBase<ENUM_SCRIPT_TYPE>
{
    public static readonly ENUM_SCRIPT_TYPE CSHARP = Define("CSHARP");
    public static readonly ENUM_SCRIPT_TYPE JS = Define("JS");
    public static readonly ENUM_SCRIPT_TYPE PY = Define("PY");
}