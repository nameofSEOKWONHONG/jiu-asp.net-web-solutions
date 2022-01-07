using System.Linq.Expressions;
using System.Reflection;

namespace Chloe.Query.QueryExpressions
{
    class IncludeExpression : QueryExpression
    {
        public IncludeExpression(Type elementType, QueryExpression prevExpression, NavigationNode navigationNode)
           : base(QueryExpressionType.Include, elementType, prevExpression)
        {
            this.NavigationNode = navigationNode;
        }
        public NavigationNode NavigationNode { get; private set; }

        public override T Accept<T>(QueryExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

    public class NavigationNode
    {
        public NavigationNode()
        {
        }
        public NavigationNode(PropertyInfo property)
        {
            this.Property = property;
        }
        public PropertyInfo Property { get; set; }
        public LambdaExpression Condition { get; set; }
        public List<LambdaExpression> ContextFilters { get; private set; } = new List<LambdaExpression>();
        public NavigationNode Next { get; set; }

        public NavigationNode Clone()
        {
            NavigationNode current = new NavigationNode(this.Property) { Condition = this.Condition };
            current.ContextFilters.AddRange(this.ContextFilters);
            if (this.Next != null)
            {
                current.Next = this.Next.Clone();
            }

            return current;
        }

        public NavigationNode GetLast()
        {
            if (this.Next == null)
                return this;

            return this.Next.GetLast();
        }

        public override string ToString()
        {
            if (this.Next == null)
                return this.Property.Name;

            return $"{this.Property.Name}.{this.Next.ToString()}";
        }
    }
}
