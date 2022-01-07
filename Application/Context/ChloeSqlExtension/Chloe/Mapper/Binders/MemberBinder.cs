using Chloe.Reflection;
using System.Data;

#if netfx
using VoidTask = System.Threading.Tasks.Task;
#else
using VoidTask = System.Threading.Tasks.ValueTask;
#endif

namespace Chloe.Mapper.Binders
{
    public class MemberBinder : IMemberBinder
    {
        MemberSetter _setter;
        IObjectActivator _activtor;
        public MemberBinder(MemberSetter setter, IObjectActivator activtor)
        {
            this._setter = setter;
            this._activtor = activtor;
        }
        public virtual void Prepare(IDataReader reader)
        {
            this._activtor.Prepare(reader);
        }
        public virtual async VoidTask Bind(object obj, IDataReader reader, bool @async)
        {
            object val = await this._activtor.CreateInstance(reader, @async);
            this._setter(obj, val);
        }
    }
}
