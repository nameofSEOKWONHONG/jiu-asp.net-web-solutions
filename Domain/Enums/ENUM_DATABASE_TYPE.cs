using eXtensionSharp;

namespace Domain.Enums;

public class ENUM_DATABASE_TYPE : XEnumBase<ENUM_DATABASE_TYPE>
{
    public static readonly ENUM_DATABASE_TYPE MSSQL = Define("MSSQL");
    public static readonly ENUM_DATABASE_TYPE MYSQL = Define("MYSQL");
    public static readonly ENUM_DATABASE_TYPE POSTGRES = Define("POSTGRES");
}