using System.Linq.Expressions;

namespace Chloe.Entity
{
    public interface IEntityTypeBuilder
    {
        EntityType EntityType { get; }
        IEntityTypeBuilder MapTo(string table);
        IEntityTypeBuilder HasSchema(string schema);
        IEntityTypeBuilder HasAnnotation(object value);
        IEntityTypeBuilder Ignore(string property);
        IEntityTypeBuilder HasQueryFilter(LambdaExpression filter);
        IPrimitivePropertyBuilder Property(string property);
        IComplexPropertyBuilder HasOne(string property);
        ICollectionPropertyBuilder HasMany(string property);
    }

    public interface IEntityTypeBuilder<TEntity> : IEntityTypeBuilder
    {
        new IEntityTypeBuilder<TEntity> MapTo(string table);
        new IEntityTypeBuilder<TEntity> HasSchema(string schema);
        new IEntityTypeBuilder<TEntity> HasAnnotation(object value);
        IEntityTypeBuilder<TEntity> Ignore(Expression<Func<TEntity, object>> property);
        IEntityTypeBuilder<TEntity> HasQueryFilter(Expression<Func<TEntity, bool>> filter);
        IPrimitivePropertyBuilder<TProperty, TEntity> Property<TProperty>(Expression<Func<TEntity, TProperty>> property);
        IComplexPropertyBuilder<TProperty, TEntity> HasOne<TProperty>(Expression<Func<TEntity, TProperty>> property);
        ICollectionPropertyBuilder<TProperty, TEntity> HasMany<TProperty>(Expression<Func<TEntity, TProperty>> property);
    }
}
