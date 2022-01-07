using Chloe.DbExpressions;
using Chloe.Descriptors;
using Chloe.Extensions;
using System.Linq.Expressions;

namespace Chloe.Core.Visitors
{
    public class DefaultExpressionParser : ExpressionVisitorBase
    {
        TypeDescriptor _typeDescriptor;
        DbTable _explicitDbTable;

        public DefaultExpressionParser(TypeDescriptor typeDescriptor, DbTable explicitDbTable)
        {
            this._typeDescriptor = typeDescriptor;
            this._explicitDbTable = explicitDbTable;
        }

        public DbExpression ParseFilterPredicate(LambdaExpression filterPredicate)
        {
            return this.Visit(BooleanResultExpressionTransformer.Transform(filterPredicate));
        }
        public DbExpression Parse(Expression exp)
        {
            return this.Visit(exp);
        }

        protected override DbExpression VisitMemberAccess(MemberExpression exp)
        {
            if (ExpressionExtension.IsDerivedFromParameter(exp))
            {
                Stack<MemberExpression> reversedExps = ExpressionExtension.Reverse(exp);

                DbExpression dbExp = null;
                bool first = true;
                foreach (var me in reversedExps)
                {
                    if (first)
                    {
                        DbColumnAccessExpression dbColumnAccessExpression = this._typeDescriptor.GetColumnAccessExpression(me.Member);
                        if (this._explicitDbTable != null && this._explicitDbTable != dbColumnAccessExpression.Table)
                            dbColumnAccessExpression = new DbColumnAccessExpression(this._explicitDbTable, dbColumnAccessExpression.Column);

                        dbExp = dbColumnAccessExpression;
                        first = false;
                    }
                    else
                    {
                        DbMemberExpression dbMe = new DbMemberExpression(me.Member, dbExp);
                        dbExp = dbMe;
                    }
                }

                if (dbExp != null)
                {
                    return dbExp;
                }
                else
                    throw new Exception();
            }
            else
            {
                return base.VisitMemberAccess(exp);
            }
        }

        protected override DbExpression VisitParameter(ParameterExpression exp)
        {
            throw new NotSupportedException(exp.ToString());
        }
    }
}
