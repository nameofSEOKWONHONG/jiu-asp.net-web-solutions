// See https://aka.ms/new-console-template for more information

using Microsoft.Data.SqlClient;
using SqlKata;
using SqlKata.Compilers;
using SqlKata.Execution;
using SqlQuerySample.Sql;
using SqlQuerySampleApp.SampleEntity;

var q = new QueryPoco();
var id = 2;
var name = "test";
var queryPoco = q.From<TB_USER>()
    .Join<TB_USER, TB_PHONE>((TB_USER, TB_PHONE) => TB_USER.ID == TB_PHONE.USER_ID && TB_USER.NAME == TB_PHONE.NUMER)
        // .On<TB_USER, TB_PHONE>((TB_USER, TB_PHONE) => TB_USER.ID == TB_PHONE.USER_ID)
        // .On<TB_USER, TB_PHONE>((TB_USER, TB_PHONE) => TB_USER.ID == TB_PHONE.USER_ID)
    .Join<TB_USER, TB_GRADE>((TB_USER, TB_GRADE) => TB_USER.ID == TB_GRADE.USER_ID)
        // .On<TB_USER, TB_GRADE>((TB_USER, TB_GRADE) => TB_USER.ID == TB_GRADE.USER_ID)
    //.Where<TB_USER>((TB_USER, o) => TB_USER.ID == 1)
    .Where<TB_USER>((TB_USER) => TB_USER.ID == id)
    .And<TB_USER>((TB_USER) => TB_USER.NAME == name);
var builder = new QueryPocoBuilder(queryPoco);
    
Console.WriteLine(builder.Build());

var compiler = new SqlServerCompiler();
var connection = new SqlConnection("Data Source=Demo.db");
var queryBuilder = new QueryFactory(connection, compiler);
var sql2 = new Query("TB_USER").Join("TB_PHONE", "TB_USER.PHONE_ID", "TB_PHONE.ID", "=")
    .Where("TB_USER.ID", "=", id);
//sql2.AsUpdate();
//sql2.AsInsert();
//sql2.AsDelete();
//sql2.Get<TB_USER>();
//sql2.AsInsert()
var compiled = compiler.Compile(sql2);
Console.WriteLine(compiled.ToString());
