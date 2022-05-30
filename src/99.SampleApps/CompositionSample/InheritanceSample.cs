using System.Reflection.PortableExecutable;

namespace CompositionSample.A;

public class Steve : Person
{
    public Steve()
    {
        Height = 182.4;
        Personality = "Wild";
    }
}

public class Julia : Person
{
    public Julia()
    {
        Height = 165;
        Personality = "Safe";
    }
}

// can't multiple inheritance
// public class Tom : Steve, Julia
// {
//     
// }

public class Child : Steve
{
    public Child()
    {
        
    }
}

public class SampleA
{
    public void Run()
    {
        var steve = new Steve();
        var julia = new Julia();
        var tom = new Child();
        var jane = new Child();
        
        Console.WriteLine($"steve height:{steve.Height}, personality:{steve.Personality}");
        Console.WriteLine($"julia height:{julia.Height}, personality:{julia.Personality}");
        Console.WriteLine($"tom height:{tom.Height}, personality:{tom.Personality}");
        Console.WriteLine($"jane height:{jane.Height}, personality:{jane.Personality}");
    }
}