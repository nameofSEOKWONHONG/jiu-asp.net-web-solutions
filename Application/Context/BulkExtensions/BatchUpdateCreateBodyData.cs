﻿using EFCore.BulkExtensions.SqlAdapters;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace EFCore.BulkExtensions
{
    public class BatchUpdateCreateBodyData
    {
        private readonly BulkConfig _tableInfoBulkConfig;
        private readonly Dictionary<Type, TableInfo> _tableInfoLookup;

        public BatchUpdateCreateBodyData(
            string baseSql,
            DbContext dbContext,
            IEnumerable<object> innerParameters,
            IQueryable query,
            Type rootType,
            string tableAlias,
            LambdaExpression updateExpression)
        {
            BaseSql = baseSql;
            DatabaseType = SqlAdaptersMapping.GetDatabaseType(dbContext);
            DbContext = dbContext;
            Query = query;
            RootInstanceParameterName = updateExpression.Parameters?.First()?.Name;
            RootType = rootType;
            TableAlias = tableAlias;
            TableAliasesInUse = new List<string>();
            UpdateColumnsSql = new StringBuilder();
            UpdateExpression = updateExpression;

            _tableInfoBulkConfig = new BulkConfig();
            _tableInfoLookup = new Dictionary<Type, TableInfo>();

            var tableInfo = TableInfo.CreateInstance(dbContext, rootType, Array.Empty<object>(), OperationType.Read, _tableInfoBulkConfig);
            _tableInfoLookup.Add(rootType, tableInfo);

            SqlParameters = new List<object>(innerParameters);

            foreach (Match match in BatchUtil.TableAliasPattern.Matches(baseSql))
            {
                TableAliasesInUse.Add(match.Groups[2].Value);
            }
        }

        public string BaseSql { get; }
        public DbServer DatabaseType { get; }
        public DbContext DbContext { get; }
        public IQueryable Query { get; }
        public string RootInstanceParameterName { get; }
        public Type RootType { get; }
        public List<object> SqlParameters { get; }
        public string TableAlias { get; }
        public List<string> TableAliasesInUse { get; }
        public StringBuilder UpdateColumnsSql { get; }
        public LambdaExpression UpdateExpression { get; }

        public TableInfo GetTableInfoForType(Type typeToLookup)
        {
            if (_tableInfoLookup.TryGetValue(typeToLookup, out var tableInfo))
            {
                return tableInfo;
            }

            tableInfo = TableInfo.CreateInstance(DbContext, typeToLookup, Array.Empty<object>(), OperationType.Read, _tableInfoBulkConfig);
            if (tableInfo != null)
            {
                _tableInfoLookup.Add(typeToLookup, tableInfo);
            }

            return tableInfo;
        }
    }
}
