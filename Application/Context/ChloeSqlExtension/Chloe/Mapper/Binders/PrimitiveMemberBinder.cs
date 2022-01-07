using Chloe.Reflection;
using System.Data;
using System.Reflection;

#if netfx
using VoidTask = System.Threading.Tasks.Task;
#else
using VoidTask = System.Threading.Tasks.ValueTask;
#endif

namespace Chloe.Mapper.Binders
{
    public class PrimitiveMemberBinder : IMemberBinder
    {
        MemberInfo _member;
        MRMTuple _mrmTuple;
        int _ordinal;
        IMRM _mMapper;

        public PrimitiveMemberBinder(MemberInfo member, MRMTuple mrmTuple, int ordinal)
        {
            this._member = member;
            this._mrmTuple = mrmTuple;
            this._ordinal = ordinal;
        }

        public int Ordinal { get { return this._ordinal; } }

        public void Prepare(IDataReader reader)
        {
            Type fieldType = reader.GetFieldType(this._ordinal);
            if (fieldType == this._member.GetMemberType().GetUnderlyingType())
            {
                this._mMapper = this._mrmTuple.StrongMRM.Value;
                return;
            }

            this._mMapper = this._mrmTuple.SafeMRM.Value;
        }
        public virtual async VoidTask Bind(object obj, IDataReader reader, bool @async)
        {
            this._mMapper.Map(obj, reader, this._ordinal);
        }
    }
}
