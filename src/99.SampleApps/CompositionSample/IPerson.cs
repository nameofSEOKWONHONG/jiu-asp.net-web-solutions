using System.Diagnostics.CodeAnalysis;

namespace CompositionSample;

public interface IPerson
{
    double Height { get; set; }
    string Personality { get; set; } 
}

public class Person : IPerson
{
    public double Height { get; set; }
    [AllowNull] // = default!; //is same thing.
    public string Personality { get; set; }
}