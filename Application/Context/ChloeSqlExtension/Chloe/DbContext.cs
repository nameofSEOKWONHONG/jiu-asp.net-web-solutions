using Chloe.Core;
using Chloe.Core.Visitors;
using Chloe.Data;
using Chloe.DbExpressions;
using Chloe.Descriptors;
using Chloe.Exceptions;
using Chloe.Infrastructure;
using Chloe.Query;
using Chloe.Query.Internals;
using Chloe.Reflection;
using Chloe.Threading.Tasks;
using Chloe.Utility;
using System.Collections;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Chloe
{
    public abstract partial class DbContext : IDbContext, IDisposable
    {
        bool _disposed = false;
        InnerAdoSession _adoSession;
        DbSession _session;

        Dictionary<Type, TrackEntityCollection> _trackingEntityContainer;
        Dictionary<Type, TrackEntityCollection> TrackingEntityContainer
        {
            get
            {
                if (this._trackingEntityContainer == null)
                {
                    this._trackingEntityContainer = new Dictionary<Type, TrackEntityCollection>();
                }

                return this._trackingEntityContainer;
            }
        }

        internal Dictionary<Type, List<LambdaExpression>> QueryFilters { get; set; } = new Dictionary<Type, List<LambdaExpression>>();
        internal InnerAdoSession AdoSession
        {
            get
            {
                this.CheckDisposed();
                if (this._adoSession == null)
                    this._adoSession = new InnerAdoSession(this.DatabaseProvider.CreateConnection());
                return this._adoSession;
            }
        }
        public abstract IDatabaseProvider DatabaseProvider { get; }

        protected DbContext()
        {
            this._session = new DbSession(this);
        }

        public IDbSession Session { get { return this._session; } }

        public void HasQueryFilter<TEntity>(Expression<Func<TEntity, bool>> filter)
        {
            PublicHelper.CheckNull(filter, nameof(filter));

            Type entityType = typeof(TEntity);
            List<LambdaExpression> filters;
            if (!this.QueryFilters.TryGetValue(entityType, out filters))
            {
                filters = new List<LambdaExpression>(1);
                this.QueryFilters.Add(entityType, filters);
            }

            filters.Add(filter);
        }

        public virtual IQuery<TEntity> Query<TEntity>()
        {
            return this.Query<TEntity>(null);
        }
        public virtual IQuery<TEntity> Query<TEntity>(string table)
        {
            return this.Query<TEntity>(table, LockType.Unspecified);
        }
        public virtual IQuery<TEntity> Query<TEntity>(LockType @lock)
        {
            return this.Query<TEntity>(null, @lock);
        }
        public virtual IQuery<TEntity> Query<TEntity>(string table, LockType @lock)
        {
            return new Query<TEntity>(this, table, @lock);
        }

        public virtual TEntity QueryByKey<TEntity>(object key)
        {
            return this.QueryByKey<TEntity>(key, false);
        }
        public virtual TEntity QueryByKey<TEntity>(object key, bool tracking)
        {
            return this.QueryByKey<TEntity>(key, null, tracking);
        }
        public virtual TEntity QueryByKey<TEntity>(object key, string table)
        {
            return this.QueryByKey<TEntity>(key, table, false);
        }
        public virtual TEntity QueryByKey<TEntity>(object key, string table, bool tracking)
        {
            return this.QueryByKey<TEntity>(key, table, LockType.Unspecified, tracking);
        }
        public virtual TEntity QueryByKey<TEntity>(object key, string table, LockType @lock, bool tracking)
        {
            return this.QueryByKey<TEntity>(key, table, @lock, tracking, false).GetResult();
        }

        public virtual Task<TEntity> QueryByKeyAsync<TEntity>(object key)
        {
            return this.QueryByKeyAsync<TEntity>(key, false);
        }
        public virtual Task<TEntity> QueryByKeyAsync<TEntity>(object key, bool tracking)
        {
            return this.QueryByKeyAsync<TEntity>(key, null, tracking);
        }
        public virtual Task<TEntity> QueryByKeyAsync<TEntity>(object key, string table)
        {
            return this.QueryByKeyAsync<TEntity>(key, table, false);
        }
        public virtual Task<TEntity> QueryByKeyAsync<TEntity>(object key, string table, bool tracking)
        {
            return this.QueryByKeyAsync<TEntity>(key, table, LockType.Unspecified, tracking);
        }
        public virtual Task<TEntity> QueryByKeyAsync<TEntity>(object key, string table, LockType @lock, bool tracking)
        {
            return this.QueryByKey<TEntity>(key, table, @lock, tracking, true);
        }
        protected virtual async Task<TEntity> QueryByKey<TEntity>(object key, string table, LockType @lock, bool tracking, bool @async)
        {
            Expression<Func<TEntity, bool>> condition = PrimaryKeyHelper.BuildCondition<TEntity>(key);
            var q = this.Query<TEntity>(table, @lock).Where(condition);

            if (tracking)
                q = q.AsTracking();

            if (@async)
                return await q.FirstOrDefaultAsync();

            return q.FirstOrDefault();
        }

        public virtual IJoinQuery<T1, T2> JoinQuery<T1, T2>(Expression<Func<T1, T2, object[]>> joinInfo)
        {
            KeyValuePairList<JoinType, Expression> joinInfos = ResolveJoinInfo(joinInfo);
            var ret = this.Query<T1>()
                .Join<T2>(joinInfos[0].Key, (Expression<Func<T1, T2, bool>>)joinInfos[0].Value);

            return ret;
        }
        public virtual IJoinQuery<T1, T2, T3> JoinQuery<T1, T2, T3>(Expression<Func<T1, T2, T3, object[]>> joinInfo)
        {
            KeyValuePairList<JoinType, Expression> joinInfos = ResolveJoinInfo(joinInfo);
            var ret = this.Query<T1>()
                .Join<T2>(joinInfos[0].Key, (Expression<Func<T1, T2, bool>>)joinInfos[0].Value)
                .Join<T3>(joinInfos[1].Key, (Expression<Func<T1, T2, T3, bool>>)joinInfos[1].Value);

            return ret;
        }
        public virtual IJoinQuery<T1, T2, T3, T4> JoinQuery<T1, T2, T3, T4>(Expression<Func<T1, T2, T3, T4, object[]>> joinInfo)
        {
            KeyValuePairList<JoinType, Expression> joinInfos = ResolveJoinInfo(joinInfo);
            var ret = this.Query<T1>()
                .Join<T2>(joinInfos[0].Key, (Expression<Func<T1, T2, bool>>)joinInfos[0].Value)
                .Join<T3>(joinInfos[1].Key, (Expression<Func<T1, T2, T3, bool>>)joinInfos[1].Value)
                .Join<T4>(joinInfos[2].Key, (Expression<Func<T1, T2, T3, T4, bool>>)joinInfos[2].Value);

            return ret;
        }
        public virtual IJoinQuery<T1, T2, T3, T4, T5> JoinQuery<T1, T2, T3, T4, T5>(Expression<Func<T1, T2, T3, T4, T5, object[]>> joinInfo)
        {
            KeyValuePairList<JoinType, Expression> joinInfos = ResolveJoinInfo(joinInfo);
            var ret = this.Query<T1>()
                .Join<T2>(joinInfos[0].Key, (Expression<Func<T1, T2, bool>>)joinInfos[0].Value)
                .Join<T3>(joinInfos[1].Key, (Expression<Func<T1, T2, T3, bool>>)joinInfos[1].Value)
                .Join<T4>(joinInfos[2].Key, (Expression<Func<T1, T2, T3, T4, bool>>)joinInfos[2].Value)
                .Join<T5>(joinInfos[3].Key, (Expression<Func<T1, T2, T3, T4, T5, bool>>)joinInfos[3].Value);

            return ret;
        }

        public virtual List<T> SqlQuery<T>(string sql, params DbParam[] parameters)
        {
            return this.SqlQuery<T>(sql, CommandType.Text, parameters);
        }
        public virtual List<T> SqlQuery<T>(string sql, CommandType cmdType, params DbParam[] parameters)
        {
            PublicHelper.CheckNull(sql, "sql");
            return new InternalSqlQuery<T>(this, sql, cmdType, parameters).AsIEnumerable().ToList();
        }
        public virtual Task<List<T>> SqlQueryAsync<T>(string sql, params DbParam[] parameters)
        {
            return this.SqlQueryAsync<T>(sql, CommandType.Text, parameters);
        }
        public virtual Task<List<T>> SqlQueryAsync<T>(string sql, CommandType cmdType, params DbParam[] parameters)
        {
            PublicHelper.CheckNull(sql, "sql");
            return new InternalSqlQuery<T>(this, sql, cmdType, parameters).AsIAsyncEnumerable().ToListAsync().AsTask();
        }

        public List<T> SqlQuery<T>(string sql, object parameter)
        {
            /*
             * Usage:
             * dbContext.SqlQuery<User>("select * from Users where Id=@Id", new { Id = 1 });
             */

            return this.SqlQuery<T>(sql, PublicHelper.BuildParams(this, parameter));
        }
        public List<T> SqlQuery<T>(string sql, CommandType cmdType, object parameter)
        {
            /*
             * Usage:
             * dbContext.SqlQuery<User>("select * from Users where Id=@Id", CommandType.Text, new { Id = 1 });
             */

            return this.SqlQuery<T>(sql, cmdType, PublicHelper.BuildParams(this, parameter));
        }
        public Task<List<T>> SqlQueryAsync<T>(string sql, object parameter)
        {
            return this.SqlQueryAsync<T>(sql, PublicHelper.BuildParams(this, parameter));
        }
        public Task<List<T>> SqlQueryAsync<T>(string sql, CommandType cmdType, object parameter)
        {
            return this.SqlQueryAsync<T>(sql, cmdType, PublicHelper.BuildParams(this, parameter));
        }

        public TEntity Save<TEntity>(TEntity entity)
        {
            this.Save(entity, false).GetResult();
            return entity;
        }
        public async Task<TEntity> SaveAsync<TEntity>(TEntity entity)
        {
            await this.Save(entity, true);
            return entity;
        }
        async Task Save<TEntity>(TEntity entity, bool @async)
        {
            if (this.Session.IsInTransaction)
            {
                await this.Save(entity, null, @async);
                return;
            }

            using (var tran = this.BeginTransaction())
            {
                await this.Save(entity, null, @async);
                tran.Commit();
            }
        }
        async Task Save<TEntity>(TEntity entity, TypeDescriptor declaringTypeDescriptor, bool @async)
        {
            await this.Insert(entity, null, @async);

            TypeDescriptor typeDescriptor = EntityTypeContainer.GetDescriptor(typeof(TEntity));

            for (int i = 0; i < typeDescriptor.ComplexPropertyDescriptors.Count; i++)
            {
                //entity.TOther
                ComplexPropertyDescriptor navPropertyDescriptor = typeDescriptor.ComplexPropertyDescriptors[i];

                if (declaringTypeDescriptor != null && navPropertyDescriptor.PropertyType == declaringTypeDescriptor.Definition.Type)
                {
                    continue;
                }

                await this.SaveOneToOne(navPropertyDescriptor, entity, typeDescriptor, @async);
            }

            for (int i = 0; i < typeDescriptor.CollectionPropertyDescriptors.Count; i++)
            {
                //entity.List
                CollectionPropertyDescriptor collectionPropertyDescriptor = typeDescriptor.CollectionPropertyDescriptors[i];
                await this.SaveCollection(collectionPropertyDescriptor, entity, typeDescriptor, @async);
            }
        }
        async Task SaveOneToOne(ComplexPropertyDescriptor navPropertyDescriptor, object owner, TypeDescriptor ownerTypeDescriptor, bool @async)
        {
            /*
             * 1:1
             * T    <1:1>    TOther
             * T.TOther <--> TOther.T
             * T.Id <--> TOther.Id
             */

            //owner is T
            //navPropertyDescriptor is T.TOther
            //TypeDescriptor of T.TOther
            TypeDescriptor navTypeDescriptor = EntityTypeContainer.GetDescriptor(navPropertyDescriptor.PropertyType);
            //TOther.T
            ComplexPropertyDescriptor TOtherDotT = navTypeDescriptor.ComplexPropertyDescriptors.Where(a => a.PropertyType == ownerTypeDescriptor.Definition.Type).FirstOrDefault();

            bool isOneToOne = TOtherDotT != null;
            if (!isOneToOne)
                return;

            //instance of T.TOther
            object navValue = navPropertyDescriptor.GetValue(owner);
            if (navValue == null)
                return;

            //T.Id
            PrimitivePropertyDescriptor foreignKeyProperty = navPropertyDescriptor.ForeignKeyProperty;
            if (foreignKeyProperty.IsAutoIncrement || foreignKeyProperty.HasSequence())
            {
                //value of T.Id
                object foreignKeyValue = foreignKeyProperty.GetValue(owner);

                //T.TOther.Id = T.Id
                TOtherDotT.ForeignKeyProperty.SetValue(navValue, foreignKeyValue);
            }

            MethodInfo saveMethod = GetSaveMethod(navPropertyDescriptor.PropertyType);
            //DbContext.Save(navValue, ownerTypeDescriptor, @async);
            Task task = (Task)saveMethod.FastInvoke(this, navValue, ownerTypeDescriptor, @async);
            await task;
        }
        async Task SaveCollection(CollectionPropertyDescriptor collectionPropertyDescriptor, object owner, TypeDescriptor ownerTypeDescriptor, bool @async)
        {
            PrimitivePropertyDescriptor ownerKeyPropertyDescriptor = ownerTypeDescriptor.PrimaryKeys.FirstOrDefault();
            if (ownerKeyPropertyDescriptor == null)
                return;

            //T.Elements
            IList elementList = collectionPropertyDescriptor.GetValue(owner) as IList;
            if (elementList == null || elementList.Count == 0)
                return;

            TypeDescriptor elementTypeDescriptor = EntityTypeContainer.GetDescriptor(collectionPropertyDescriptor.ElementType);
            //Element.T
            ComplexPropertyDescriptor elementDotT = elementTypeDescriptor.ComplexPropertyDescriptors.Where(a => a.PropertyType == ownerTypeDescriptor.Definition.Type).FirstOrDefault();

            object ownerKeyValue = ownerKeyPropertyDescriptor.GetValue(owner);
            MethodInfo saveMethod = GetSaveMethod(collectionPropertyDescriptor.ElementType);
            for (int i = 0; i < elementList.Count; i++)
            {
                object element = elementList[i];
                if (element == null)
                    continue;

                //element.ForeignKey = T.Id
                elementDotT.ForeignKeyProperty.SetValue(element, ownerKeyValue);
                //DbContext.Save(element, ownerTypeDescriptor, @async);
                Task task = (Task)saveMethod.FastInvoke(this, element, ownerTypeDescriptor, @async);
                await task;
            }
        }

        public virtual TEntity Insert<TEntity>(TEntity entity)
        {
            return this.Insert(entity, null);
        }
        public virtual TEntity Insert<TEntity>(TEntity entity, string table)
        {
            return this.Insert(entity, table, false).GetResult();
        }
        public virtual Task<TEntity> InsertAsync<TEntity>(TEntity entity)
        {
            return this.InsertAsync(entity, null);
        }
        public virtual Task<TEntity> InsertAsync<TEntity>(TEntity entity, string table)
        {
            return this.Insert(entity, table, true);
        }
        protected virtual async Task<TEntity> Insert<TEntity>(TEntity entity, string table, bool @async)
        {
            PublicHelper.CheckNull(entity);

            TypeDescriptor typeDescriptor = EntityTypeContainer.GetDescriptor(typeof(TEntity));

            Dictionary<PrimitivePropertyDescriptor, object> keyValueMap = PrimaryKeyHelper.CreateKeyValueMap(typeDescriptor);

            Dictionary<PrimitivePropertyDescriptor, DbExpression> insertColumns = new Dictionary<PrimitivePropertyDescriptor, DbExpression>();
            foreach (PrimitivePropertyDescriptor propertyDescriptor in typeDescriptor.PrimitivePropertyDescriptors)
            {
                if (propertyDescriptor.IsAutoIncrement)
                    continue;

                object val = propertyDescriptor.GetValue(entity);

                if (propertyDescriptor.IsPrimaryKey)
                {
                    keyValueMap[propertyDescriptor] = val;
                }

                PublicHelper.NotNullCheck(propertyDescriptor, val);

                DbParameterExpression valExp = DbExpression.Parameter(val, propertyDescriptor.PropertyType, propertyDescriptor.Column.DbType);
                insertColumns.Add(propertyDescriptor, valExp);
            }

            PrimitivePropertyDescriptor nullValueKey = keyValueMap.Where(a => a.Value == null && !a.Key.IsAutoIncrement).Select(a => a.Key).FirstOrDefault();
            if (nullValueKey != null)
            {
                /* 主键为空并且主键又不是自增列 */
                throw new ChloeException(string.Format("The primary key '{0}' could not be null.", nullValueKey.Property.Name));
            }

            DbTable dbTable = PublicHelper.CreateDbTable(typeDescriptor, table);
            DbInsertExpression e = new DbInsertExpression(dbTable);

            foreach (var kv in insertColumns)
            {
                e.InsertColumns.Add(kv.Key.Column, kv.Value);
            }

            PrimitivePropertyDescriptor autoIncrementPropertyDescriptor = typeDescriptor.AutoIncrement;
            if (autoIncrementPropertyDescriptor == null)
            {
                await this.ExecuteNonQuery(e, @async);
                return entity;
            }

            IDbExpressionTranslator translator = this.DatabaseProvider.CreateDbExpressionTranslator();
            DbCommandInfo dbCommandInfo = translator.Translate(e);

            dbCommandInfo.CommandText = string.Concat(dbCommandInfo.CommandText, ";", this.GetSelectLastInsertIdClause());

            //SELECT @@IDENTITY 返回的是 decimal 类型
            object retIdentity = await this.ExecuteScalar(dbCommandInfo, @async);

            if (retIdentity == null || retIdentity == DBNull.Value)
            {
                throw new ChloeException("Unable to get the identity value.");
            }

            retIdentity = PublicHelper.ConvertObjectType(retIdentity, autoIncrementPropertyDescriptor.PropertyType);
            autoIncrementPropertyDescriptor.SetValue(entity, retIdentity);
            return entity;
        }
        public virtual object Insert<TEntity>(Expression<Func<TEntity>> content)
        {
            return this.Insert(content, null);
        }
        public virtual object Insert<TEntity>(Expression<Func<TEntity>> content, string table)
        {
            return this.Insert(content, table, false).GetResult();
        }
        public virtual Task<object> InsertAsync<TEntity>(Expression<Func<TEntity>> content)
        {
            return this.InsertAsync(content, null);
        }
        public virtual Task<object> InsertAsync<TEntity>(Expression<Func<TEntity>> content, string table)
        {
            return this.Insert(content, table, true);
        }
        protected virtual async Task<object> Insert<TEntity>(Expression<Func<TEntity>> content, string table, bool @async)
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
            DbInsertExpression e = new DbInsertExpression(dbTable);

            object keyVal = null;

            foreach (var kv in insertColumns)
            {
                MemberInfo key = kv.Key;
                PrimitivePropertyDescriptor propertyDescriptor = typeDescriptor.GetPrimitivePropertyDescriptor(key);

                if (propertyDescriptor.IsAutoIncrement)
                    throw new ChloeException(string.Format("Could not insert value into the identity column '{0}'.", propertyDescriptor.Column.Name));

                if (propertyDescriptor.IsPrimaryKey)
                {
                    object val = ExpressionEvaluator.Evaluate(kv.Value);
                    if (val == null)
                        throw new ChloeException(string.Format("The primary key '{0}' could not be null.", propertyDescriptor.Property.Name));
                    else
                    {
                        keyVal = val;
                        e.InsertColumns.Add(propertyDescriptor.Column, DbExpression.Parameter(keyVal, propertyDescriptor.PropertyType, propertyDescriptor.Column.DbType));
                        continue;
                    }
                }

                e.InsertColumns.Add(propertyDescriptor.Column, expressionParser.Parse(kv.Value));
            }

            if (keyPropertyDescriptor != null)
            {
                //主键为空并且主键又不是自增列
                if (keyVal == null && !keyPropertyDescriptor.IsAutoIncrement)
                {
                    throw new ChloeException(string.Format("The primary key '{0}' could not be null.", keyPropertyDescriptor.Property.Name));
                }
            }

            if (keyPropertyDescriptor == null || !keyPropertyDescriptor.IsAutoIncrement)
            {
                await this.ExecuteNonQuery(e, @async);
                return keyVal; /* It will return null if an entity does not define primary key. */
            }

            IDbExpressionTranslator translator = this.DatabaseProvider.CreateDbExpressionTranslator();
            DbCommandInfo dbCommandInfo = translator.Translate(e);
            dbCommandInfo.CommandText = string.Concat(dbCommandInfo.CommandText, ";", this.GetSelectLastInsertIdClause());

            //SELECT @@IDENTITY 返回的是 decimal 类型
            object retIdentity = await this.ExecuteScalar(dbCommandInfo, @async);
            if (retIdentity == null || retIdentity == DBNull.Value)
            {
                throw new ChloeException("Unable to get the identity value.");
            }

            retIdentity = PublicHelper.ConvertObjectType(retIdentity, typeDescriptor.AutoIncrement.PropertyType);
            return retIdentity;
        }

        public virtual void InsertRange<TEntity>(List<TEntity> entities)
        {
            this.InsertRange(entities, null);
        }
        public virtual void InsertRange<TEntity>(List<TEntity> entities, string table)
        {
            this.InsertRange(entities, table, false).GetResult();
        }
        public virtual Task InsertRangeAsync<TEntity>(List<TEntity> entities)
        {
            return this.InsertRangeAsync(entities, null);
        }
        public virtual Task InsertRangeAsync<TEntity>(List<TEntity> entities, string table)
        {
            return this.InsertRange(entities, table, true);
        }
        protected virtual Task InsertRange<TEntity>(List<TEntity> entities, string table, bool @async)
        {
            throw new NotImplementedException();
        }

        public virtual int Update<TEntity>(TEntity entity)
        {
            return this.Update(entity, null);
        }
        public virtual int Update<TEntity>(TEntity entity, string table)
        {
            return this.Update<TEntity>(entity, table, false).GetResult();
        }
        public virtual Task<int> UpdateAsync<TEntity>(TEntity entity)
        {
            return this.UpdateAsync(entity, null);
        }
        public virtual Task<int> UpdateAsync<TEntity>(TEntity entity, string table)
        {
            return this.Update<TEntity>(entity, table, true);
        }
        protected virtual async Task<int> Update<TEntity>(TEntity entity, string table, bool @async)
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

            object rowVersionNewValue = null;
            if (typeDescriptor.HasRowVersion())
            {
                var rowVersionDescriptor = typeDescriptor.RowVersion;
                var rowVersionOldValue = rowVersionDescriptor.GetValue(entity);
                rowVersionNewValue = PublicHelper.IncreaseRowVersionNumber(rowVersionOldValue);
                updateColumns.Add(rowVersionDescriptor, DbExpression.Parameter(rowVersionNewValue, rowVersionDescriptor.PropertyType, rowVersionDescriptor.Column.DbType));
                keyValues.Add(rowVersionDescriptor, rowVersionOldValue);
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

            int rowsAffected = await this.ExecuteNonQuery(e, @async);

            if (typeDescriptor.HasRowVersion())
            {
                PublicHelper.CauseErrorIfOptimisticUpdateFailed(rowsAffected);
                typeDescriptor.RowVersion.SetValue(entity, rowVersionNewValue);
            }

            if (entityState != null)
                entityState.Refresh();

            return rowsAffected;
        }

        public virtual int Update<TEntity>(Expression<Func<TEntity, bool>> condition, Expression<Func<TEntity, TEntity>> content)
        {
            return this.Update(condition, content, null);
        }
        public virtual int Update<TEntity>(Expression<Func<TEntity, bool>> condition, Expression<Func<TEntity, TEntity>> content, string table)
        {
            return this.Update<TEntity>(condition, content, table, false).GetResult();
        }
        public virtual Task<int> UpdateAsync<TEntity>(Expression<Func<TEntity, bool>> condition, Expression<Func<TEntity, TEntity>> content)
        {
            return this.UpdateAsync(condition, content, null);
        }
        public virtual Task<int> UpdateAsync<TEntity>(Expression<Func<TEntity, bool>> condition, Expression<Func<TEntity, TEntity>> content, string table)
        {
            return this.Update<TEntity>(condition, content, table, true);
        }
        protected virtual async Task<int> Update<TEntity>(Expression<Func<TEntity, bool>> condition, Expression<Func<TEntity, TEntity>> content, string table, bool @async)
        {
            PublicHelper.CheckNull(condition);
            PublicHelper.CheckNull(content);

            TypeDescriptor typeDescriptor = EntityTypeContainer.GetDescriptor(typeof(TEntity));

            Dictionary<MemberInfo, Expression> updateColumns = InitMemberExtractor.Extract(content);

            DbTable dbTable = PublicHelper.CreateDbTable(typeDescriptor, table);
            DefaultExpressionParser expressionParser = typeDescriptor.GetExpressionParser(dbTable);

            DbExpression conditionExp = expressionParser.ParseFilterPredicate(condition);

            DbUpdateExpression e = new DbUpdateExpression(dbTable, conditionExp);

            foreach (var kv in updateColumns)
            {
                MemberInfo key = kv.Key;
                PrimitivePropertyDescriptor propertyDescriptor = typeDescriptor.GetPrimitivePropertyDescriptor(key);

                if (propertyDescriptor.IsPrimaryKey)
                    throw new ChloeException(string.Format("Could not update the primary key '{0}'.", propertyDescriptor.Column.Name));

                if (propertyDescriptor.IsAutoIncrement || propertyDescriptor.HasSequence())
                    throw new ChloeException(string.Format("Could not update the column '{0}', because it's mapping member is auto increment or has define a sequence.", propertyDescriptor.Column.Name));

                e.UpdateColumns.Add(propertyDescriptor.Column, expressionParser.Parse(kv.Value));
            }

            if (e.UpdateColumns.Count == 0)
                return 0;

            return await this.ExecuteNonQuery(e, @async);
        }

        public virtual int Delete<TEntity>(TEntity entity)
        {
            return this.Delete(entity, null);
        }
        public virtual int Delete<TEntity>(TEntity entity, string table)
        {
            return this.Delete<TEntity>(entity, table, false).GetResult();
        }
        public virtual Task<int> DeleteAsync<TEntity>(TEntity entity)
        {
            return this.DeleteAsync(entity, null);
        }
        public virtual Task<int> DeleteAsync<TEntity>(TEntity entity, string table)
        {
            return this.Delete<TEntity>(entity, table, true);
        }
        protected virtual async Task<int> Delete<TEntity>(TEntity entity, string table, bool @async)
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

            if (typeDescriptor.HasRowVersion())
            {
                var rowVersionValue = typeDescriptor.RowVersion.GetValue(entity);
                keyValues.Add(typeDescriptor.RowVersion, rowVersionValue);
            }

            DbTable dbTable = PublicHelper.CreateDbTable(typeDescriptor, table);
            DbExpression conditionExp = PublicHelper.MakeCondition(keyValues, dbTable);
            DbDeleteExpression e = new DbDeleteExpression(dbTable, conditionExp);

            int rowsAffected = await this.ExecuteNonQuery(e, @async);

            if (typeDescriptor.HasRowVersion())
            {
                PublicHelper.CauseErrorIfOptimisticUpdateFailed(rowsAffected);
            }

            return rowsAffected;
        }

        public virtual int Delete<TEntity>(Expression<Func<TEntity, bool>> condition)
        {
            return this.Delete(condition, null);
        }
        public virtual int Delete<TEntity>(Expression<Func<TEntity, bool>> condition, string table)
        {
            return this.Delete<TEntity>(condition, table, false).GetResult();
        }
        public virtual Task<int> DeleteAsync<TEntity>(Expression<Func<TEntity, bool>> condition)
        {
            return this.DeleteAsync(condition, null);
        }
        public virtual Task<int> DeleteAsync<TEntity>(Expression<Func<TEntity, bool>> condition, string table)
        {
            return this.Delete<TEntity>(condition, table, true);
        }
        protected virtual Task<int> Delete<TEntity>(Expression<Func<TEntity, bool>> condition, string table, bool @async)
        {
            PublicHelper.CheckNull(condition);

            TypeDescriptor typeDescriptor = EntityTypeContainer.GetDescriptor(typeof(TEntity));

            DbTable dbTable = typeDescriptor.GenDbTable(table);
            DefaultExpressionParser expressionParser = typeDescriptor.GetExpressionParser(dbTable);
            DbExpression conditionExp = expressionParser.ParseFilterPredicate(condition);

            DbDeleteExpression e = new DbDeleteExpression(dbTable, conditionExp);

            return this.ExecuteNonQuery(e, @async);
        }

        public virtual int DeleteByKey<TEntity>(object key)
        {
            return this.DeleteByKey<TEntity>(key, null);
        }
        public virtual int DeleteByKey<TEntity>(object key, string table)
        {
            return this.DeleteByKey<TEntity>(key, table, false).GetResult();
        }
        public virtual Task<int> DeleteByKeyAsync<TEntity>(object key)
        {
            return this.DeleteByKeyAsync<TEntity>(key, null);
        }
        public virtual Task<int> DeleteByKeyAsync<TEntity>(object key, string table)
        {
            return this.DeleteByKey<TEntity>(key, table, true);
        }
        protected virtual Task<int> DeleteByKey<TEntity>(object key, string table, bool @async)
        {
            Expression<Func<TEntity, bool>> condition = PrimaryKeyHelper.BuildCondition<TEntity>(key);
            return this.Delete<TEntity>(condition, table, @async);
        }


        public ITransientTransaction BeginTransaction()
        {
            /*
             * using(ITransientTransaction tran = dbContext.BeginTransaction())
             * {
             *      dbContext.Insert()...
             *      dbContext.Update()...
             *      dbContext.Delete()...
             *      tran.Commit();
             * }
             */
            return new TransientTransaction(this);
        }
        public ITransientTransaction BeginTransaction(IsolationLevel il)
        {
            return new TransientTransaction(this, il);
        }
        public void UseTransaction(Action action)
        {
            /*
             * dbContext.UseTransaction(() =>
             * {
             *     dbContext.Insert()...
             *     dbContext.Update()...
             *     dbContext.Delete()...
             * });
             */

            PublicHelper.CheckNull(action);
            using (ITransientTransaction tran = this.BeginTransaction())
            {
                action();
                tran.Commit();
            }
        }
        public void UseTransaction(Action action, IsolationLevel il)
        {
            PublicHelper.CheckNull(action);
            using (ITransientTransaction tran = this.BeginTransaction(il))
            {
                action();
                tran.Commit();
            }
        }
        public async Task UseTransaction(Func<Task> func)
        {
            /*
             * await dbContext.UseTransaction(async () =>
             * {
             *     await dbContext.InsertAsync()...
             *     await dbContext.UpdateAsync()...
             *     await dbContext.DeleteAsync()...
             * });
             */

            PublicHelper.CheckNull(func);
            using (ITransientTransaction tran = this.BeginTransaction())
            {
                await func();
                tran.Commit();
            }
        }
        public async Task UseTransaction(Func<Task> func, IsolationLevel il)
        {
            PublicHelper.CheckNull(func);
            using (ITransientTransaction tran = this.BeginTransaction(il))
            {
                await func();
                tran.Commit();
            }
        }

        public virtual void TrackEntity(object entity)
        {
            PublicHelper.CheckNull(entity);
            Type entityType = entity.GetType();

            if (ReflectionExtension.IsAnonymousType(entityType))
                return;

            Dictionary<Type, TrackEntityCollection> entityContainer = this.TrackingEntityContainer;

            TrackEntityCollection collection;
            if (!entityContainer.TryGetValue(entityType, out collection))
            {
                TypeDescriptor typeDescriptor = EntityTypeContainer.GetDescriptor(entityType);

                if (!typeDescriptor.HasPrimaryKey())
                    return;

                collection = new TrackEntityCollection(typeDescriptor);
                entityContainer.Add(entityType, collection);
            }

            collection.TryAddEntity(entity);
        }
        protected virtual string GetSelectLastInsertIdClause()
        {
            return "SELECT @@IDENTITY";
        }
        protected virtual IEntityState TryGetTrackedEntityState(object entity)
        {
            PublicHelper.CheckNull(entity);
            Type entityType = entity.GetType();
            Dictionary<Type, TrackEntityCollection> entityContainer = this._trackingEntityContainer;

            if (entityContainer == null)
                return null;

            TrackEntityCollection collection;
            if (!entityContainer.TryGetValue(entityType, out collection))
            {
                return null;
            }

            IEntityState ret = collection.TryGetEntityState(entity);
            return ret;
        }


        public void Dispose()
        {
            if (this._disposed)
                return;

            if (this._adoSession != null)
                this._adoSession.Dispose();
            this.Dispose(true);
            this._disposed = true;
        }
        protected virtual void Dispose(bool disposing)
        {

        }
        void CheckDisposed()
        {
            if (this._disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        class TrackEntityCollection
        {
            public TrackEntityCollection(TypeDescriptor typeDescriptor)
            {
                this.TypeDescriptor = typeDescriptor;
                this.Entities = new Dictionary<object, IEntityState>(1);
            }
            public TypeDescriptor TypeDescriptor { get; private set; }
            public Dictionary<object, IEntityState> Entities { get; private set; }
            public bool TryAddEntity(object entity)
            {
                if (this.Entities.ContainsKey(entity))
                {
                    return false;
                }

                IEntityState entityState = new EntityState(this.TypeDescriptor, entity);
                this.Entities.Add(entity, entityState);

                return true;
            }
            public IEntityState TryGetEntityState(object entity)
            {
                IEntityState ret;
                if (!this.Entities.TryGetValue(entity, out ret))
                    ret = null;

                return ret;
            }
        }
    }
}
