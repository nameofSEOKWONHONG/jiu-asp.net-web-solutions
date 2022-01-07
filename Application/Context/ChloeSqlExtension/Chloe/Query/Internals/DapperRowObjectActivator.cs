using Chloe.Mapper;
using Chloe.Mapper.Activators;
using System.Data;

#if netfx
using ObjectResultTask = System.Threading.Tasks.Task<object>;
#else
using ObjectResultTask = System.Threading.Tasks.ValueTask<object>;
#endif

namespace Chloe.Query.Internals
{
    class DapperRowObjectActivator : ObjectActivatorBase, IObjectActivator
    {
        DapperTable _table = null;
        public DapperRowObjectActivator()
        {
        }

        public override async ObjectResultTask CreateInstance(IDataReader reader, bool @async)
        {
            int effectiveFieldCount = reader.FieldCount;
            if (this._table == null)
            {
                string[] names = new string[effectiveFieldCount];
                for (int i = 0; i < effectiveFieldCount; i++)
                {
                    names[i] = reader.GetName(i);
                }
                this._table = new DapperTable(names);
            }

            var values = new object[effectiveFieldCount];

            reader.GetValues(values);
            for (int i = 0; i < values.Length; i++)
                if (values[i] is DBNull) values[i] = null;

            var row = new DapperRow(this._table, values);
            return row;
        }
    }
}
