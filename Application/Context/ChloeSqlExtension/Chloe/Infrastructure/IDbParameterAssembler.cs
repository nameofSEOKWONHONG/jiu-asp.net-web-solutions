using System.Data;

namespace Chloe.Infrastructure
{
    public interface IDbParameterAssembler
    {
        /// <summary>
        /// 修正 IDbDataParameter 对象各项属性。
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="param"></param>
        void SetupParameter(IDbDataParameter parameter, DbParam param);
    }

    public class DbParameterAssembler : IDbParameterAssembler
    {
        static readonly DbType[] DbTypes = {
            DbType.Object,
            DbType.Object,
            DbType.Object,
            DbType.Boolean,
            DbType.AnsiString,
            DbType.SByte,
            DbType.Byte,
            DbType.Int16,
            DbType.UInt16,
            DbType.Int32,
            DbType.UInt32,
            DbType.Int64,
            DbType.UInt64,
            DbType.Single,
            DbType.Double,
            DbType.Decimal,
            DbType.DateTime,
            DbType.Object,
            DbType.String
        };

        public static readonly DbParameterAssembler Default = new DbParameterAssembler();

        public virtual void SetupParameter(IDbDataParameter parameter, DbParam param)
        {
            parameter.ParameterName = param.Name;

            Type parameterType = null;
            if (param.Value == null || param.Value == DBNull.Value)
            {
                parameter.Value = DBNull.Value;
                parameterType = param.Type ?? typeof(object);
            }
            else
            {
                parameterType = param.Value.GetType();
                if (parameterType.IsEnum)
                {
                    parameterType = Enum.GetUnderlyingType(parameterType);
                    parameter.Value = Convert.ChangeType(param.Value, parameterType);
                }
                else
                {
                    parameter.Value = param.Value;
                }
            }

            if (param.Precision != null)
                parameter.Precision = param.Precision.Value;

            if (param.Scale != null)
                parameter.Scale = param.Scale.Value;

            if (param.Size != null)
                parameter.Size = param.Size.Value;

            if (param.DbType != null)
                parameter.DbType = param.DbType.Value;
            else
            {
                TypeCode typeCode = Convert.GetTypeCode(parameter.Value);
                DbType dbType = DbTypes[(int)typeCode];
                if (dbType != DbType.Object)
                {
                    parameter.DbType = dbType;
                }
                else
                {
                    var t = MappingTypeSystem.GetDbType(parameterType);
                    if (t != null)
                        parameter.DbType = t.Value;
                }
            }

            const int defaultSizeOfStringOutputParameter = 4000;/* 当一个 string 类型输出参数未显示指定 Size 时使用的默认大小。如果有需要更大或者该值不足以满足需求，需显示指定 DbParam.Size 值 */

            if (param.Direction == ParamDirection.Input)
            {
                parameter.Direction = ParameterDirection.Input;
            }
            else if (param.Direction == ParamDirection.Output)
            {
                parameter.Direction = ParameterDirection.Output;
                param.Value = null;
                if (param.Size == null && param.Type == PublicConstants.TypeOfString)
                {
                    parameter.Size = defaultSizeOfStringOutputParameter;
                }
            }
            else if (param.Direction == ParamDirection.InputOutput)
            {
                parameter.Direction = ParameterDirection.InputOutput;
                if (param.Size == null && param.Type == PublicConstants.TypeOfString)
                {
                    parameter.Size = defaultSizeOfStringOutputParameter;
                }
            }
            else if (param.Direction == ParamDirection.ReturnValue)
            {
                parameter.Direction = ParameterDirection.ReturnValue;
            }
            else
                throw new NotSupportedException(string.Format("ParamDirection '{0}' is not supported.", param.Direction));
        }
    }
}
