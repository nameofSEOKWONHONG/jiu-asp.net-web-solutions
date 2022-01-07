using System.Data;

namespace Chloe.Infrastructure
{
    public interface IDatabaseProvider
    {
        string DatabaseType { get; }
        IDbConnection CreateConnection();
        IDbExpressionTranslator CreateDbExpressionTranslator();
        string CreateParameterName(string name);
    }
}
