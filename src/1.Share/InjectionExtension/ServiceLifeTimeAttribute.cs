using System.Reflection;
using System.Text;

namespace InjectionExtension;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
public class ServiceLifeTimeAttribute : Attribute
{
    public readonly ENUM_LIFE_TYPE LifeType;
    public readonly Type TypeOfInterface;
    public ServiceLifeTimeAttribute(ENUM_LIFE_TYPE lifeType, Type typeOfInterface = null)
    {
        LifeType = lifeType;
        TypeOfInterface = typeOfInterface;
    }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
public class ServiceLifeTimeFactoryAttribute : Attribute
{
    public readonly ENUM_LIFE_TYPE LifeType;
    public readonly Type Resolver;
    public readonly Type TypeOfInterface;
    public readonly Type ImplementType;
    public readonly MethodInfo MethodInfo; 

    public ServiceLifeTimeFactoryAttribute(ENUM_LIFE_TYPE lifeType, Type typeOfInterface, Type implementType, MethodInfo methodInfo)
    {
        LifeType = lifeType;
        TypeOfInterface = typeOfInterface;
        ImplementType = implementType;
        MethodInfo = methodInfo;
    }
}

public enum ENUM_LIFE_TYPE
{
    Singleton,
    Transient,
    Scope,
}