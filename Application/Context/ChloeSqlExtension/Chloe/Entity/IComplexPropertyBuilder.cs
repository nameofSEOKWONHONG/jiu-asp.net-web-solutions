using System.Linq.Expressions;

namespace Chloe.Entity
{
    public interface IComplexPropertyBuilder
    {
        IEntityTypeBuilder DeclaringBuilder { get; }
        ComplexProperty Property { get; }
        IComplexPropertyBuilder WithForeignKey(string foreignKey);
    }

    public interface IComplexPropertyBuilder<TProperty, TEntity> : IComplexPropertyBuilder
    {
        new IEntityTypeBuilder<TEntity> DeclaringBuilder { get; }
        new IComplexPropertyBuilder<TProperty, TEntity> WithForeignKey(string foreignKey);
        IComplexPropertyBuilder<TProperty, TEntity> WithForeignKey<TForeignKey>(Expression<Func<TEntity, TForeignKey>> foreignKey);
    }
}
