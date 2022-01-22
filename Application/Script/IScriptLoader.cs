namespace Application.Script;

public interface IScriptLoader
{
    double Version { get; set; }
    bool Reset(string fileName = null);
}