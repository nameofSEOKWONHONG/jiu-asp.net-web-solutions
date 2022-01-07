using System.Data;

namespace Chloe.Entity
{
    public interface IPrimitivePropertyBuilder
    {
        IEntityTypeBuilder DeclaringBuilder { get; }
        PrimitiveProperty Property { get; }
        IPrimitivePropertyBuilder MapTo(string column);
        IPrimitivePropertyBuilder HasAnnotation(object value);
        IPrimitivePropertyBuilder IsPrimaryKey(bool isPrimaryKey = true);
        IPrimitivePropertyBuilder IsAutoIncrement(bool autoIncrement = true);
        IPrimitivePropertyBuilder IsNullable(bool isNullable = true);
        IPrimitivePropertyBuilder IsRowVersion(bool isRowVersion = true);
        IPrimitivePropertyBuilder HasDbType(DbType dbType);
        IPrimitivePropertyBuilder HasSize(int? size);
        IPrimitivePropertyBuilder HasScale(byte? scale);
        IPrimitivePropertyBuilder HasPrecision(byte? precision);
        IPrimitivePropertyBuilder HasSequence(string name, string schema = null);
        IPrimitivePropertyBuilder UpdateIgnore(bool updateIgnore = true);
    }
    public interface IPrimitivePropertyBuilder<TProperty, TEntity> : IPrimitivePropertyBuilder
    {
        new IEntityTypeBuilder<TEntity> DeclaringBuilder { get; }
        new IPrimitivePropertyBuilder<TProperty, TEntity> MapTo(string column);
        new IPrimitivePropertyBuilder<TProperty, TEntity> HasAnnotation(object value);
        new IPrimitivePropertyBuilder<TProperty, TEntity> IsPrimaryKey(bool isPrimaryKey = true);
        new IPrimitivePropertyBuilder<TProperty, TEntity> IsAutoIncrement(bool autoIncrement = true);
        new IPrimitivePropertyBuilder<TProperty, TEntity> IsNullable(bool isNullable = true);
        new IPrimitivePropertyBuilder<TProperty, TEntity> IsRowVersion(bool isRowVersion = true);
        new IPrimitivePropertyBuilder<TProperty, TEntity> HasDbType(DbType dbType);
        new IPrimitivePropertyBuilder<TProperty, TEntity> HasSize(int? size);
        new IPrimitivePropertyBuilder<TProperty, TEntity> HasScale(byte? scale);
        new IPrimitivePropertyBuilder<TProperty, TEntity> HasPrecision(byte? precision);
        new IPrimitivePropertyBuilder<TProperty, TEntity> HasSequence(string name, string schema = null);
        /// <summary>
        /// 更新忽略
        /// </summary>
        /// <param name="updateIgnore"></param>
        /// <returns></returns>
        new IPrimitivePropertyBuilder<TProperty, TEntity> UpdateIgnore(bool updateIgnore = true);
    }
}
