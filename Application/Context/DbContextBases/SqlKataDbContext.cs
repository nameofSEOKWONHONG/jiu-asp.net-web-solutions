using System.Data;
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using Npgsql;
using SqlKata.Compilers;
using SqlKata.Execution;

namespace Application.Context;

/// <summary>
/// TODO : 리졸버 만들어야 함. 아니면 SQLKATA와 CHLOE를 묶어야 함. SqlSugar는 그대로 사용.
/// <see href="https://github.com/sqlkata/querybuilder"></see>
/// </summary>
public class SqlKataDbContext
{
    private readonly string _connection;
    private readonly ENUM_DATABASE_TYPE _databaseType;
    public SqlKataDbContext(string connection, ENUM_DATABASE_TYPE type)
    {
        _connection = connection;
        _databaseType = type;
    }
    
    /// <summary>
    /// SqlKata와 ORM등을 사용할 경우의 장점은 ORM 프레임워크가 지원하는 DB에 따라 하나의 소스로
    /// DB를 변경할 경우 모두 동작할 수 있다는 점이다. 물론 개발자가 Sql을 작성하지 않아도 되는 점은 말하지 않아도 장점이겠지만...
    /// 다만, Sql과 비교할 경우 쿼리에 대한 작성이 동적으로 진행되는 점, 즉, 해당 쿼리 생성에 시간이 소요되고 복잡할 수록 ORM으로 작성하기 어려워지지만,
    /// 단, SqlKata와 SqlSuggar는 모든 쿼리 문자에 대한 지원이 있으므로 다소나마 작성하기 수월하다.
    /// 또한, 해당 쿼리 생성기를 가지고 프로시저를 작성하도록 할 수도 있고, 물론 캐시도 포함되어야 하지만...
    /// 본인의 솔루션이 다양한 DB환경에서 동작해야 한다면 고려해볼만 가치가 있다.
    /// </summary>
    /// <returns></returns>
    public (IDbConnection connection, QueryFactory queryFactory) GetSqlKataQueryFactory()
    {
        Compiler compiler = null;
        //이렇게 한 이유??? Singleton으로 사용할꺼니까~
        IDbConnection connection = null;
        
        if (_databaseType == ENUM_DATABASE_TYPE.MSSQL) connection = new SqlConnection(_connection);
        else if (_databaseType == ENUM_DATABASE_TYPE.MYSQL) connection = new MySqlConnection(_connection);
        else if (_databaseType == ENUM_DATABASE_TYPE.POSTGRES) connection = new NpgsqlConnection(_connection);
        else throw new NotImplementedException();
        
        if(_databaseType == ENUM_DATABASE_TYPE.MSSQL) compiler = new SqlServerCompiler();
        else if (_databaseType == ENUM_DATABASE_TYPE.MYSQL) compiler = new MySqlCompiler();
        else if (_databaseType == ENUM_DATABASE_TYPE.POSTGRES) compiler = new PostgresCompiler();
        else throw new NotImplementedException();
        
        return new (connection, new QueryFactory(connection, compiler));
    }
}