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

public enum ENUM_LIFE_TYPE
{
    Singleton,
    Transient,
    Scope,
}