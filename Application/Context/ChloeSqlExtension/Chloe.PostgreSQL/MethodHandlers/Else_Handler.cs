using Chloe.DbExpressions;
using Chloe.RDBMS;
using static Chloe.DbExpressions.DbCaseWhenExpression;

namespace Chloe.PostgreSQL.MethodHandlers
{
    class Else_Handler : IMethodHandler
    {
        public bool CanProcess(DbMethodCallExpression exp)
        {
            return exp.Method.DeclaringType.IsGenericType && exp.Method.DeclaringType.GetGenericTypeDefinition() == typeof(Then<>);
        }
        public void Process(DbMethodCallExpression exp, SqlGeneratorBase generator)
        {
            List<WhenThenExpressionPair> pairs = new List<WhenThenExpressionPair>();
            GetWhenThenPairs(exp.Object as DbMethodCallExpression, pairs);

            pairs.Reverse();
            List<WhenThenExpressionPair> whenThenPairs = pairs;
            DbExpression elseExp = exp.Arguments[0];
            DbCaseWhenExpression caseWhenExp = new DbCaseWhenExpression(exp.Type, whenThenPairs, elseExp);

            caseWhenExp.Accept(generator);
        }

        void GetWhenThenPairs(DbMethodCallExpression thenCall, List<WhenThenExpressionPair> pairs)
        {
            DbMethodCallExpression whenCall = thenCall.Object as DbMethodCallExpression;

            var thenExp = thenCall.Arguments[0];
            var conditionExp = whenCall.Arguments[0];
            WhenThenExpressionPair pair = new WhenThenExpressionPair(conditionExp, thenExp);
            pairs.Add(pair);
            if (whenCall.Object == null)
            {
                return;
            }

            GetWhenThenPairs(whenCall.Object as DbMethodCallExpression, pairs);
        }
    }
}
