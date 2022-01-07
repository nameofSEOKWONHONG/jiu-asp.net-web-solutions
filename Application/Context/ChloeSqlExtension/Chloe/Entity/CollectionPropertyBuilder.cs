namespace Chloe.Entity
{
    public class CollectionPropertyBuilder<TProperty, TEntity> : ICollectionPropertyBuilder<TProperty, TEntity>
    {
        public CollectionPropertyBuilder(CollectionProperty property, IEntityTypeBuilder<TEntity> declaringBuilder)
        {
            this.Property = property;
            this.DeclaringBuilder = declaringBuilder;
        }

        IEntityTypeBuilder ICollectionPropertyBuilder.DeclaringBuilder { get { return this.DeclaringBuilder; } }
        public IEntityTypeBuilder<TEntity> DeclaringBuilder { get; }

        public CollectionProperty Property { get; private set; }
    }
}
