using System.Data;

#if netfx
using ObjectResultTask = System.Threading.Tasks.Task<object>;
#else
using ObjectResultTask = System.Threading.Tasks.ValueTask<object>;
#endif

namespace Chloe.Mapper
{
    public interface IObjectActivator
    {
        void Prepare(IDataReader reader);
        ObjectResultTask CreateInstance(IDataReader reader, bool @async);
    }
}
