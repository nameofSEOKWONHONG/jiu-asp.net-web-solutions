using Chloe.Reflection;

namespace Chloe.Mapper.Binders
{
    public class ComplexMemberBinder : MemberBinder, IMemberBinder
    {
        public ComplexMemberBinder(MemberSetter setter, IObjectActivator activtor) : base(setter, activtor)
        {
        }
    }
}
