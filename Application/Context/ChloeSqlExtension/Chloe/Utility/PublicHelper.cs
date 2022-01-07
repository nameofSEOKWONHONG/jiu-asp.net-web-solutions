using Chloe.DbExpressions;
using Chloe.Descriptors;
using Chloe.Exceptions;
using Chloe.Infrastructure;
using Chloe.InternalExtensions;
using Chloe.Reflection;
using Chloe.Utility;
using System.Reflection;

namespace Chloe
{
    public class PublicHelper
    {
        public static void CheckNull(object obj, string paramName = null)
        {
            if (obj == null)
                throw new ArgumentNullException(paramName);
        }
        public static bool AreEqual(object obj1, object obj2)
        {
            if (obj1 == null && obj2 == null)
                return true;

            if (obj1 != null)
            {
                return obj1.Equals(obj2);
            }

            if (obj2 != null)
            {
                return obj2.Equals(obj1);
            }

            return object.Equals(obj1, obj2);
        }
        public static DbMethodCallExpression MakeNextValueForSequenceDbExpression(PrimitivePropertyDescriptor propertyDescriptor, string defaultSequenceSchema)
        {
            string sequenceSchema = propertyDescriptor.Definition.SequenceSchema;
            sequenceSchema = string.IsNullOrEmpty(sequenceSchema) ? defaultSequenceSchema : sequenceSchema;
            return MakeNextValueForSequenceDbExpression(propertyDescriptor.PropertyType, propertyDescriptor.Definition.SequenceName, sequenceSchema);
        }
        public static DbMethodCallExpression MakeNextValueForSequenceDbExpression(Type retType, string sequenceName, string sequenceSchema)
        {
            MethodInfo nextValueForSequenceMethod = PublicConstants.MethodInfo_Sql_NextValueForSequence.MakeGenericMethod(retType);
            List<DbExpression> arguments = new List<DbExpression>(2) { new DbConstantExpression(sequenceName), new DbConstantExpression(sequenceSchema) };

            DbMethodCallExpression getNextValueForSequenceExp = new DbMethodCallExpression(null, nextValueForSequenceMethod, arguments);
            return getNextValueForSequenceExp;
        }
        public static object ConvertObjectType(object obj, Type conversionType)
        {
            if (obj == null)
                return null;

            Type objType = obj.GetType();

            if (objType == conversionType)
                return obj;

            conversionType = conversionType.GetUnderlyingType();
            if (objType != conversionType)
                return Convert.ChangeType(obj, conversionType);

            return obj;
        }

        public static List<T> Clone<T>(List<T> source, int? capacity = null)
        {
            List<T> ret = new List<T>(capacity ?? source.Count);
            ret.AddRange(source);
            return ret;
        }
        public static List<T> CloneAndAppendOne<T>(List<T> source, T t)
        {
            List<T> ret = new List<T>(source.Count + 1);
            ret.AddRange(source);
            ret.Add(t);
            return ret;
        }
        public static Dictionary<TKey, TValue> Clone<TKey, TValue>(Dictionary<TKey, TValue> source)
        {
            Dictionary<TKey, TValue> ret = Clone<TKey, TValue>(source, source.Count);
            return ret;
        }
        public static Dictionary<TKey, TValue> Clone<TKey, TValue>(Dictionary<TKey, TValue> source, int capacity)
        {
            Dictionary<TKey, TValue> ret = new Dictionary<TKey, TValue>(capacity);

            foreach (var kv in source)
            {
                ret.Add(kv.Key, kv.Value);
            }

            return ret;
        }

        public static void EnsureHasPrimaryKey(TypeDescriptor typeDescriptor)
        {
            if (!typeDescriptor.HasPrimaryKey())
                throw new ChloeException(string.Format("The entity type '{0}' does not define any primary key.", typeDescriptor.Definition.Type.FullName));
        }

        public static DbParam[] BuildParams(DbContext dbContext, object parameter)
        {
            if (parameter == null)
                return new DbParam[0];

            if (parameter is IEnumerable<DbParam>)
            {
                return ((IEnumerable<DbParam>)parameter).ToArray();
            }

            List<DbParam> parameters = new List<DbParam>();
            Type parameterType = parameter.GetType();
            var props = parameterType.GetProperties();
            foreach (var prop in props)
            {
                if (prop.GetGetMethod() == null || !MappingTypeSystem.IsMappingType(prop.GetMemberType()))
                {
                    continue;
                }

                object value = ReflectionExtension.FastGetMemberValue(prop, parameter);

                string paramName = dbContext.DatabaseProvider.CreateParameterName(prop.Name);

                DbParam p = new DbParam(paramName, value, prop.PropertyType);
                parameters.Add(p);
            }

            return parameters.ToArray();
        }

        public static void NotNullCheck(PrimitivePropertyDescriptor propertyDescriptor, object val)
        {
            if (!propertyDescriptor.IsNullable && val == null)
            {
                throw new ChloeException($"The property '{propertyDescriptor.Property.Name}' can not be null.");
            }
        }

        public static object IncreaseRowVersionNumber(object val)
        {
            if (val.GetType() == PublicConstants.TypeOfInt32)
            {
                return (int)val + 1;
            }

            return (long)val + 1;
        }

        public static DbExpression MakeCondition(PairList<PrimitivePropertyDescriptor, object> propertyValuePairs, DbTable dbTable)
        {
            DbExpression conditionExp = null;
            foreach (var pair in propertyValuePairs)
            {
                PrimitivePropertyDescriptor propertyDescriptor = pair.Item1;
                object val = pair.Item2;

                DbExpression left = new DbColumnAccessExpression(dbTable, propertyDescriptor.Column);
                DbExpression right = DbExpression.Parameter(val, propertyDescriptor.PropertyType, propertyDescriptor.Column.DbType);
                DbExpression equalExp = new DbEqualExpression(left, right);
                conditionExp = conditionExp.And(equalExp);
            }

            return conditionExp;
        }

        public static void CauseErrorIfOptimisticUpdateFailed(int rowsAffected)
        {
            if (rowsAffected <= 0)
                throw new OptimisticConcurrencyException();
        }

        public static DbTable CreateDbTable(TypeDescriptor typeDescriptor, string table)
        {
            return typeDescriptor.GenDbTable(table);
        }
    }
}
