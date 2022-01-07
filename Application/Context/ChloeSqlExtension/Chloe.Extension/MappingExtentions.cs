using System.Linq.Expressions;

namespace Chloe.Entity
{
    public static class MappingExtentions
    {
        public static IEntityTypeBuilder<TEntity> UpdateIgnore<TEntity, TProperty>(this IEntityTypeBuilder<TEntity> entityTypeBuilder, Expression<Func<TEntity, TProperty>> property)
        {
            entityTypeBuilder.Property(property).UpdateIgnore();
            return entityTypeBuilder;
        }
    }
}
