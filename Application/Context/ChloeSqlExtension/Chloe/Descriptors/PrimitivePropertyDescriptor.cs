using Chloe.DbExpressions;
using Chloe.Entity;

namespace Chloe.Descriptors
{
    public class PrimitivePropertyDescriptor : PropertyDescriptor
    {
        public PrimitivePropertyDescriptor(PrimitivePropertyDefinition definition, TypeDescriptor declaringTypeDescriptor) : base(definition, declaringTypeDescriptor)
        {
            this.Definition = definition;
        }

        public new PrimitivePropertyDefinition Definition { get; private set; }

        public bool IsPrimaryKey { get { return this.Definition.IsPrimaryKey; } }
        public bool IsAutoIncrement { get { return this.Definition.IsAutoIncrement; } }
        public bool IsNullable { get { return this.Definition.IsNullable; } }
        public bool IsRowVersion { get { return this.Definition.IsRowVersion; } }

        /// <summary>
        /// 更新忽略
        /// </summary>
        public bool UpdateIgnore { get { return this.Definition.UpdateIgnore; } }

        public DbColumn Column { get { return this.Definition.Column; } }


        public bool HasSequence()
        {
            return !string.IsNullOrEmpty(this.Definition.SequenceName);
        }

        public bool CannotUpdate()
        {
            return this.IsPrimaryKey || this.IsAutoIncrement || this.HasSequence() || this.IsRowVersion || this.UpdateIgnore;
        }
    }
}
