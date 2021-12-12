using System.Collections.Generic;

namespace JUIControls;

public class JUIControlSection
{
    public List<JUIControlGroup> Header { get; set; }
    public List<JUIControlGroup> Body { get; set; }
    public List<JUIControlGroup> Footer { get; set; }
}