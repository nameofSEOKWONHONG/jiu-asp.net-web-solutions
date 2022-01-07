using Chloe.Core.Visitors;
using Chloe.DbExpressions;
using Chloe.Descriptors;
using Chloe.Exceptions;
using Chloe.Infrastructure;
using Chloe.RDBMS;
using Chloe.Threading.Tasks;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Chloe.MySql
{
    public class MySqlContext : DbContext
    {
        DatabaseProvider _databaseProvider;
        public MySqlContext(IDbConnectionFactory dbConnectionFactory)
        {
            PublicHelper.CheckNull(dbConnectionFactory);
            this._databaseProvider = new DatabaseProvider(dbConnectionFactory);
        }
        public MySqlContext(Func<IDbConnection> dbConnectionFactory) : this(new DbConnectionFactory(dbConnectionFactory))
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

        public override IDatabaseProvider DatabaseProvider
        {
            get { return this._databaseProvider; }
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

            int maxParameters = 1000;
            int batchSize = 60; /* 每批实体大小，此值通过测试得出相对插入速度比较快的一个值 */

            TypeDescriptor typeDescriptor = EntityTypeContainer.GetDescriptor(typeof(TEntity));

            List<PrimitivePropertyDescriptor> mappingPropertyDescriptors = typeDescriptor.PrimitivePropertyDescriptors.Where(a => a.IsAutoIncrement == false).ToList();
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
                        dbParams.Add(dbParam);
                        sqlBuilder.Append(paramName);
                    }
                    sqlBuilder.Append(")");

                    batchCount++;

                    if ((batchCount >= 20 && dbParams.Count >= 400/*参数个数太多也会影响速度*/) || dbParams.Count >= maxDbParamsCount || batchCount >= batchSize || (i + 1) == entities.Count)
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

        static string AppendInsertRangeSqlTemplate(DbTable table, List<PrimitivePropertyDescriptor> mappingPropertyDescriptors)
        {
            StringBuilder sqlBuilder = new StringBuilder();

            sqlBuilder.Append("INSERT INTO ");
            sqlBuilder.Append(AppendTableName(table));
            sqlBuilder.Append("(");

            for (int i = 0; i < mappingPropertyDescriptors.Count; i++)
            {
                PrimitivePropertyDescriptor mappingPropertyDescriptor = mappingPropertyDescriptors[i];
                if (i > 0)
                    sqlBuilder.Append(",");
                sqlBuilder.Append(Utils.QuoteName(mappingPropertyDescriptor.Column.Name));
            }

            sqlBuilder.Append(") VALUES");

            string sqlTemplate = sqlBuilder.ToString();
            return sqlTemplate;
        }

        static string AppendTableName(DbTable table)
        {
            return Utils.QuoteName(table.Name);
        }

        public virtual int Update<TEntity>(Expression<Func<TEntity, bool>> condition, Expression<Func<TEntity, TEntity>> content, int limits)
        {
            return this.Update<TEntity>(condition, content, null, limits);
        }
        public virtual int Update<TEntity>(Expression<Func<TEntity, bool>> condition, Expression<Func<TEntity, TEntity>> content, string table, int limits)
        {
            return this.Update<TEntity>(condition, content, table, limits, false).GetResult();
        }
        public virtual async Task<int> UpdateAsync<TEntity>(Expression<Func<TEntity, bool>> condition, Expression<Func<TEntity, TEntity>> content, int limits)
        {
            return await this.UpdateAsync<TEntity>(condition, content, null, limits);
        }
        public virtual async Task<int> UpdateAsync<TEntity>(Expression<Func<TEntity, bool>> condition, Expression<Func<TEntity, TEntity>> content, string table, int limits)
        {
            return await this.Update<TEntity>(condition, content, table, limits, true);
        }
        protected virtual async Task<int> Update<TEntity>(Expression<Func<TEntity, bool>> condition, Expression<Func<TEntity, TEntity>> content, string table, int limits, bool @async)
        {
            PublicHelper.CheckNull(condition);
            PublicHelper.CheckNull(content);

            TypeDescriptor typeDescriptor = EntityTypeContainer.GetDescriptor(typeof(TEntity));

            Dictionary<MemberInfo, Expression> updateColumns = InitMemberExtractor.Extract(content);

            DbTable dbTable = PublicHelper.CreateDbTable(typeDescriptor, table);
            DefaultExpressionParser expressionParser = typeDescriptor.GetExpressionParser(dbTable);

            DbExpression conditionExp = expressionParser.ParseFilterPredicate(condition);

            MySqlDbUpdateExpression e = new MySqlDbUpdateExpression(dbTable, conditionExp);

            foreach (var kv in updateColumns)
            {
                MemberInfo key = kv.Key;
                PrimitivePropertyDescriptor propertyDescriptor = typeDescriptor.GetPrimitivePropertyDescriptor(key);

                if (propertyDescriptor.IsPrimaryKey)
                    throw new ChloeException(string.Format("Could not update the primary key '{0}'.", propertyDescriptor.Column.Name));

                if (propertyDescriptor.IsAutoIncrement)
                    throw new ChloeException(string.Format("Could not update the identity column '{0}'.", propertyDescriptor.Column.Name));

                e.UpdateColumns.Add(propertyDescriptor.Column, expressionParser.Parse(kv.Value));
            }

            e.Limits = limits;

            if (e.UpdateColumns.Count == 0)
                return 0;

            return await this.ExecuteNonQuery(e, @async);
        }

        public virtual int Delete<TEntity>(Expression<Func<TEntity, bool>> condition, int limits)
        {
            return this.Delete(condition, null, limits);
        }
        public virtual int Delete<TEntity>(Expression<Func<TEntity, bool>> condition, string table, int limits)
        {
            return this.Delete(condition, table, limits, false).GetResult();
        }
        public virtual async Task<int> DeleteAsync<TEntity>(Expression<Func<TEntity, bool>> condition, int limits)
        {
            return await this.DeleteAsync(condition, null, limits);
        }
        public virtual async Task<int> DeleteAsync<TEntity>(Expression<Func<TEntity, bool>> condition, string table, int limits)
        {
            return await this.Delete(condition, table, limits, true);
        }
        protected virtual async Task<int> Delete<TEntity>(Expression<Func<TEntity, bool>> condition, string table, int limits, bool @async)
        {
            PublicHelper.CheckNull(condition);

            TypeDescriptor typeDescriptor = EntityTypeContainer.GetDescriptor(typeof(TEntity));

            DbTable dbTable = PublicHelper.CreateDbTable(typeDescriptor, table);
            DefaultExpressionParser expressionParser = typeDescriptor.GetExpressionParser(dbTable);
            DbExpression conditionExp = expressionParser.ParseFilterPredicate(condition);

            MySqlDbDeleteExpression e = new MySqlDbDeleteExpression(dbTable, conditionExp);
            e.Limits = limits;

            return await this.ExecuteNonQuery(e, @async);
        }
    }
}
