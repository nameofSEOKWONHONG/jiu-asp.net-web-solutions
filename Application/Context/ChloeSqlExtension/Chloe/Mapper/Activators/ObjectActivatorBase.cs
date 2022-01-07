using System.Data;

#if netfx
using ObjectResultTask = System.Threading.Tasks.Task<object>;
#else
using ObjectResultTask = System.Threading.Tasks.ValueTask<object>;
#endif

namespace Chloe.Mapper.Activators
{
    public abstract class ObjectActivatorBase : IObjectActivator
    {
        public virtual void Prepare(IDataReader reader)
        {

        }
        public abstract ObjectResultTask CreateInstance(IDataReader reader, bool @async);
    }
}
