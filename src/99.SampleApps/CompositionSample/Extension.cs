using System.Diagnostics.CodeAnalysis;

namespace CompositionSample;

public static class Extension
{
    public static bool IsNull<T>(this T nullable)
    {
        if(nullable is null) return true;
        return false;
    }

    public static bool IsNotNull<T>(this T nullable)
    {
        if (nullable is not null) return true;
        return false;
    }
}