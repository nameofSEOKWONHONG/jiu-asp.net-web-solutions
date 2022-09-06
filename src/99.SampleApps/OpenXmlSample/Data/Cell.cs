namespace OpenXmlSample.Data;

public sealed class Cell
{
    public string Value { get; set; }
    public CellType CellType { get; set; }
    public AlignmentType AlignmentType { get; set; }
    public int CellRgba { get; set; }
}