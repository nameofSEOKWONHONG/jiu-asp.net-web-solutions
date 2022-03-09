namespace SpectreConsoleApplication;



public enum ENUM_SAMPLE
{
    A,
    B
}

public delegate ISample SampleResolver(ENUM_SAMPLE type);

public interface ISample
{
}

//[ServiceLifeTimeFactory(ENUM_LIFE_TYPE.Singleton, typeof(SampleResolver), typeof(SampleA))]
public class SampleA : ISample
{
}

//[ServiceLifeTimeFactory(ENUM_LIFE_TYPE.Singleton, typeof(SampleResolver), typeof(SampleB))]
public class SampleB : ISample
{
    
}