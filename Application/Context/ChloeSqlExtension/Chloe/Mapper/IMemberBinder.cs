using System.Data;

#if netfx
using VoidTask = System.Threading.Tasks.Task;
#else
using VoidTask = System.Threading.Tasks.ValueTask;
#endif

namespace Chloe.Mapper
{
    public interface IMemberBinder
    {
        void Prepare(IDataReader reader);
        VoidTask Bind(object obj, IDataReader reader, bool @async);
    }
}
