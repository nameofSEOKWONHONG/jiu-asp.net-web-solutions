using System.Collections.ObjectModel;
using System.Reflection;

namespace Chloe.DbExpressions
{
    public class DbAggregateExpression : DbExpression
    {
        MethodInfo _method;
        ReadOnlyCollection<DbExpression> _arguments;
        public DbAggregateExpression(Type type, MethodInfo method, IList<DbExpression> arguments)
            : base(DbExpressionType.Aggregate, type)
        {
            this._method = method;
            this._arguments = new ReadOnlyCollection<DbExpression>(arguments);
        }

        public MethodInfo Method { get { return this._method; } }
        public ReadOnlyCollection<DbExpression> Arguments { get { return this._arguments; } }

        public override T Accept<T>(DbExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
