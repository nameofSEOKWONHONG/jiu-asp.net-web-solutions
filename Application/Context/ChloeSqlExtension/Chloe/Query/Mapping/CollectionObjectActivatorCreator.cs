using Chloe.Descriptors;
using Chloe.Infrastructure;
using Chloe.Mapper;
using Chloe.Mapper.Activators;

namespace Chloe.Query.Mapping
{
    public class CollectionObjectActivatorCreator : IObjectActivatorCreator
    {
        public CollectionObjectActivatorCreator(Type collectionType, Type ownerType, IObjectActivatorCreator elementActivatorCreator)
        {
            this.CollectionType = collectionType;
            this.OwnerType = ownerType;
            this.ElementActivatorCreator = elementActivatorCreator;
        }

        public Type ObjectType { get { return this.CollectionType; } }
        public bool IsRoot { get; set; }
        public Type CollectionType { get; private set; }
        public Type OwnerType { get; private set; }
        public IObjectActivatorCreator ElementActivatorCreator { get; private set; }

        public IObjectActivator CreateObjectActivator()
        {
            return this.CreateObjectActivator(null);
        }
        public IObjectActivator CreateObjectActivator(IDbContext dbContext)
        {
            CollectionObjectActivator ret = new CollectionObjectActivator(this.CollectionType);
            return ret;
        }

        public IFitter CreateFitter(IDbContext dbContext)
        {
            IFitter elementFitter = this.ElementActivatorCreator.CreateFitter(dbContext);

            ComplexObjectActivatorCreator elementActivatorCreator = (ComplexObjectActivatorCreator)this.ElementActivatorCreator;
            TypeDescriptor elementTypeDescriptor = EntityTypeContainer.GetDescriptor(elementActivatorCreator.ObjectType);

            List<Tuple<PropertyDescriptor, int>> keys = new List<Tuple<PropertyDescriptor, int>>(elementTypeDescriptor.PrimaryKeys.Count);
            foreach (PrimitivePropertyDescriptor primaryKey in elementTypeDescriptor.PrimaryKeys)
            {
                keys.Add(new Tuple<PropertyDescriptor, int>(primaryKey, elementActivatorCreator.PrimitiveMembers[primaryKey.Definition.Property]));
            }

            IEntityKey entityKey = new EntityKey(keys);

            PropertyDescriptor elementOwnerProperty = elementTypeDescriptor.ComplexPropertyDescriptors.Where(a => a.Definition.Property.PropertyType == this.OwnerType).First();

            CollectionObjectFitter fitter = new CollectionObjectFitter(this.ElementActivatorCreator.CreateObjectActivator(dbContext), entityKey, elementFitter, elementOwnerProperty);
            return fitter;
        }
    }
}
