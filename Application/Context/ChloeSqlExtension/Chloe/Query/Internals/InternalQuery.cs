using Chloe.Core;
using Chloe.Data;
using Chloe.Infrastructure;
using Chloe.Mapper;
using Chloe.Mapper.Activators;
using Chloe.Query.Mapping;
using Chloe.Query.QueryState;
using Chloe.Query.Visitors;
using Chloe.Reflection;
using Chloe.Utility;
using System.Collections;
using System.Data;
using System.Threading;

namespace Chloe.Query.Internals
{
    class InternalQuery<T> : IEnumerable<T>, IAsyncEnumerable<T>
    {
        Query<T> _query;

        internal InternalQuery(Query<T> query)
        {
            this._query = query;
        }

        DbCommandFactor GenerateCommandFactor()
        {
            IQueryState qs = QueryExpressionResolver.Resolve(this._query.QueryExpression, new ScopeParameterDictionary(), new StringSet());
            MappingData data = qs.GenerateMappingData();

            IObjectActivator objectActivator;
            if (this._query._trackEntity)
                objectActivator = data.ObjectActivatorCreator.CreateObjectActivator(this._query.DbContext);
            else
                objectActivator = data.ObjectActivatorCreator.CreateObjectActivator();

            IDbExpressionTranslator translator = this._query.DbContext.DatabaseProvider.CreateDbExpressionTranslator();
            DbCommandInfo dbCommandInfo = translator.Translate(data.SqlQuery);

            DbCommandFactor commandFactor = new DbCommandFactor(objectActivator, dbCommandInfo.CommandText, dbCommandInfo.GetParameters());
            return commandFactor;
        }

        public IEnumerator<T> GetEnumerator()
        {
            DbCommandFactor commandFactor = this.GenerateCommandFactor();
            QueryEnumerator<T> enumerator = new QueryEnumerator<T>(async (@async) =>
            {
                IDataReader dataReader = await this._query.DbContext.Session.ExecuteReader(commandFactor.CommandText, CommandType.Text, commandFactor.Parameters, @async);

                return DataReaderReady(dataReader, commandFactor.ObjectActivator);
            }, commandFactor.ObjectActivator);
            return enumerator;
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        IAsyncEnumerator<T> IAsyncEnumerable<T>.GetAsyncEnumerator(CancellationToken cancellationToken)
        {
            IAsyncEnumerator<T> enumerator = this.GetEnumerator() as IAsyncEnumerator<T>;
            return enumerator;
        }

        public override string ToString()
        {
            DbCommandFactor commandFactor = this.GenerateCommandFactor();
            return AppendDbCommandInfo(commandFactor.CommandText, commandFactor.Parameters);
        }

        static IDataReader DataReaderReady(IDataReader dataReader, IObjectActivator objectActivator)
        {
            if (objectActivator is RootEntityActivator)
            {
                dataReader = new QueryDataReader(dataReader);
            }

            objectActivator.Prepare(dataReader);

            return dataReader;
        }

        static string AppendDbCommandInfo(string cmdText, DbParam[] parameters)
        {
            StringBuilder sb = new StringBuilder();
            if (parameters != null)
            {
                foreach (DbParam param in parameters)
                {
                    if (param == null)
                        continue;

                    string typeName = null;
                    object value = null;
                    Type parameterType;
                    if (param.Value == null || param.Value == DBNull.Value)
                    {
                        parameterType = param.Type;
                        value = "NULL";
                    }
                    else
                    {
                        value = param.Value;
                        parameterType = param.Value.GetType();

                        if (parameterType == typeof(string) || parameterType == typeof(DateTime))
                            value = "'" + value + "'";
                    }

                    if (parameterType != null)
                        typeName = GetTypeName(parameterType);

                    sb.AppendFormat("{0} {1} = {2};", typeName, param.Name, value);
                    sb.AppendLine();
                }
            }

            sb.AppendLine(cmdText);

            return sb.ToString();
        }
        static string GetTypeName(Type type)
        {
            Type underlyingType;
            if (ReflectionExtension.IsNullable(type, out underlyingType))
            {
                return string.Format("Nullable<{0}>", GetTypeName(underlyingType));
            }

            return type.Name;
        }
    }
}
