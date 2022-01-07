using Chloe.Data;
using Chloe.Exceptions;
using System.Data;

#if netfx
using ObjectResultTask = System.Threading.Tasks.Task<object>;
#else
using ObjectResultTask = System.Threading.Tasks.ValueTask<object>;
#endif

namespace Chloe.Mapper.Activators
{
    public class PrimitiveObjectActivator : ObjectActivatorBase, IObjectActivator
    {
        Type _primitiveType;
        int _readerOrdinal;
        IDbValueReader _dbValueReader;

        public PrimitiveObjectActivator(Type primitiveType, int readerOrdinal)
        {
            this._primitiveType = primitiveType;
            this._readerOrdinal = readerOrdinal;
            this._dbValueReader = DataReaderConstant.GetDbValueReader(primitiveType);
        }

        public override async ObjectResultTask CreateInstance(IDataReader reader, bool @async)
        {
            try
            {
                return this._dbValueReader.GetValue(reader, this._readerOrdinal);
            }
            catch (Exception ex)
            {
                throw new ChloeException(ComplexObjectActivator.AppendErrorMsg(reader, this._readerOrdinal, ex), ex);
            }
        }
    }
}
