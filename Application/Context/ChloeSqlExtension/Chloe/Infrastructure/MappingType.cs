using System.Data;

namespace Chloe.Infrastructure
{
    public class MappingType
    {
        public MappingType(Type type)
        {
            this.Type = type;
            this.DbValueConverter = new DbValueConverter(type);
            this.DbParameterAssembler = Chloe.Infrastructure.DbParameterAssembler.Default;
        }
        public Type Type { get; private set; }
        public DbType DbType { get; set; } = DbType.Object;
        public IDbValueConverter DbValueConverter { get; set; }
        public IDbParameterAssembler DbParameterAssembler { get; set; }
    }
}
