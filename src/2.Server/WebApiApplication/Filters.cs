namespace WebApiApplication;

public class FilterOption
{
    public Filter[] Filters { get; set; }
}
public class Filter
{
    public string Name { get; set; }
    public string[] Rules { get; set; }
    
}