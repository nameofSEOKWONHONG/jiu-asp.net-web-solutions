using System;

namespace Application.Infrastructure.Injection;

public class ServiceLifeTimeAttribute : Attribute
{
    public readonly ENUM_LIFE_TYPE LifeType;
    public readonly Type TypeofInterface;
    public ServiceLifeTimeAttribute(ENUM_LIFE_TYPE lifeType, Type typeOfInterface = null)
    {
        LifeType = lifeType;
        TypeofInterface = typeOfInterface;
    }
}

public enum ENUM_LIFE_TYPE
{
    Singleton,
    Transient,
    Scope,
}