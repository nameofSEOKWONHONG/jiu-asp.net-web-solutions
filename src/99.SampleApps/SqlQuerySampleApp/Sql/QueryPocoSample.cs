using eXtensionSharp;
using Microsoft.Data.SqlClient;
using SqlKata;
using SqlKata.Compilers;
using SqlKata.Execution;
using SqlQuerySampleApp.SampleEntity;

namespace SqlQuerySample.Sql;

public class QueryPocoSample
{
    public void Run()
    {
        #region [impl1]

        var q = new QueryPoco();
        var id = 2;
        var name = "test";
        var queryPoco = q.From<TB_USER>(s => new
            {
                s.ID,
                s.NAME
            })
            .Join<TB_USER, TB_PHONE>((TB_USER, TB_PHONE) =>
                TB_USER.ID == TB_PHONE.USER_ID && TB_USER.NAME == TB_PHONE.NUMER)
            .Join<TB_USER, TB_GRADE>((TB_USER, TB_GRADE) => TB_USER.ID == TB_GRADE.USER_ID)
            .Where<TB_USER>((TB_USER) => TB_USER.ID == id)
            .And<TB_USER>((TB_USER) => TB_USER.NAME == name);

        var builder = new QueryPocoBuilder(queryPoco);
        var buildResult = builder.Build();
        Console.WriteLine(buildResult.sql);

        #endregion

        #region [impl2]

        var query = new QueryPocoV2<TB_USER, TB_PHONE>()
            .From((a, b) => new
            {
                a.ID,
                a.NAME,
                b.NUMER,
                T_ID = "TEST"
            }).Join((a, b) => a.ID == b.USER_ID)
            .Where((a, b) => a.ID == 10);
            
        

        #endregion


        var compiler = new SqlServerCompiler();
        var connection = new SqlConnection("Data Source=Demo.db");
        var queryBuilder = new QueryFactory(connection, compiler);
        var sql2 = new Query("TB_USER").Join("TB_PHONE", "TB_USER.PHONE_ID", "TB_PHONE.ID", "=")
            .Where("TB_USER.ID", "=", 1);
//sql2.AsUpdate();
//sql2.AsInsert();
//sql2.AsDelete();
//sql2.Get<TB_USER>();
//sql2.AsInsert()
        var compiled = compiler.Compile(sql2);
        var user = queryBuilder.Query("TB_USER").Get<TB_USER>();
        Console.WriteLine(compiled.ToString());
        var bindings = compiled.Bindings;
        if (bindings.xIsNotEmpty())
        {
            
        }
    }
}