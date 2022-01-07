using System.Reflection;

namespace Chloe.DbExpressions
{
    public class DbModuloExpression : DbBinaryExpression
    {
        public DbModuloExpression(Type type, DbExpression left, DbExpression right)
            : this(type, left, right, null)
        {

        }
        public DbModuloExpression(Type type, DbExpression left, DbExpression right, MethodInfo method)
            : base(DbExpressionType.Modulo, type, left, right, method)
        {

        }

        public override T Accept<T>(DbExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
