using System.ComponentModel;
using System.Text.Json.Serialization;
using eXtensionSharp;

namespace Domain.Enums;

[JsonConverter(typeof(XEnumBaseJsonConverter<ENUM_UPLOAD_TYPE>))]
[TypeConverter(typeof (XEnumBaseTypeConverter<ENUM_UPLOAD_TYPE>))]
public sealed class ENUM_UPLOAD_TYPE : XEnumBase<ENUM_UPLOAD_TYPE>
{
    public static readonly ENUM_UPLOAD_TYPE Product = Define("Images\\Products");

    public static readonly ENUM_UPLOAD_TYPE ProfilePicture = Define("Images\\ProfilePictures");

    public static readonly ENUM_UPLOAD_TYPE Document = Define("Docuemnts");
}