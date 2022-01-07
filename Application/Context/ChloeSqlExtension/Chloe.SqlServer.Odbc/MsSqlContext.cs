using Chloe.Core;
using Chloe.Core.Visitors;
using Chloe.DbExpressions;
using Chloe.Descriptors;
using Chloe.Exceptions;
using Chloe.Infrastructure;
using Chloe.Reflection;
using Chloe.Threading.Tasks;
using Chloe.Utility;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Chloe.RDBMS;

#if net5
using Microsoft.Data.SqlClient;
#else
using System.Data.SqlClient;
#endif

namespace Chloe.SqlServer.Odbc
{
    public partial class MsSqlContext : DbContext
    {
        DatabaseProvider _databaseProvider;
        public MsSqlContext(string connString)
            : this(new DefaultDbConnectionFactory(connString))
        {
        }

        public MsSqlContext(IDbConnectionFactory dbConnectionFactory)
        {
            PublicHelper.CheckNull(dbConnectionFactory);

            this.PagingMode = PagingMode.ROW_NUMBER;
            this._databaseProvider = new DatabaseProvider(dbConnectionFactory, this);
        }
        public MsSqlContext(Func<IDbConnection> dbConnectionFactory) : this(new DbConnectionFactory(dbConnectionFactory))
        {
        }


        /// <summary>
        /// 设置方法解析器。
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="handler"></param>
        public static void SetMethodHandler(string methodName, IMethodHandler handler)
        {
            PublicHelper.CheckNull(methodName, nameof(methodName));
            PublicHelper.CheckNull(handler, nameof(handler));
            lock (SqlGenerator.MethodHandlers)
            {
                SqlGenerator.MethodHandlers[methodName] = handler;
            }
        }

        /// <summary>
        /// 分页模式。
        /// </summary>
        public PagingMode PagingMode { get; set; }
        public override IDatabaseProvider DatabaseProvider
        {
            get { return this._databaseProvider; }
        }

        protected override async Task<TEntity> Insert<TEntity>(TEntity entity, string table, bool @async)
        {
            PublicHelper.CheckNull(entity);

            TypeDescriptor typeDescriptor = EntityTypeContainer.GetDescriptor(typeof(TEntity));

            DbTable dbTable = PublicHelper.CreateDbTable(typeDescriptor, table);

            Dictionary<PrimitivePropertyDescriptor, object> keyValueMap = PrimaryKeyHelper.CreateKeyValueMap(typeDescriptor);

            Dictionary<PrimitivePropertyDescriptor, DbExpression> insertColumns = new Dictionary<PrimitivePropertyDescriptor, DbExpression>();
            List<PrimitivePropertyDescriptor> outputColumns = new List<PrimitivePropertyDescriptor>();
            foreach (PrimitivePropertyDescriptor propertyDescriptor in typeDescriptor.PrimitivePropertyDescriptors)
            {
                if (propertyDescriptor.IsAutoIncrement || propertyDescriptor.IsTimestamp())
                {
                    outputColumns.Add(propertyDescriptor);
                    continue;
                }

                if (propertyDescriptor.HasSequence())
                {
                    DbMethodCallExpression getNextValueForSequenceExp = PublicHelper.MakeNextValueForSequenceDbExpression(propertyDescriptor, dbTable.Schema);
                    insertColumns.Add(propertyDescriptor, getNextValueForSequenceExp);
                    outputColumns.Add(propertyDescriptor);
                    continue;
                }

                object val = propertyDescriptor.GetValue(entity);

                PublicHelper.NotNullCheck(propertyDescriptor, val);

                if (propertyDescriptor.IsPrimaryKey)
                {
                    keyValueMap[propertyDescriptor] = val;
                }

                DbExpression valExp = DbExpression.Parameter(val, propertyDescriptor.PropertyType, propertyDescriptor.Column.DbType);
                insertColumns.Add(propertyDescriptor, valExp);
            }

            PrimitivePropertyDescriptor nullValueKey = keyValueMap.Where(a => a.Value == null && !a.Key.IsAutoIncrement).Select(a => a.Key).FirstOrDefault();
            if (nullValueKey != null)
            {
                /* 主键为空并且主键又不是自增列 */
                throw new ChloeException(string.Format("The primary key '{0}' could not be null.", nullValueKey.Property.Name));
            }

            DbInsertExpression insertExp = new DbInsertExpression(dbTable);

            foreach (var kv in insertColumns)
            {
                insertExp.InsertColumns.Add(kv.Key.Column, kv.Value);
            }

            if (outputColumns.Count == 0)
            {
                await this.ExecuteNonQuery(insertExp, @async);
                return entity;
            }

            List<Action<TEntity, IDataReader>> mappers = new List<Action<TEntity, IDataReader>>();
            IDbExpressionTranslator translator = this.DatabaseProvider.CreateDbExpressionTranslator();
            DbCommandInfo dbCommandInfo;
            if (outputColumns.Count == 1 && outputColumns[0].IsAutoIncrement)
            {
                dbCommandInfo = translator.Translate(insertExp);

                /* 自增 id 不能用 output  inserted.Id 输出，因为如果表设置了触发器的话会报错 */
                dbCommandInfo.CommandText = string.Concat(dbCommandInfo.CommandText, ";", this.GetSelectLastInsertIdClause());
                mappers.Add(GetMapper<TEntity>(outputColumns[0], 0));
            }
            else
            {
                foreach (PrimitivePropertyDescriptor outputColumn in outputColumns)
                {
                    mappers.Add(GetMapper<TEntity>(outputColumn, insertExp.Returns.Count));
                    insertExp.Returns.Add(outputColumn.Column);
                }

                dbCommandInfo = translator.Translate(insertExp);
            }

            IDataReader dataReader = this.Session.ExecuteReader(dbCommandInfo.CommandText, dbCommandInfo.GetParameters());
            using (dataReader)
            {
                dataReader.Read();
                foreach (var mapper in mappers)
                {
                    mapper(entity, dataReader);
                }
            }

            return entity;
        }
        protected override async Task<object> Insert<TEntity>(Expression<Func<TEntity>> content, string table, bool @async)
        {
            PublicHelper.CheckNull(content);

            TypeDescriptor typeDescriptor = EntityTypeContainer.GetDescriptor(typeof(TEntity));

            if (typeDescriptor.PrimaryKeys.Count > 1)
            {
                /* 对于多主键的实体，暂时不支持调用这个方法进行插入 */
                throw new NotSupportedException(string.Format("Can not call this method because entity '{0}' has multiple keys.", typeDescriptor.Definition.Type.FullName));
            }

            PrimitivePropertyDescriptor keyPropertyDescriptor = typeDescriptor.PrimaryKeys.FirstOrDefault();

            Dictionary<MemberInfo, Expression> insertColumns = InitMemberExtractor.Extract(content);

            DbTable dbTable = PublicHelper.CreateDbTable(typeDescriptor, table);

            DefaultExpressionParser expressionParser = typeDescriptor.GetExpressionParser(dbTable);
            DbInsertExpression insertExp = new DbInsertExpression(dbTable);

            object keyVal = null;

            foreach (var kv in insertColumns)
            {
                MemberInfo key = kv.Key;
                PrimitivePropertyDescriptor propertyDescriptor = typeDescriptor.GetPrimitivePropertyDescriptor(key);

                if (propertyDescriptor.IsAutoIncrement)
                    throw new ChloeException(string.Format("Could not insert value into the identity column '{0}'.", propertyDescriptor.Column.Name));

                if (propertyDescriptor.HasSequence())
                    throw new ChloeException(string.Format("Can not insert value into the column '{0}', because it's mapping member has define a sequence.", propertyDescriptor.Column.Name));

                if (propertyDescriptor.IsPrimaryKey)
                {
                    object val = ExpressionEvaluator.Evaluate(kv.Value);
                    if (val == null)
                        throw new ChloeException(string.Format("The primary key '{0}' could not be null.", propertyDescriptor.Property.Name));
                    else
                    {
                        keyVal = val;
                        insertExp.InsertColumns.Add(propertyDescriptor.Column, DbExpression.Parameter(keyVal, propertyDescriptor.PropertyType, propertyDescriptor.Column.DbType));
                        continue;
                    }
                }

                insertExp.InsertColumns.Add(propertyDescriptor.Column, expressionParser.Parse(kv.Value));
            }

            foreach (var item in typeDescriptor.PrimitivePropertyDescriptors.Where(a => a.HasSequence()))
            {
                DbMethodCallExpression getNextValueForSequenceExp = PublicHelper.MakeNextValueForSequenceDbExpression(item, dbTable.Schema);
                insertExp.InsertColumns.Add(item.Column, getNextValueForSequenceExp);
            }

            if (keyPropertyDescriptor != null)
            {
                //主键为空并且主键又不是自增列
                if (keyVal == null && !keyPropertyDescriptor.IsAutoIncrement && !keyPropertyDescriptor.HasSequence())
                {
                    throw new ChloeException(string.Format("The primary key '{0}' could not be null.", keyPropertyDescriptor.Property.Name));
                }
            }

            if (keyPropertyDescriptor == null)
            {
                await this.ExecuteNonQuery(insertExp, @async);
                return keyVal; /* It will return null if an entity does not define primary key. */
            }
            if (!keyPropertyDescriptor.IsAutoIncrement && !keyPropertyDescriptor.HasSequence())
            {
                await this.ExecuteNonQuery(insertExp, @async);
                return keyVal;
            }

            IDbExpressionTranslator translator = this.DatabaseProvider.CreateDbExpressionTranslator();
            DbCommandInfo dbCommandInfo = translator.Translate(insertExp);

            if (keyPropertyDescriptor.IsAutoIncrement)
            {
                /* 自增 id 不能用 output  inserted.Id 输出，因为如果表设置了触发器的话会报错 */
                dbCommandInfo.CommandText = string.Concat(dbCommandInfo.CommandText, ";", this.GetSelectLastInsertIdClause());
            }
            else if (keyPropertyDescriptor.HasSequence())
            {
                insertExp.Returns.Add(keyPropertyDescriptor.Column);
            }

            object ret = this.Session.ExecuteScalar(dbCommandInfo.CommandText, dbCommandInfo.GetParameters());
            if (ret == null || ret == DBNull.Value)
            {
                throw new ChloeException("Unable to get the identity/sequence value.");
            }

            ret = PublicHelper.ConvertObjectType(ret, typeDescriptor.AutoIncrement.PropertyType);
            return ret;
        }
        protected override async Task InsertRange<TEntity>(List<TEntity> entities, string table, bool @async)
        {
            /*
             * 将 entities 分批插入数据库
             * 每批生成 insert into TableName(...) values(...),(...)... 
             * 该方法相对循环一条一条插入，速度提升 2/3 这样
             */

            PublicHelper.CheckNull(entities);
            if (entities.Count == 0)
                return;

            int maxParameters = 2100;
            int batchSize = 50; /* 每批实体大小，此值通过测试得出相对插入速度比较快的一个值 */

            TypeDescriptor typeDescriptor = EntityTypeContainer.GetDescriptor(typeof(TEntity));

            List<PrimitivePropertyDescriptor> mappingPropertyDescriptors = typeDescriptor.PrimitivePropertyDescriptors.Where(a => a.IsAutoIncrement == false && !a.IsTimestamp()).ToList();
            int maxDbParamsCount = maxParameters - mappingPropertyDescriptors.Count; /* 控制一个 sql 的参数个数 */

            DbTable dbTable = PublicHelper.CreateDbTable(typeDescriptor, table);
            string sqlTemplate = AppendInsertRangeSqlTemplate(dbTable, mappingPropertyDescriptors);

            Func<Task> insertAction = async () =>
            {
                int batchCount = 0;
                List<DbParam> dbParams = new List<DbParam>();
                StringBuilder sqlBuilder = new StringBuilder();
                for (int i = 0; i < entities.Count; i++)
                {
                    var entity = entities[i];

                    if (batchCount > 0)
                        sqlBuilder.Append(",");

                    sqlBuilder.Append("(");
                    for (int j = 0; j < mappingPropertyDescriptors.Count; j++)
                    {
                        if (j > 0)
                            sqlBuilder.Append(",");

                        PrimitivePropertyDescriptor mappingPropertyDescriptor = mappingPropertyDescriptors[j];

                        if (mappingPropertyDescriptor.HasSequence())
                        {
                            string sequenceSchema = mappingPropertyDescriptor.Definition.SequenceSchema;
                            sequenceSchema = string.IsNullOrEmpty(sequenceSchema) ? dbTable.Schema : sequenceSchema;

                            sqlBuilder.Append("NEXT VALUE FOR ");
                            if (!string.IsNullOrEmpty(sequenceSchema))
                            {
                                sqlBuilder.Append(Utils.QuoteName(sequenceSchema));
                                sqlBuilder.Append(".");
                            }
                            sqlBuilder.Append(Utils.QuoteName(mappingPropertyDescriptor.Definition.SequenceName));
                            continue;
                        }

                        object val = mappingPropertyDescriptor.GetValue(entity);

                        PublicHelper.NotNullCheck(mappingPropertyDescriptor, val);

                        if (val == null)
                        {
                            sqlBuilder.Append("NULL");
                            continue;
                        }

                        Type valType = val.GetType();
                        if (valType.IsEnum)
                        {
                            val = Convert.ChangeType(val, Enum.GetUnderlyingType(valType));
                            valType = val.GetType();
                        }

                        if (Utils.IsToStringableNumericType(valType))
                        {
                            sqlBuilder.Append(val.ToString());
                            continue;
                        }

                        if (val is bool)
                        {
                            if ((bool)val == true)
                                sqlBuilder.AppendFormat("1");
                            else
                                sqlBuilder.AppendFormat("0");
                            continue;
                        }

                        string paramName = UtilConstants.ParameterNamePrefix + dbParams.Count.ToString();
                        DbParam dbParam = new DbParam(paramName, val) { DbType = mappingPropertyDescriptor.Column.DbType };
                        //解决日期类型报精度溢出错误问题
                        if (val.GetType() == PublicConstants.TypeOfDateTime)
                        {
                            
                            dbParam.Precision = 23;
                            dbParam.Scale = 3;
                        }
                        dbParams.Add(dbParam);
                        sqlBuilder.Append("?");
                    }
                    sqlBuilder.Append(")");

                    batchCount++;

                    if ((batchCount >= 20 && dbParams.Count >= 120/*参数个数太多也会影响速度*/) || dbParams.Count >= maxDbParamsCount || batchCount >= batchSize || (i + 1) == entities.Count)
                    {
                        sqlBuilder.Insert(0, sqlTemplate);
                        string sql = sqlBuilder.ToString();
                        await this.Session.ExecuteNonQuery(sql, dbParams.ToArray(), @async);

                        sqlBuilder.Clear();
                        dbParams.Clear();
                        batchCount = 0;
                    }
                }
            };

            Func<Task> fAction = insertAction;

            if (this.Session.IsInTransaction)
            {
                await fAction();
                return;
            }

            /* 因为分批插入，所以需要开启事务保证数据一致性 */
            using (var tran = this.BeginTransaction())
            {
                await fAction();
                tran.Commit();
            }
        }

        /// <summary>
        /// 利用 SqlBulkCopy 批量插入数据。
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        /// <param name="table"></param>
        /// <param name="batchSize">设置 SqlBulkCopy.BatchSize 的值</param>
        /// <param name="bulkCopyTimeout">设置 SqlBulkCopy.BulkCopyTimeout 的值</param>
        /// <param name="keepIdentity">是否保留源自增值。false 由数据库分配自增值</param>
        public virtual void BulkInsert<TEntity>(List<TEntity> entities, string table = null, int? batchSize = null, int? bulkCopyTimeout = null, bool keepIdentity = false)
        {
            this.BulkInsert(entities, table, batchSize, bulkCopyTimeout, keepIdentity, false).GetResult();
        }
        public virtual async Task BulkInsertAsync<TEntity>(List<TEntity> entities, string table = null, int? batchSize = null, int? bulkCopyTimeout = null, bool keepIdentity = false)
        {
            await this.BulkInsert(entities, table, batchSize, bulkCopyTimeout, keepIdentity, true);
        }
        protected virtual async Task BulkInsert<TEntity>(List<TEntity> entities, string table, int? batchSize, int? bulkCopyTimeout, bool keepIdentity, bool @async)
        {
            PublicHelper.CheckNull(entities);

            TypeDescriptor typeDescriptor = EntityTypeContainer.GetDescriptor(typeof(TEntity));

            DataTable dtToWrite = ConvertToSqlBulkCopyDataTable(entities, typeDescriptor);

            bool shouldCloseConnection = false;
            SqlConnection conn = this.Session.CurrentConnection as SqlConnection;
            try
            {
                SqlTransaction externalTransaction = null;
                if (this.Session.IsInTransaction)
                {
                    externalTransaction = this.Session.CurrentTransaction as SqlTransaction;
                }

                SqlBulkCopyOptions sqlBulkCopyOptions = SqlBulkCopyOptions.CheckConstraints | SqlBulkCopyOptions.KeepNulls | SqlBulkCopyOptions.FireTriggers;
                if (keepIdentity)
                    sqlBulkCopyOptions = SqlBulkCopyOptions.KeepIdentity | sqlBulkCopyOptions;

                SqlBulkCopy sbc = new SqlBulkCopy(conn, sqlBulkCopyOptions, externalTransaction);

                using (sbc)
                {
                    for (int i = 0; i < dtToWrite.Columns.Count; i++)
                    {
                        var column = dtToWrite.Columns[i];
                        sbc.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                    }

                    if (batchSize != null)
                        sbc.BatchSize = batchSize.Value;

                    DbTable dbTable = PublicHelper.CreateDbTable(typeDescriptor, table);
                    sbc.DestinationTableName = AppendTableName(dbTable);

                    if (bulkCopyTimeout != null)
                        sbc.BulkCopyTimeout = bulkCopyTimeout.Value;

                    if (conn.State != ConnectionState.Open)
                    {
                        conn.Open();
                        shouldCloseConnection = true;
                    }

                    if (@async)
                        await sbc.WriteToServerAsync(dtToWrite);
                    else
                        sbc.WriteToServer(dtToWrite);
                }
            }
            finally
            {
                if (conn != null)
                {
                    if (shouldCloseConnection && conn.State == ConnectionState.Open)
                        conn.Close();
                }
            }
        }


        protected override async Task<int> Update<TEntity>(TEntity entity, string table, bool @async)
        {
            PublicHelper.CheckNull(entity);

            TypeDescriptor typeDescriptor = EntityTypeContainer.GetDescriptor(typeof(TEntity));
            PublicHelper.EnsureHasPrimaryKey(typeDescriptor);

            PairList<PrimitivePropertyDescriptor, object> keyValues = new PairList<PrimitivePropertyDescriptor, object>(typeDescriptor.PrimaryKeys.Count);

            IEntityState entityState = this.TryGetTrackedEntityState(entity);
            Dictionary<PrimitivePropertyDescriptor, DbExpression> updateColumns = new Dictionary<PrimitivePropertyDescriptor, DbExpression>();

            foreach (PrimitivePropertyDescriptor propertyDescriptor in typeDescriptor.PrimitivePropertyDescriptors)
            {
                if (propertyDescriptor.IsPrimaryKey)
                {
                    var keyValue = propertyDescriptor.GetValue(entity);
                    PrimaryKeyHelper.KeyValueNotNull(propertyDescriptor, keyValue);
                    keyValues.Add(propertyDescriptor, keyValue);
                    continue;
                }

                if (propertyDescriptor.CannotUpdate())
                    continue;

                object val = propertyDescriptor.GetValue(entity);
                PublicHelper.NotNullCheck(propertyDescriptor, val);

                if (entityState != null && !entityState.HasChanged(propertyDescriptor, val))
                    continue;

                DbExpression valExp = DbExpression.Parameter(val, propertyDescriptor.PropertyType, propertyDescriptor.Column.DbType);
                updateColumns.Add(propertyDescriptor, valExp);
            }

            PrimitivePropertyDescriptor rowVersionDescriptor = null;
            object rowVersionValue = null;
            object rowVersionNewValue = null;
            if (typeDescriptor.HasRowVersion())
            {
                rowVersionDescriptor = typeDescriptor.RowVersion;
                if (rowVersionDescriptor.IsTimestamp())
                {
                    rowVersionValue = rowVersionDescriptor.GetValue(entity);
                    this.EnsureRowVersionValueIsNotNull(rowVersionValue);
                    keyValues.Add(rowVersionDescriptor, rowVersionValue);
                }
                else
                {
                    rowVersionValue = rowVersionDescriptor.GetValue(entity);
                    rowVersionNewValue = PublicHelper.IncreaseRowVersionNumber(rowVersionValue);
                    updateColumns.Add(rowVersionDescriptor, DbExpression.Parameter(rowVersionNewValue, rowVersionDescriptor.PropertyType, rowVersionDescriptor.Column.DbType));
                    keyValues.Add(rowVersionDescriptor, rowVersionValue);
                }
            }

            if (updateColumns.Count == 0)
                return 0;

            DbTable dbTable = PublicHelper.CreateDbTable(typeDescriptor, table);
            DbExpression conditionExp = PublicHelper.MakeCondition(keyValues, dbTable);
            DbUpdateExpression e = new DbUpdateExpression(dbTable, conditionExp);

            foreach (var item in updateColumns)
            {
                e.UpdateColumns.Add(item.Key.Column, item.Value);
            }

            int rowsAffected = 0;
            if (rowVersionDescriptor == null)
            {
                rowsAffected = await this.ExecuteNonQuery(e, @async);
                if (entityState != null)
                    entityState.Refresh();
                return rowsAffected;
            }

            if (rowVersionDescriptor.IsTimestamp())
            {
                List<Action<TEntity, IDataReader>> mappers = new List<Action<TEntity, IDataReader>>();
                mappers.Add(GetMapper<TEntity>(rowVersionDescriptor, e.Returns.Count));
                e.Returns.Add(rowVersionDescriptor.Column);

                IDataReader dataReader = await this.ExecuteReader(e, @async);
                using (dataReader)
                {
                    while (dataReader.Read())
                    {
                        rowsAffected++;
                        foreach (var mapper in mappers)
                        {
                            mapper(entity, dataReader);
                        }
                    }
                }

                PublicHelper.CauseErrorIfOptimisticUpdateFailed(rowsAffected);
            }
            else
            {
                rowsAffected = await this.ExecuteNonQuery(e, @async);
                PublicHelper.CauseErrorIfOptimisticUpdateFailed(rowsAffected);
                rowVersionDescriptor.SetValue(entity, rowVersionNewValue);
            }

            if (entityState != null)
                entityState.Refresh();

            return rowsAffected;
        }

        protected override async Task<int> Delete<TEntity>(TEntity entity, string table, bool @async)
        {
            PublicHelper.CheckNull(entity);

            TypeDescriptor typeDescriptor = EntityTypeContainer.GetDescriptor(typeof(TEntity));
            PublicHelper.EnsureHasPrimaryKey(typeDescriptor);

            PairList<PrimitivePropertyDescriptor, object> keyValues = new PairList<PrimitivePropertyDescriptor, object>(typeDescriptor.PrimaryKeys.Count);

            foreach (PrimitivePropertyDescriptor keyPropertyDescriptor in typeDescriptor.PrimaryKeys)
            {
                object keyValue = keyPropertyDescriptor.GetValue(entity);
                PrimaryKeyHelper.KeyValueNotNull(keyPropertyDescriptor, keyValue);
                keyValues.Add(keyPropertyDescriptor, keyValue);
            }

            PrimitivePropertyDescriptor rowVersionDescriptor = null;
            if (typeDescriptor.HasRowVersion())
            {
                rowVersionDescriptor = typeDescriptor.RowVersion;
                var rowVersionValue = typeDescriptor.RowVersion.GetValue(entity);
                this.EnsureRowVersionValueIsNotNull(rowVersionValue);
                keyValues.Add(typeDescriptor.RowVersion, rowVersionValue);
            }

            DbTable dbTable = PublicHelper.CreateDbTable(typeDescriptor, table);
            DbExpression conditionExp = PublicHelper.MakeCondition(keyValues, dbTable);
            DbDeleteExpression e = new DbDeleteExpression(dbTable, conditionExp);

            int rowsAffected = await this.ExecuteNonQuery(e, @async);

            if (rowVersionDescriptor != null)
            {
                PublicHelper.CauseErrorIfOptimisticUpdateFailed(rowsAffected);
            }

            return rowsAffected;
        }

        DataTable ConvertToSqlBulkCopyDataTable<TModel>(List<TModel> modelList, TypeDescriptor typeDescriptor)
        {
            DataTable dt = new DataTable();

            var mappingPropertyDescriptors = typeDescriptor.PrimitivePropertyDescriptors.Where(a => !a.IsTimestamp()).ToList();

            for (int i = 0; i < mappingPropertyDescriptors.Count; i++)
            {
                PrimitivePropertyDescriptor mappingPropertyDescriptor = mappingPropertyDescriptors[i];

                Type dataType = mappingPropertyDescriptor.PropertyType.GetUnderlyingType();
                if (dataType.IsEnum)
                    dataType = Enum.GetUnderlyingType(dataType);

                dt.Columns.Add(new DataColumn(mappingPropertyDescriptor.Column.Name, dataType));
            }

            foreach (var model in modelList)
            {
                DataRow dr = dt.NewRow();
                for (int i = 0; i < mappingPropertyDescriptors.Count; i++)
                {
                    PrimitivePropertyDescriptor mappingPropertyDescriptor = mappingPropertyDescriptors[i];
                    object value = mappingPropertyDescriptor.GetValue(model);

                    PublicHelper.NotNullCheck(mappingPropertyDescriptor, value);

                    if (mappingPropertyDescriptor.PropertyType.GetUnderlyingType().IsEnum)
                    {
                        if (value != null)
                            value = Convert.ChangeType(value, Enum.GetUnderlyingType(value.GetType()));
                    }

                    dr[i] = value ?? DBNull.Value;
                }

                dt.Rows.Add(dr);
            }

            return dt;
        }
        void EnsureRowVersionValueIsNotNull(object rowVersionValue)
        {
            if (rowVersionValue == null)
            {
                throw new ArgumentException("The row version member of entity cannot be null.");
            }
        }
    }
}
