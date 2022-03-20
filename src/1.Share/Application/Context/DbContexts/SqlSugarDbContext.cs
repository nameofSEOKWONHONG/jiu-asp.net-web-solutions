// using System;
// using System.Linq;
// using eXtensionSharp;
// using SqlSugar;
//
// namespace Application.Context;
//
// public class SqlSugarDbContext
// {
//     /// <summary>
//     /// SqlSugarClient, Entity 명칭이 Table명칭과 100% 일치할때만 사용가능함.
//     /// 즉, Table("TB_TODO") 이런 Attribute로 재정의 될 경우 사용 불가...
//     /// 이외에 Entity에 의존하는 다른 ORM을 사용하려면 TableAttribute를 사용하지 않는 것이 좋을 듯 함.
//     /// SqlSugar는 중국에서 만들어지 Entity기반 Sql ORM이고 쿼리 문장을 완전히 지원함.
//     /// 즉, Queryable에 대한 생성이 Linq가 아닌 쿼리 문장을 코드로 만들고 해당 내역을 바탕으로 쿼리 문장을 생성하는 구조임.
//     /// 물론, Linq도 마찮가지이지만 Linq의 단점은 해당식이 복잡할 경우 해석하기 난해하고 Sql 문장 중심이 아니므로
//     /// Sql과 이질감이 드는 부분이 있고 문장 표현력도 제한적임.
//     /// SqlKata와 비슷하지만 SqlKata는 Entity가 아닌 사용자 문장에 의존하는 반면,
//     /// SqlSugar는 100% Entity 중심으로 생성할 수 있는 것으로 보임.
//     /// SqlSugar는 기본 무료, 유료 지원에 20$임.
//     /// Entity 중심으로 코딩하길 원한다면 유용한 선택지임. 단, 중국이라서 못 사용하겠다면 다른 선택지인 SqlKata가 있다.
//     /// </summary>
//     private readonly SqlSugarClient _sqlSugarClient;
//
//     public SqlSugarDbContext(string connection, DbType type)
//     {
//         this._sqlSugarClient = new SqlSugarClient(new ConnectionConfig()
//         {
//             DbType = type,
//             ConnectionString = connection,
//             InitKeyType = InitKeyType.Attribute,
//             IsAutoCloseConnection = true,
//             //Master-Slave 옵션을 지원함. SlaveConnectionConfigs
//             AopEvents = new AopEvents
//             {
//                 OnLogExecuting = (sql, p) =>
//                 {
//                     Console.WriteLine(sql);
//                     Console.WriteLine(string.Join(",", p?.Select(it => it.ParameterName + ":" + it.Value)));
//                 }
//             }
//         });
//     }
//     
//     public SqlSugarClient GetSqlSugarClient()
//     {
//         if (_sqlSugarClient.xIsNotEmpty())
//             return _sqlSugarClient;
//
//         throw new NotSupportedException("database not init.");
//     }
// }