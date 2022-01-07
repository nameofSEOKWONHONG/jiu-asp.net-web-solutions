using Chloe.DbExpressions;
using Chloe.InternalExtensions;

namespace Chloe.Oracle
{
    static class DbExpressionHelper
    {
        /// <summary>
        /// 判定 exp 返回值肯定是 null 或 ''
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static bool AffirmExpressionRetValueIsNullOrEmpty(this DbExpression exp)
        {
            exp = DbExpressionExtension.StripConvert(exp);

            if (exp.NodeType == DbExpressionType.Constant)
            {
                var c = (DbConstantExpression)exp;
                return IsNullOrEmpty(c.Value);
            }

            if (exp.NodeType == DbExpressionType.Parameter)
            {
                var p = (DbParameterExpression)exp;
                return IsNullOrEmpty(p.Value);
            }

            return false;
        }

        static bool IsNullOrEmpty(object obj)
        {
            if (obj == null || obj == DBNull.Value)
                return true;

            string stringValue = obj as string;
            if (stringValue != null && stringValue == string.Empty)
                return true;

            return false;
        }
    }
}
