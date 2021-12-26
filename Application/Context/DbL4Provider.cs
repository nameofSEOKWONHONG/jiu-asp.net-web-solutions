using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using eXtensionSharp;
using Microsoft.Extensions.Configuration;

namespace Application.Context
{
    public class DbL4Provider
    {
        private readonly Dictionary<string, string> _tableAttributeKeyValues;
        private readonly IConfiguration _configuration;
        public DbL4Provider(IConfiguration configuration)
        {
            _configuration = configuration;
            
            this._tableAttributeKeyValues = new Dictionary<string, string>();

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes().Where(type => type.GetCustomAttribute<TableAttribute>() != null))
                {
                    var tableAttribute = type.GetCustomAttribute<TableAttribute>();
                    string tableName = tableAttribute.Name; //whatever the field the of the tablename is

                    _tableAttributeKeyValues.Add($"[{tableName}]", $"[{tableName}_L4]");
                }
            }
        }

        public string ReplaceTable(string commandText)
        {
            var enableL4 = _configuration["L4Enable"].xValue<bool>(false);
            if (!enableL4) return commandText;
            
            foreach (var tableAttributeKeyValue in _tableAttributeKeyValues)
            {
                commandText = commandText.Replace(tableAttributeKeyValue.Key, tableAttributeKeyValue.Value);
            }

            return commandText;
        }
    }
}