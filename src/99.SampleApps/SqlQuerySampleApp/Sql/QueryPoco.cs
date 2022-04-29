using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using eXtensionSharp;

namespace SqlQuerySample.Sql;

public class QueryPoco
{
    public List<string> Selectors { get; private set; } = new List<string>();
    public string FromTable { get; private set; }
    public List<QueryPocoJoinClause> QueryPocoJoins { get; private set; } = new List<QueryPocoJoinClause>();
    public List<QueryPocoWhereClause> QueryPocoClauses { get; private set; } = new List<QueryPocoWhereClause>();

    public QueryPoco()
    {
    }

    public QueryPoco From<TEntity>(Expression<Func<TEntity, dynamic>> selector)
    {
        if (selector.Body.NodeType == ExpressionType.New)
        {
            var members = (selector.Body as NewExpression).Members;
            members.xForEach(member =>
            {
                Selectors.Add($"{typeof(TEntity).Name}.{member.Name}");
            });
        }
        
        FromTable = typeof(TEntity).Name;
        return this;
    }

    private string _dummyKey = string.Empty;
    /// <summary>
    /// inner join
    /// </summary>
    /// <param name="expression"></param>
    /// <typeparam name="FromEntity"></typeparam>
    /// <typeparam name="ToEntity"></typeparam>
    /// <returns></returns>
    public QueryPoco Join<FromEntity, ToEntity>(Expression<Func<FromEntity, ToEntity, bool>> expression, string joinType = "inner join")
    {
        var body = expression.Body as BinaryExpression;
        var expressionText = body.ToString();
        var items = expressionText.xSplit(' ').ToList();
        var joinWhereClauseItems = new List<QueryPocoWhereClause>();
        for (var i = 0; i < items.Count; i++)
        {
            var joinWhereClauseItem = new QueryPocoWhereClause();
            joinWhereClauseItem.First = items[i].Replace("(", "").Replace(")", "");
            i += 1;
            joinWhereClauseItem.Op = GetOp(items[i].Replace("(", "").Replace(")", ""));
            i += 1;
            joinWhereClauseItem.Second = items[i].Replace("(", "").Replace(")", "");
            i += 1;
            if (i >= items.Count)
            {
                joinWhereClauseItem.Key =
                    $"{joinWhereClauseItem.First},{joinWhereClauseItem.Op},{joinWhereClauseItem.Second},{joinWhereClauseItem.NextOp}"
                        .xGetHashCode();
                joinWhereClauseItems.Add(joinWhereClauseItem);
                break;
            }
            joinWhereClauseItem.NextOp = items[i].Replace("(", "").Replace(")", "");
            joinWhereClauseItem.Key = 
                $"{joinWhereClauseItem.First},{joinWhereClauseItem.Op},{joinWhereClauseItem.Second},{joinWhereClauseItem.NextOp}"
                .xGetHashCode();
            joinWhereClauseItems.Add(joinWhereClauseItem);
        }

        var joinKey = $"{joinType},{typeof(ToEntity).Name}".xGetHashCode();
        var exists = QueryPocoJoins.FirstOrDefault(m => m.Key == joinKey);
        if (exists.xIsNotEmpty()) throw new Exception("join sql exists");
        _dummyKey = joinKey;

        QueryPocoJoins.Add(new QueryPocoJoinClause()
        {
            Key = joinKey,
            JoinType = joinType,
            JoinTableName = typeof(ToEntity).Name,
            JoinWhereItems = joinWhereClauseItems
        });
        return this;
    }

    // public QueryPoco On<TTableA, TTableB>(Expression<Func<TTableA, TTableB, bool>> expression)
    // {
    //     dynamic body = expression.Body;
    //     var first = $"{body.Left.Expression.Name}.{body.Left.Member.Name}";
    //     var second = $"{body.Right.Expression.Name}.{body.Right.Member.Name}";        
    //     var op = GetOp(body.NodeType);
    //     var generateKey = $"{first},{second},{op}".xGetHashCode();
    //     var parantJoinItem = QueryPocoJoins.First(m => m.Key == _dummyKey);
    //     if (parantJoinItem.xIsEmpty()) throw new Exception("Join sql not exists");
    //     var existsClause = parantJoinItem.OnItems.FirstOrDefault(m => m.Key == generateKey);
    //     if (existsClause.xIsNotEmpty()) throw new Exception("Join clause sql exists");
    //     
    //     parantJoinItem.OnItems.Add(new QueryPocoClause()
    //     {
    //         First =  $"{body.Left.Expression.Name}.{body.Left.Member.Name}",
    //         Second = $"{body.Right.Expression.Name}.{body.Right.Member.Name}",
    //         Op = op
    //     });
    //     return this;
    // }

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

    public QueryPoco Where<Entity>(Expression<Func<Entity, bool>> expression)
    {
        dynamic body = expression.Body;
        var first = $"{body.Left.Expression.Name}.{body.Left.Member.Name}";
        var secondValue = GetExpressionValue(body.Right.NodeType, body);
        var op = GetOp(body.NodeType);
        var generateKey = $"{first},{secondValue},{op}".xGetHashCode();
        var exists = QueryPocoClauses.FirstOrDefault(m => m.Key == generateKey);
        if (exists.xIsEmpty())
        {
            QueryPocoClauses.Add(new QueryPocoWhereClause()
            {
                First = first,
                SecondValue = secondValue,
                Op = op
            });             
        }
       
        return this;
    }

    public QueryPoco And<Entity>(Expression<Func<Entity, bool>> expression)
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

    public string GetOp(string text)
    {
        return text switch
        {
            "==" => "=",
            _ => text
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

