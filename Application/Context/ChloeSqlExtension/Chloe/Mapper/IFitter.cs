using Chloe.Descriptors;
using System.Collections;
using System.Data;

#if netfx
using VoidTask = System.Threading.Tasks.Task;
#else
using VoidTask = System.Threading.Tasks.ValueTask;
#endif

namespace Chloe.Mapper
{
    /// <summary>
    /// 集合填充器
    /// </summary>
    public interface IFitter
    {
        void Prepare(IDataReader reader);
        VoidTask Fill(object obj, object owner, IDataReader reader, bool @async);
    }

    public class ComplexObjectFitter : IFitter
    {
        List<Tuple<PropertyDescriptor, IFitter>> _includings;

        public ComplexObjectFitter(List<Tuple<PropertyDescriptor, IFitter>> includings)
        {
            this._includings = includings;
        }

        public void Prepare(IDataReader reader)
        {
            for (int i = 0; i < this._includings.Count; i++)
            {
                var kv = this._includings[i];
                kv.Item2.Prepare(reader);
            }
        }

        public async VoidTask Fill(object entity, object owner, IDataReader reader, bool @async)
        {
            for (int i = 0; i < this._includings.Count; i++)
            {
                var kv = this._includings[i];

                var propertyValue = kv.Item1.GetValue(entity);
                if (propertyValue == null)
                    continue;

                await kv.Item2.Fill(propertyValue, entity, reader, @async);
            }
        }
    }
    public class CollectionObjectFitter : IFitter
    {
        IObjectActivator _elementActivator;
        IEntityKey _entityKey;
        IFitter _elementFitter;
        PropertyDescriptor _elementOwnerProperty;

        HashSet<object> _keySet = new HashSet<object>();
        object _collection;

        public CollectionObjectFitter(IObjectActivator elementActivator, IEntityKey entityKey, IFitter elementFitter, PropertyDescriptor elementOwnerProperty)
        {
            this._elementActivator = elementActivator;
            this._entityKey = entityKey;
            this._elementFitter = elementFitter;
            this._elementOwnerProperty = elementOwnerProperty;
        }

        public void Prepare(IDataReader reader)
        {
            this._elementActivator.Prepare(reader);
            this._elementFitter.Prepare(reader);
        }

        public async VoidTask Fill(object collection, object owner, IDataReader reader, bool @async)
        {
            if (this._collection != collection)
            {
                this._keySet.Clear();
                this._collection = collection;
            }

            IList entityContainer = collection as IList;

            object entity = null;

            var keyValue = this._entityKey.GetKeyValue(reader);
            if (!this._keySet.Contains(keyValue))
            {
                entity = await this._elementActivator.CreateInstance(reader, @async);
                if (entity != null)
                {
                    this._elementOwnerProperty.SetValue(entity, owner); //entity.XX = owner
                    entityContainer.Add(entity);
                    this._keySet.Add(keyValue);
                }
            }

            if (entityContainer.Count > 0)
            {
                entity = entityContainer[entityContainer.Count - 1];
                await this._elementFitter.Fill(entity, null, reader, @async);
            }
        }
    }
}
