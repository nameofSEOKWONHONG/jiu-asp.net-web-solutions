namespace Chloe.Entity
{
    public interface ICollectionPropertyBuilder
    {
        IEntityTypeBuilder DeclaringBuilder { get; }
        CollectionProperty Property { get; }
    }

    public interface ICollectionPropertyBuilder<TProperty, TEntity> : ICollectionPropertyBuilder
    {
        new IEntityTypeBuilder<TEntity> DeclaringBuilder { get; }
    }
}
