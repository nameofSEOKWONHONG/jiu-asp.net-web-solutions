using System.Collections.Generic;
using System.Linq;
using eXtensionSharp;

namespace JUIControls;

public class TestImpl
{
    public void Impl()
    {
        var uiTemplateControlGroup = new List<JUIControlGroup>();
        var uiTemplateControls1 = new List<JUIControl>()
        {
            new JUIControl()
            {
                Index = 0,
                ControlName = "test",
                BinderName = "test",
                ControlType = ENUM_CONTROL_TYPE.LABEL,
                Value = "test",
                ValueOptions = new Dictionary<string, string>()
                {
                    {"test1", "test1"},
                    {"test2", "test2"},
                    {"test3", "test3"},
                    {"test4", "test4"}
                }
            },
            new JUIControl()
            {
                Index = 1,
                ControlName = "test1",
                BinderName = "test1",
                ControlType = ENUM_CONTROL_TYPE.INPUT
            },
            new JUIControl()
            {
                Index = 2,
                ControlName = "test2",
                BinderName = "test2",
                ControlType = ENUM_CONTROL_TYPE.BUTTON
            }
        };

        var uiTemplateControls2 = new List<JUIControl>()
        {
            new JUIControl()
            {
                Index = 0,
                ControlName = "test",
                BinderName = "test",
                ControlType = ENUM_CONTROL_TYPE.LABEL
            },
            new JUIControl()
            {
                Index = 1,
                ControlName = "test1",
                BinderName = "test1",
                ControlType = ENUM_CONTROL_TYPE.INPUT
            },
            new JUIControl()
            {
                Index = 2,
                ControlName = "test2",
                BinderName = "test2",
                ControlType = ENUM_CONTROL_TYPE.BUTTON
            }
        };

        uiTemplateControlGroup.Add(new JUIControlGroup(uiTemplateControls1));
        uiTemplateControlGroup.Add(new JUIControlGroup(uiTemplateControls2));
        
        var section = new JUIControlSection();
        section.Header = uiTemplateControlGroup;
        section.Body = new List<JUIControlGroup>();
        section.Footer = new List<JUIControlGroup>();
    }
}