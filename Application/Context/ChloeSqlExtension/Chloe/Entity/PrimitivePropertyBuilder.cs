using System.Data;

namespace Chloe.Entity
{
    public class PrimitivePropertyBuilder<TProperty, TEntity> : IPrimitivePropertyBuilder<TProperty, TEntity>
    {
        public PrimitivePropertyBuilder(PrimitiveProperty property, IEntityTypeBuilder<TEntity> declaringBuilder)
        {
            this.Property = property;
            this.DeclaringBuilder = declaringBuilder;
        }

        IEntityTypeBuilder IPrimitivePropertyBuilder.DeclaringBuilder { get { return this.DeclaringBuilder; } }
        public IEntityTypeBuilder<TEntity> DeclaringBuilder { get; }

        public PrimitiveProperty Property { get; private set; }

        IPrimitivePropertyBuilder AsNonGenericBuilder()
        {
            return this;
        }

        public IPrimitivePropertyBuilder<TProperty, TEntity> MapTo(string column)
        {
            this.AsNonGenericBuilder().MapTo(column);
            return this;
        }
        IPrimitivePropertyBuilder IPrimitivePropertyBuilder.MapTo(string column)
        {
            this.Property.ColumnName = column;
            return this;
        }

        public IPrimitivePropertyBuilder<TProperty, TEntity> HasAnnotation(object value)
        {
            this.AsNonGenericBuilder().HasAnnotation(value);
            return this;
        }
        IPrimitivePropertyBuilder IPrimitivePropertyBuilder.HasAnnotation(object value)
        {
            if (value == null)
                throw new ArgumentNullException();

            this.Property.Annotations.Add(value);
            return this;
        }

        public IPrimitivePropertyBuilder<TProperty, TEntity> IsPrimaryKey(bool isPrimaryKey = true)
        {
            this.AsNonGenericBuilder().IsPrimaryKey(isPrimaryKey);
            return this;
        }
        IPrimitivePropertyBuilder IPrimitivePropertyBuilder.IsPrimaryKey(bool isPrimaryKey)
        {
            this.Property.IsPrimaryKey = isPrimaryKey;
            return this;
        }

        public IPrimitivePropertyBuilder<TProperty, TEntity> IsAutoIncrement(bool isAutoIncrement = true)
        {
            this.AsNonGenericBuilder().IsAutoIncrement(isAutoIncrement);
            return this;
        }
        IPrimitivePropertyBuilder IPrimitivePropertyBuilder.IsAutoIncrement(bool isAutoIncrement)
        {
            this.Property.IsAutoIncrement = isAutoIncrement;
            if (isAutoIncrement)
            {
                this.Property.SequenceName = null;
                this.Property.SequenceSchema = null;
            }

            return this;
        }

        public IPrimitivePropertyBuilder<TProperty, TEntity> IsNullable(bool isNullable = true)
        {
            this.AsNonGenericBuilder().IsNullable(isNullable);
            return this;
        }
        IPrimitivePropertyBuilder IPrimitivePropertyBuilder.IsNullable(bool isNullable)
        {
            this.Property.IsNullable = isNullable;
            return this;
        }

        public IPrimitivePropertyBuilder<TProperty, TEntity> IsRowVersion(bool isRowVersion = true)
        {
            this.AsNonGenericBuilder().IsRowVersion(isRowVersion);
            return this;
        }
        IPrimitivePropertyBuilder IPrimitivePropertyBuilder.IsRowVersion(bool isRowVersion)
        {
            this.Property.IsRowVersion = isRowVersion;
            return this;
        }

        public IPrimitivePropertyBuilder<TProperty, TEntity> HasDbType(DbType dbType)
        {
            this.AsNonGenericBuilder().HasDbType(dbType);
            return this;
        }
        IPrimitivePropertyBuilder IPrimitivePropertyBuilder.HasDbType(DbType dbType)
        {
            this.Property.DbType = dbType;
            return this;
        }

        public IPrimitivePropertyBuilder<TProperty, TEntity> HasSize(int? size)
        {
            this.AsNonGenericBuilder().HasSize(size);
            return this;
        }
        IPrimitivePropertyBuilder IPrimitivePropertyBuilder.HasSize(int? size)
        {
            this.Property.Size = size;
            return this;
        }

        public IPrimitivePropertyBuilder<TProperty, TEntity> HasScale(byte? scale)
        {
            this.AsNonGenericBuilder().HasScale(scale);
            return this;
        }
        IPrimitivePropertyBuilder IPrimitivePropertyBuilder.HasScale(byte? scale)
        {
            this.Property.Scale = scale;
            return this;
        }

        public IPrimitivePropertyBuilder<TProperty, TEntity> HasPrecision(byte? precision)
        {
            this.AsNonGenericBuilder().HasPrecision(precision);
            return this;
        }
        IPrimitivePropertyBuilder IPrimitivePropertyBuilder.HasPrecision(byte? precision)
        {
            this.Property.Precision = precision;
            return this;
        }

        public IPrimitivePropertyBuilder<TProperty, TEntity> HasSequence(string name, string schema)
        {
            this.AsNonGenericBuilder().HasSequence(name, schema);
            return this;
        }
        IPrimitivePropertyBuilder IPrimitivePropertyBuilder.HasSequence(string name, string schema)
        {
            this.Property.SequenceName = name;
            this.Property.SequenceSchema = schema;
            if (!string.IsNullOrEmpty(name))
            {
                this.Property.IsAutoIncrement = false;
            }

            return this;
        }

        public IPrimitivePropertyBuilder<TProperty, TEntity> UpdateIgnore(bool updateIgnore = true)
        {
            this.AsNonGenericBuilder().UpdateIgnore(updateIgnore);
            return this;
        }
        IPrimitivePropertyBuilder IPrimitivePropertyBuilder.UpdateIgnore(bool updateIgnore)
        {
            this.Property.UpdateIgnore = updateIgnore;
            return this;
        }
    }
}
