using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Application.Base;
using eXtensionSharp;
using JUIControlService.Binders;
using JUIControlService.Forms.Sale;
using JUIControlService.Widget;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace JUIControlService.Forms;

/// <summary>
/// Jennifersoft User Interface Framework Form Controller
/// Not Recommended Project. (.Personal Project)
/// </summary>
public class DashBoardForm : IFormMaker
{
    private readonly ISaleWidgetBinder _saleWidgetBinder;

    public DashBoardForm(ISaleWidgetBinder saleWidgetBinder)
    {
        _saleWidgetBinder = saleWidgetBinder;
    }
    
    public Form Create()
    {
        var form = new Form()
        {
            MenuCode = "DashBoard",
            MenuName = "대시보드",
            FormType = ENUM_FORM_TYPE.LIST
        };
        form.Header = CreateHeader();
        form.Body = CreateBody();
        form.Footer = CreateFooter();

        return form;
    }

    private FormHeader CreateHeader()
    {
        var header = new FormHeader();
        header.WidgetSectionGroups = new List<WidgetSectionGroup>()
        {
            new WidgetSectionGroup()
            {
                WidgetSections = new List<WidgetSection>()
                {
                    new WidgetSection()
                    {
                        WidgetGroups = new List<WidgetGroup>()
                        {
                            new WidgetGroup()
                            {
                                Widgets = new List<WidgetBase>()
                                {
                                    new WidgetBase()
                                    {
                                        Index = 0,
                                        WigetName = "test",
                                        WidgetType = ENUM_WIDGET_TYPE.LABEL,
                                        Value = "test",
                                        ValueOptions = new Dictionary<string, object>()
                                        {
                                            { "test1", "test1" },
                                            { "test2", "test2" },
                                            { "test3", "test3" },
                                            { "test4", "test4" }
                                        }
                                    },
                                    new WidgetBase()
                                    {
                                        Index = 1,
                                        WigetName = "test1",
                                        WidgetType = ENUM_WIDGET_TYPE.INPUT
                                    },
                                    new WidgetBase()
                                    {
                                        Index = 2,
                                        WigetName = "test2",
                                        WidgetType = ENUM_WIDGET_TYPE.BUTTON,
                                        NewLine = true
                                    }
                                }
                            },
                            new WidgetGroup()
                            {
                                Widgets = new List<WidgetBase>()
                                {
                                    new SaleSelectBoxWidget(new SaleSelectBoxWidgetBinder(_saleWidgetBinder),
                                        new Dictionary<string, string>()
                                        {
                                            { "id", "1" },
                                            { "ComCode", "300000" }
                                        })
                                    {
                                        Index = 0,
                                        WigetName = "test",
                                        WidgetType = ENUM_WIDGET_TYPE.LABEL
                                    },
                                    new WidgetBase()
                                    {
                                        Index = 1,
                                        WigetName = "test1",
                                        WidgetType = ENUM_WIDGET_TYPE.INPUT
                                    },
                                    new WidgetBase()
                                    {
                                        Index = 2,
                                        WigetName = "test2",
                                        WidgetType = ENUM_WIDGET_TYPE.BUTTON,
                                        NewLine = true
                                    },
                                }
                            }
                        }
                    }
                }
            }
        };

        return header;
    }

    private FormBody CreateBody()
    {
        return null;
    }

    private FormFooter CreateFooter()
    {
        return null;
    }
}

