using Chloe.DbExpressions;
using System.Reflection;

namespace Chloe.Entity
{
    public class PrimitivePropertyDefinition : PropertyDefinition
    {
        public PrimitivePropertyDefinition(PropertyInfo property, DbColumn column, bool isPrimaryKey, bool isAutoIncrement, bool isNullable, bool isRowVersion, string sequenceName, string sequenceSchema, bool updateIgnore, IList<object> annotations) : base(property, annotations)
        {
            PublicHelper.CheckNull(column, nameof(column));

            this.Column = column;
            this.IsPrimaryKey = isPrimaryKey;
            this.IsAutoIncrement = isAutoIncrement;
            this.IsNullable = isNullable;
            this.IsRowVersion = isRowVersion;
            this.SequenceName = sequenceName;
            this.SequenceSchema = sequenceSchema;
            this.UpdateIgnore = updateIgnore;
        }
        public override TypeKind Kind { get { return TypeKind.Primitive; } }
        public DbColumn Column { get; private set; }
        public bool IsPrimaryKey { get; private set; }
        public bool IsAutoIncrement { get; private set; }
        public bool IsNullable { get; private set; }
        public bool IsRowVersion { get; private set; }
        public string SequenceName { get; private set; }
        public string SequenceSchema { get; private set; }

        /// <summary>
        /// 更新忽略
        /// </summary>
        public bool UpdateIgnore { get; private set; }
    }
}
