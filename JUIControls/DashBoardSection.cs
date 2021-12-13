using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Application.Abstract;
using eXtensionSharp;
using JUIControls.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace JUIControls;

/// <summary>
/// Jennifersoft User Interface Framework Server Controller
/// Not Recommended Project. (Personal Project)
/// </summary>
public class DashBoardSection : ISectionMaker
{
    private readonly ISaleService _saleService;

    public DashBoardSection(ISaleService saleService)
    {
        _saleService = saleService;
    }
    
    public Section Create()
    {
        var section = new Section();
        section.MenuCode    = "DashBoard";
        section.MenuName    = "대시보드";
        section.SectionType = ENUM_SECTION_TYPE.SEARCH;
        section.Header      = CreateHeader();
        section.Body        = CreateBody();
        section.Footer      = CreateFooter();
        return section;
    }

    private List<WidgetGroup> CreateHeader()
    {   
        var controlGroups = new List<WidgetGroup>();
        var controls1 = new List<WidgetBase>()
        {
            new WidgetBase()
            {
                Index = 0,
                WigetName = "test",
                WidgetType = ENUM_WIDGET_TYPE.LABEL,
                Value = "test",
                ValueOptions = new Dictionary<string, string>()
                {
                    {"test1", "test1"},
                    {"test2", "test2"},
                    {"test3", "test3"},
                    {"test4", "test4"}
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
        };

        var controls2 = new List<WidgetBase>()
        {
            new SaleSelectBoxWidget(new SaleSelectBoxWidgetBinder(_saleService), 
                new Dictionary<string, string>()
                {
                    {"id", "1"},
                    {"ComCode", "300000"}
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
        };

        controlGroups.Add(new WidgetGroup(controls1));
        controlGroups.Add(new WidgetGroup(controls2));

        return controlGroups;
    }

    private List<WidgetGroup> CreateBody()
    {
        return null;
    }

    private List<WidgetGroup> CreateFooter()
    {
        return null;
    }
}

public class SearchSection : ISectionMaker
{
    public Section Create()
    {
        var section = new Section();
        section.Header = null;
        section.Body = CreateBody();
        section.Footer = null;
        return section;
    }

    private List<WidgetGroup> CreateHeader()
    {
        var group = new List<WidgetGroup>();
        group.Add(new WidgetGroup(new List<WidgetBase>()
        {
            new WidgetBase(),
            new WidgetBase(),
            new WidgetBase(),
            new WidgetBase(),
            new WidgetBase(),
        }));
        return group;
    }

    private List<WidgetGroup> CreateBody()
    {
        var group = new List<WidgetGroup>();

        var masterWidgetItems = new List<WidgetBase>();
        var detailWidgetItems = new List<WidgetBase>();
        
        group.Add(new WidgetGroup(masterWidgetItems) { IsMaster = true });
        group.Add(new WidgetGroup(detailWidgetItems));
        
        return group;
    }
}

