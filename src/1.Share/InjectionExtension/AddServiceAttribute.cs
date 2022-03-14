using System.Reflection;
using System.Text;

namespace InjectionExtension;

/// <summary>
/// reflection을 이용한 서비스 주입
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
public class AddServiceAttribute : Attribute
{
    public readonly ENUM_LIFE_TIME_TYPE LifeTimeType;
    public readonly Type TypeOfInterface;
    public AddServiceAttribute(ENUM_LIFE_TIME_TYPE lifeTimeType, Type typeOfInterface = null)
    {
        LifeTimeType = lifeTimeType;
        TypeOfInterface = typeOfInterface;
    }
}

public enum ENUM_LIFE_TIME_TYPE
{
    Singleton,
    Transient,
    Scope,
}