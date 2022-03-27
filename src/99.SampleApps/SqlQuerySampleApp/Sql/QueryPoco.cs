using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using eXtensionSharp;

namespace SqlQuerySample.Sql;

public class QueryPoco
{
    public string FromTable { get; private set; }
    public List<QueryPocoJoin> QueryPocoJoins { get; private set; } = new List<QueryPocoJoin>();
    public List<QueryPocoClause> QueryPocoClauses { get; private set; } = new List<QueryPocoClause>();
    public QueryPoco()
    {
    }

    public QueryPoco From<TTable>()
    {
        FromTable = typeof(TTable).Name;
        return this;
    }

    private string _dummyKey = string.Empty;
    /// <summary>
    /// inner join
    /// </summary>
    /// <param name="expression"></param>
    /// <typeparam name="TTableA"></typeparam>
    /// <typeparam name="TTableB"></typeparam>
    /// <returns></returns>
    public QueryPoco Join<TTableA, TTableB>(Expression<Func<TTableA, TTableB, bool>> expression, string joinType = "inner join")
    {
        dynamic body = expression.Body;
        var first = $"{body.Left.Expression.Name}.{body.Left.Member.Name}";
        var second = $"{body.Right.Expression.Name}.{body.Right.Member.Name}";        
        var op = GetOp(body.NodeType);
        var generateKey = $"{first},{second},{op}".xGetHashCode();
        var exists = QueryPocoJoins.FirstOrDefault(m => m.Key == generateKey);
        if (exists.xIsNotEmpty()) throw new Exception("join sql exists");
        _dummyKey = generateKey;

        QueryPocoJoins.Add(new QueryPocoJoin()
        {
            Key = generateKey,
            JoinType = joinType,
            JoinTableName = typeof(TTableB).Name,
            First = $"{body.Left.Expression.Name}.{body.Left.Member.Name}",
            Second = $"{body.Right.Expression.Name}.{body.Right.Member.Name}",
            Op = op,
            OnItems = new List<QueryPocoClause>()
        });
        return this;
    }

    public QueryPoco On<TTableA, TTableB>(Expression<Func<TTableA, TTableB, bool>> expression)
    {
        dynamic body = expression.Body;
        var first = $"{body.Left.Expression.Name}.{body.Left.Member.Name}";
        var second = $"{body.Right.Expression.Name}.{body.Right.Member.Name}";        
        var op = GetOp(body.NodeType);
        var generateKey = $"{first},{second},{op}".xGetHashCode();
        var parantJoinItem = QueryPocoJoins.First(m => m.Key == _dummyKey);
        if (parantJoinItem.xIsEmpty()) throw new Exception("Join sql not exists");
        var existsClause = parantJoinItem.OnItems.FirstOrDefault(m => m.Key == generateKey);
        if (existsClause.xIsNotEmpty()) throw new Exception("Join clause sql exists");
        
        parantJoinItem.OnItems.Add(new QueryPocoClause()
        {
            First =  $"{body.Left.Expression.Name}.{body.Left.Member.Name}",
            Second = $"{body.Right.Expression.Name}.{body.Right.Member.Name}",
            Op = op
        });
        return this;
    }

    /// <summary>
    /// left join
    /// </summary>
    /// <param name="expression"></param>
    /// <typeparam name="TTableA"></typeparam>
    /// <typeparam name="TTableB"></typeparam>
    /// <returns></returns>
    public QueryPoco LeftJoin<TTableA, TTableB>(Expression<Func<TTableA, TTableB, bool>> expression)
    {
        this.Join(expression, "left join");
        return this;
    }

    public QueryPoco RightJoin<TTableA, TTableB>(Expression<Func<TTableA, TTableB, bool>> expression)
    {
        this.Join(expression, "right join");
        return this;
    }

    public QueryPoco Where<TTable>(Expression<Func<TTable, bool>> expression)
    {
        dynamic body = expression.Body;
        var first = $"{body.Left.Expression.Name}.{body.Left.Member.Name}";
        var second = GetExpressionValue(body.Right.NodeType, body);
        var op = GetOp(body.NodeType);
        var generateKey = $"{first},{second},{op}".xGetHashCode();
        var exists = QueryPocoClauses.FirstOrDefault(m => m.Key == generateKey);
        if (exists.xIsEmpty())
        {
            QueryPocoClauses.Add(new QueryPocoClause()
            {
                First = first,
                Second = second,
                Op = op
            });             
        }
       
        return this;
    }

    public QueryPoco And<TTable>(Expression<Func<TTable, bool>> expression)
    {
        this.Where(expression);
        return this;
    }

    #region [utils]

    public string GetOp(ExpressionType expressionType)
    {
        return expressionType switch
        {
            ExpressionType.Equal => "=",
            ExpressionType.NotEqual => "!=",
            ExpressionType.GreaterThan => ">",
            ExpressionType.GreaterThanOrEqual => ">=",
            ExpressionType.LessThan => "<",
            ExpressionType.LessThanOrEqual => "<=",
            _ => throw new NotImplementedException()
        };
    }

    private object GetExpressionValue(ExpressionType expressionType, dynamic body)
    {
        return expressionType switch
        {
            ExpressionType.MemberAccess => (body.Right.Member as FieldInfo).GetValue(body.Right.Expression.Value),
            ExpressionType.Constant => body.Right.Value,
            _ => throw new NotImplementedException()
        };
    }

    #endregion

}

