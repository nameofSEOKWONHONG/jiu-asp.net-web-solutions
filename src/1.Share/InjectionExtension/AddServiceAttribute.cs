using System.Transactions;

namespace InjectionExtension;

public enum ENUM_LIFE_TIME_TYPE
{
    Singleton,
    Transient,
    Scope,
}

/// <summary>
/// reflection을 이용한 서비스 주입
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
public class AddServiceAttribute : Attribute
{
    public readonly ENUM_LIFE_TIME_TYPE LifeTimeType;
    public readonly Type InterfaceType;
    public AddServiceAttribute(ENUM_LIFE_TIME_TYPE lifeTimeType, Type interfaceType = null)
    {
        LifeTimeType = lifeTimeType;
        InterfaceType = interfaceType;
    }
}


