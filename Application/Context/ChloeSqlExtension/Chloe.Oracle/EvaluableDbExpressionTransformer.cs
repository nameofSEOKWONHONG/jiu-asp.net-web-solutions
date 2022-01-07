using Chloe.Core.Visitors;
using Chloe.DbExpressions;
using Chloe.RDBMS;
using System.Reflection;

namespace Chloe.Oracle
{
    class EvaluableDbExpressionTransformer : EvaluableDbExpressionTransformerBase
    {
        static EvaluableDbExpressionTransformer _transformer = new EvaluableDbExpressionTransformer();

        static HashSet<MemberInfo> _toTranslateMembers = new HashSet<MemberInfo>();
        static EvaluableDbExpressionTransformer()
        {
            _toTranslateMembers.Add(PublicConstants.PropertyInfo_String_Length);

            _toTranslateMembers.Add(PublicConstants.PropertyInfo_DateTime_Now);
            _toTranslateMembers.Add(PublicConstants.PropertyInfo_DateTime_UtcNow);
            _toTranslateMembers.Add(PublicConstants.PropertyInfo_DateTime_Today);
            _toTranslateMembers.Add(PublicConstants.PropertyInfo_DateTime_Date);

            _toTranslateMembers.Add(PublicConstants.PropertyInfo_DateTime_Year);
            _toTranslateMembers.Add(PublicConstants.PropertyInfo_DateTime_Month);
            _toTranslateMembers.Add(PublicConstants.PropertyInfo_DateTime_Day);
            _toTranslateMembers.Add(PublicConstants.PropertyInfo_DateTime_Hour);
            _toTranslateMembers.Add(PublicConstants.PropertyInfo_DateTime_Minute);
            _toTranslateMembers.Add(PublicConstants.PropertyInfo_DateTime_Second);
            _toTranslateMembers.Add(PublicConstants.PropertyInfo_DateTime_Millisecond);
            _toTranslateMembers.Add(PublicConstants.PropertyInfo_DateTime_DayOfWeek);

            _toTranslateMembers.Add(OracleSemantics.PropertyInfo_ROWNUM);

            _toTranslateMembers.TrimExcess();
        }

        public static DbExpression Transform(DbExpression exp)
        {
            return exp.Accept(_transformer);
        }

        public override bool CanTranslateToSql(DbMemberExpression exp)
        {
            return _toTranslateMembers.Contains(exp.Member);
        }
        public override bool CanTranslateToSql(DbMethodCallExpression exp)
        {
            IMethodHandler methodHandler;
            if (SqlGenerator.MethodHandlers.TryGetValue(exp.Method.Name, out methodHandler))
            {
                if (methodHandler.CanProcess(exp))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
