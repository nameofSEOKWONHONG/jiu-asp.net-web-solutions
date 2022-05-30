using System.Text;

namespace CompositionSample.B;

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

/// <summary>
/// composite example
/// </summary>
public class Child
{
    private readonly IPerson _person;
    public IPerson Person => _person;

    public Child(IPerson person)
    {
        if (person == null) person = new Julia();
        _person = person;
    }
}

public class SampleB
{
    public void Run()
    {
        var steve = new Steve();
        var julia = new Julia();
        var tom = new Child(steve);
        var jane = new Child(julia);
        
        Console.WriteLine($"steve height:{steve.Height}, personality:{steve.Personality}");
        Console.WriteLine($"julia height:{julia.Height}, personality:{julia.Personality}");
        Console.WriteLine($"tom height:{tom.Person.Height}, personality:{tom.Person.Personality}");
        Console.WriteLine($"jane height:{jane.Person.Height}, personality:{jane.Person.Personality}");
    }
}