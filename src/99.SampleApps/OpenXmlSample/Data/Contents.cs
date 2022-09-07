namespace OpenXmlSample.Data;

public sealed class Contents
{
    public IEnumerable<IEnumerable<String>> Values { get; set; }
    public IEnumerable<CellType> CellTypes { get; set; }
    public IEnumerable<AlignmentType> AlignmentTypes { get; set; }
}