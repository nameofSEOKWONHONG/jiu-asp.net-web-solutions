using Chloe.Descriptors;
using System.Reflection;

namespace Chloe.Core
{
    public interface IEntityState
    {
        object Entity { get; }
        TypeDescriptor TypeDescriptor { get; }
        bool HasChanged(PrimitivePropertyDescriptor propertyDescriptor, object val);
        void Refresh();
    }

    class EntityState : IEntityState
    {
        Dictionary<MemberInfo, object> _fakes;
        object _entity;
        TypeDescriptor _typeDescriptor;

        public EntityState(TypeDescriptor typeDescriptor, object entity)
        {
            this._typeDescriptor = typeDescriptor;
            this._entity = entity;
            this.Refresh();
        }

        public object Entity { get { return this._entity; } }
        public TypeDescriptor TypeDescriptor { get { return this._typeDescriptor; } }

        public bool HasChanged(PrimitivePropertyDescriptor propertyDescriptor, object val)
        {
            object oldVal;
            if (!this._fakes.TryGetValue(propertyDescriptor.Property, out oldVal))
            {
                return true;
            }

            if (propertyDescriptor.PropertyType == PublicConstants.TypeOfByteArray)
            {
                //byte[] is a big big hole~
                return !AreEqual((byte[])oldVal, (byte[])val);
            }

            return !PublicHelper.AreEqual(oldVal, val);
        }
        public void Refresh()
        {
            if (this._fakes == null)
            {
                this._fakes = new Dictionary<MemberInfo, object>(this.TypeDescriptor.PrimitivePropertyDescriptors.Count);
            }
            else
            {
                this._fakes.Clear();
            }

            object entity = this._entity;
            foreach (PrimitivePropertyDescriptor propertyDescriptor in this.TypeDescriptor.PrimitivePropertyDescriptors)
            {
                var val = propertyDescriptor.GetValue(entity);

                //I hate the byte[].
                if (propertyDescriptor.PropertyType == PublicConstants.TypeOfByteArray)
                {
                    val = Clone((byte[])val);
                }

                this._fakes[propertyDescriptor.Definition.Property] = val;
            }
        }

        static byte[] Clone(byte[] arr)
        {
            if (arr == null)
                return null;

            byte[] ret = new byte[arr.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                ret[i] = arr[i];
            }

            return ret;
        }
        static bool AreEqual(byte[] obj1, byte[] obj2)
        {
            if (obj1 == obj2)
                return true;

            if (obj1 != null && obj2 != null)
            {
                if (obj1.Length != obj2.Length)
                    return false;

                for (int i = 0; i < obj1.Length; i++)
                {
                    if (obj1[i] != obj2[i])
                        return false;
                }

                return true;
            }

            return false;
        }
    }
}
