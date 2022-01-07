using System.Data;

namespace Chloe.Infrastructure
{
    public class MappingTypeBuilder
    {
        MappingType _mappingType;
        public MappingTypeBuilder(MappingType mappingType)
        {
            this._mappingType = mappingType;
        }

        public MappingTypeBuilder HasDbType(DbType dbType)
        {
            this._mappingType.DbType = dbType;
            return this;
        }
        public MappingTypeBuilder HasDbValueConverter(IDbValueConverter dbValueConverter)
        {
            if (dbValueConverter == null)
                throw new ArgumentNullException(nameof(dbValueConverter));

            this._mappingType.DbValueConverter = dbValueConverter;
            return this;
        }
        public MappingTypeBuilder HasDbValueConverter(Func<object, object> dbValueConverter)
        {
            if (dbValueConverter == null)
                throw new ArgumentNullException(nameof(dbValueConverter));

            this._mappingType.DbValueConverter = new InternalDbValueConverter(dbValueConverter);
            return this;
        }
        public MappingTypeBuilder HasDbValueConverter<TConverter>() where TConverter : IDbValueConverter
        {
            IDbValueConverter dbValueConverter = Activator.CreateInstance(typeof(TConverter)) as IDbValueConverter;
            return this.HasDbValueConverter(dbValueConverter);
        }

        public MappingTypeBuilder HasDbParameterAssembler(IDbParameterAssembler dbParameterAssembler)
        {
            if (dbParameterAssembler == null)
                throw new ArgumentNullException(nameof(dbParameterAssembler));

            this._mappingType.DbParameterAssembler = dbParameterAssembler;
            return this;
        }
        public MappingTypeBuilder HasDbParameterAssembler<TAssembler>() where TAssembler : IDbParameterAssembler
        {
            IDbParameterAssembler dbParameterAssembler = Activator.CreateInstance(typeof(TAssembler)) as IDbParameterAssembler;
            return this.HasDbParameterAssembler(dbParameterAssembler);
        }
    }
}
