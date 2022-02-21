using System.ComponentModel;
using System.Text.Json.Serialization;
using eXtensionSharp;

namespace Domain.Enums;

[TypeConverter(typeof (XEnumBaseTypeConverter<ENUM_DATABASE_TYPE>))]
[JsonConverter(typeof(XEnumBaseJsonConverter<ENUM_DATABASE_TYPE>))]
public class ENUM_DATABASE_TYPE : XEnumBase<ENUM_DATABASE_TYPE>
{
    public static readonly ENUM_DATABASE_TYPE MSSQL = Define("MSSQL");
    public static readonly ENUM_DATABASE_TYPE MYSQL = Define("MYSQL");
    public static readonly ENUM_DATABASE_TYPE POSTGRES = Define("POSTGRES");
}