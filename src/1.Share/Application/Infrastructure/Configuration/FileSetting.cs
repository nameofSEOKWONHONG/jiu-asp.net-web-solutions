using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Configuration;
using eXtensionSharp;
using Microsoft.Extensions.Options;

namespace Application.Infrastructure.Configuration;

public class FileSetting
{
    private FileFilterOptions _options;
    public FileSetting(IOptionsMonitor<FileFilterOptions> options)
    {
        _options = options.CurrentValue;
    }

    public IEnumerable<FileFilter> GetAllowFileFilters()
    {
        return _options.FileFilters;
    }

    public IEnumerable<FileFilter> GetAllowFileFilters(string code, string name = null)
    {
        Func<FileFilter, bool> codeWhere = e => e.Code == code;
        Func<FileFilter, bool> nameWhere = e => e.Name == name;
        var p = _options.FileFilters.Where(codeWhere);
        if (name.xIsNotEmpty()) p = p.Where(nameWhere);
        return p;
    }

    public FileFilter GetAllowFileFilter(string code)
    {
        return _options.FileFilters.First(m => m.Code == code);
    }

    public IEnumerable<string> GetAllowFileExtensions()
    {
        var list = new List<string>();
        _options.FileFilters.Select(m => m.Extensions).xForEach(item => list.AddRange(item));
        return list;
    }   
}