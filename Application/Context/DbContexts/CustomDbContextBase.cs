using eXtensionSharp;
using Microsoft.Extensions.Configuration;

namespace Application.Context;

public abstract class CustomDbContextBase
{
    protected readonly string ConnectionString;
    protected readonly ENUM_DATABASE_TYPE DatabaseType;
    
    protected CustomDbContextBase(ENUM_DATABASE_TYPE type, IConfiguration configuration)
    {
        DatabaseType = type;
        if(this.ConnectionString.xIsEmpty())
            ConnectionString = configuration.GetConnectionString(type.ToString());
    }
}