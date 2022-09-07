namespace OpenXmlSample.Data;

public sealed class Row
{
    public bool FirstRowEmpty { get; set; }
    public int Height { get; set; }
    public List<Col> Columns { get; set; } = new List<Col>();
}