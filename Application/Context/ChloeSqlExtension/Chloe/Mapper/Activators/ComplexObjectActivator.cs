using Chloe.Exceptions;
using Chloe.Mapper.Binders;
using Chloe.Reflection;
using System.Data;

#if netfx
using ObjectResultTask = System.Threading.Tasks.Task<object>;
#else
using ObjectResultTask = System.Threading.Tasks.ValueTask<object>;
#endif

namespace Chloe.Mapper.Activators
{
    public class ComplexObjectActivator : ObjectActivatorBase, IObjectActivator
    {
        InstanceCreator _instanceCreator;
        List<IObjectActivator> _argumentActivators;
        List<IMemberBinder> _memberBinders;
        int? _checkNullOrdinal;

        public ComplexObjectActivator(InstanceCreator instanceCreator, List<IObjectActivator> argumentActivators, List<IMemberBinder> memberBinders, int? checkNullOrdinal)
        {
            this._instanceCreator = instanceCreator;
            this._argumentActivators = argumentActivators;
            this._memberBinders = memberBinders;
            this._checkNullOrdinal = checkNullOrdinal;
        }

        public override void Prepare(IDataReader reader)
        {
            for (int i = 0; i < this._argumentActivators.Count; i++)
            {
                IObjectActivator argumentActivator = this._argumentActivators[i];
                argumentActivator.Prepare(reader);
            }
            for (int i = 0; i < this._memberBinders.Count; i++)
            {
                IMemberBinder binder = this._memberBinders[i];
                binder.Prepare(reader);
            }
        }
        public override async ObjectResultTask CreateInstance(IDataReader reader, bool @async)
        {
            if (this._checkNullOrdinal != null)
            {
                if (reader.IsDBNull(this._checkNullOrdinal.Value))
                    return null;
            }


            object[] arguments = this._argumentActivators.Count == 0 ? PublicConstants.EmptyArray : new object[this._argumentActivators.Count];

            for (int i = 0; i < this._argumentActivators.Count; i++)
            {
                arguments[i] = await this._argumentActivators[i].CreateInstance(reader, @async);
            }

            object obj = this._instanceCreator(arguments);

            IMemberBinder memberBinder = null;
            try
            {
                int count = this._memberBinders.Count;
                for (int i = 0; i < count; i++)
                {
                    memberBinder = this._memberBinders[i];
                    await memberBinder.Bind(obj, reader, @async);
                }
            }
            catch (ChloeException)
            {
                throw;
            }
            catch (Exception ex)
            {
                PrimitiveMemberBinder binder = memberBinder as PrimitiveMemberBinder;
                if (binder != null)
                {
                    throw new ChloeException(AppendErrorMsg(reader, binder.Ordinal, ex), ex);
                }

                throw;
            }

            return obj;
        }

        public static string AppendErrorMsg(IDataReader reader, int ordinal, Exception ex)
        {
            string msg = null;
            if (reader.IsDBNull(ordinal))
            {
                msg = string.Format("Please make sure that the member of the column '{0}'({1},{2},{3}) map is nullable.", reader.GetName(ordinal), ordinal.ToString(), reader.GetDataTypeName(ordinal), reader.GetFieldType(ordinal).FullName);
            }
            else if (ex is InvalidCastException)
            {
                msg = string.Format("Please make sure that the member of the column '{0}'({1},{2},{3}) map is the correct type.", reader.GetName(ordinal), ordinal.ToString(), reader.GetDataTypeName(ordinal), reader.GetFieldType(ordinal).FullName);
            }
            else
                msg = string.Format("An error occurred while mapping the column '{0}'({1},{2},{3}). For details please see the inner exception.", reader.GetName(ordinal), ordinal.ToString(), reader.GetDataTypeName(ordinal), reader.GetFieldType(ordinal).FullName);
            return msg;
        }
    }

    public class ObjectActivatorWithTracking : ComplexObjectActivator
    {
        IDbContext _dbContext;
        public ObjectActivatorWithTracking(InstanceCreator instanceCreator, List<IObjectActivator> argumentActivators, List<IMemberBinder> memberBinders, int? checkNullOrdinal, IDbContext dbContext)
            : base(instanceCreator, argumentActivators, memberBinders, checkNullOrdinal)
        {
            this._dbContext = dbContext;
        }

        public override async ObjectResultTask CreateInstance(IDataReader reader, bool @async)
        {
            object obj = await base.CreateInstance(reader, @async);

            if (obj != null)
                this._dbContext.TrackEntity(obj);

            return obj;
        }
    }
}
