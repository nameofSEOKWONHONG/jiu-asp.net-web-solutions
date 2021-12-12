using System.Collections.Generic;
using System.Linq;
using eXtensionSharp;

namespace JUIControls;

public class JUIControlGroup
{
    public List<JUIControl> UITemplateControls { get; private set; }

    public JUIControlGroup(List<JUIControl> uiTemplateControls)
    {
        uiTemplateControls = uiTemplateControls;
    }

    public JUIControlGroup AddControl(int index, JUIControl juiControl)
    {
        this.UITemplateControls.Add(juiControl);
        return this;
    }

    public JUIControlGroup InsertControl(int index, JUIControl juiControl)
    {
        this.UITemplateControls.Insert(index, juiControl);
        return this;
    }

    public JUIControlGroup RemoveControl(JUIControl juiControl)
    {
        var exists = this.UITemplateControls.FirstOrDefault(m => m.ControlName == juiControl.ControlName);
        if (exists.xIsNotEmpty())
        {
            this.UITemplateControls.Remove(exists);
        }

        return this;
    }
}