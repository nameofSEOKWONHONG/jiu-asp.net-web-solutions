using Chloe.Core;
using Chloe.DbExpressions;

namespace Chloe.Infrastructure
{
    public interface IDbExpressionTranslator
    {
        DbCommandInfo Translate(DbExpression expression);
    }
}
