using Chloe.Entity;

namespace Chloe.SQLite
{
    public static class PrimitivePropertyBuilderExtension
    {
        public static IPrimitivePropertyBuilder HasSQLiteDataType(this IPrimitivePropertyBuilder propertyBuilder, string name)
        {
            DataTypeAttribute dataTypeAttribute = new DataTypeAttribute(name);
            propertyBuilder.HasAnnotation(dataTypeAttribute);
            return propertyBuilder;
        }

        public static IPrimitivePropertyBuilder<TProperty, TEntity> HasSQLiteDataType<TProperty, TEntity>(this IPrimitivePropertyBuilder<TProperty, TEntity> propertyBuilder, string name)
        {
            (propertyBuilder as IPrimitivePropertyBuilder).HasSQLiteDataType(name);
            return propertyBuilder;
        }
    }
}
