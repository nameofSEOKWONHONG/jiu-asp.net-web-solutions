using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Domain.Entities.System.Config;
using Domain.Enums;
using eXtensionSharp;
using SqlKata.Compilers;
using SqlKata.Execution;
using SqlSugar;

namespace Application.Context
{
    public class DbContextBase : DbContext
    {
        private readonly DbConnection _dbConnection;
        /// <summary>
        /// SqlSugarClient, Entity 명칭이 Table명칭과 100% 일치할때만 사용가능함.
        /// 즉, Table("TB_TODO") 이런 Attribute로 재정의 될 경우 사용 불가...
        /// 이외에 Entity에 의존하는 다른 ORM을 사용하려면 TableAttribute를 사용하지 않는 것이 좋을 듯 함.
        /// SqlSugar는 중국에서 만들어지 Entity기반 Sql ORM이고 쿼리 문장을 완전히 지원함.
        /// 즉, Queryable에 대한 생성이 Linq가 아닌 쿼리 문장을 코드로 만들고 해당 내역을 바탕으로 쿼리 문장을 생성하는 구조임.
        /// 물론, Linq도 마찮가지이지만 Linq의 단점은 해당식이 복잡할 경우 해석하기 난해하고 Sql 문장 중심이 아니므로
        /// Sql과 이질감이 드는 부분이 있고 문장 표현력도 제한적임.
        /// SqlKata와 비슷하지만 SqlKata는 Entity가 아닌 사용자 문장에 의존하는 반면,
        /// SqlSugar는 100% Entity 중심으로 생성할 수 있는 것으로 보임.
        /// SqlSugar는 기본 무료, 유료 지원에 20$임.
        /// Entity 중심으로 코딩하길 원한다면 유용한 선택지임. 단, 중국이라서 못 사용하겠다면 다른 선택지인 SqlKata가 있다.
        /// </summary>
        private readonly SqlSugarClient _sqlSugarClient;

        public DbContextBase(DbContextOptions options) : base(options)
        {
            this._sqlSugarClient = new SqlSugarClient(new ConnectionConfig()
            {
                DbType = DbType.SqlServer,
                ConnectionString = this.Database.GetConnectionString(),
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true,
                //Master-Slave 옵션을 지원함. SlaveConnectionConfigs
                AopEvents = new AopEvents
                {
                    OnLogExecuting = (sql, p) =>
                    {
                        Console.WriteLine(sql);
                        Console.WriteLine(string.Join(",", p?.Select(it => it.ParameterName + ":" + it.Value)));
                    }
                }
            });
        }

        public DbContextBase(DbConnection connection)
        {
            this._dbConnection = connection;
            this._sqlSugarClient = new SqlSugarClient(new ConnectionConfig()
            {
                DbType = DbType.SqlServer,
                ConnectionString = this._dbConnection.ConnectionString,
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true,
                AopEvents = new AopEvents
                {
                    OnLogExecuting = (sql, p) =>
                    {
                        Console.WriteLine(sql);
                        Console.WriteLine(string.Join(",", p?.Select(it => it.ParameterName + ":" + it.Value)));
                    }
                }
            });
        }

        public SqlSugarClient GetSqlSugarClient()
        {
            if (_sqlSugarClient.xIsNotEmpty())
                return _sqlSugarClient;

            throw new NotSupportedException("database not init.");
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
        public QueryFactory GetSqlKataQueryFactory()
        {
            var con = this.Database.GetDbConnection();
            var compiler = new SqlServerCompiler();
            return new QueryFactory(con, compiler);
        }
    }
    public sealed class JIUDbContext : DbContextBase
    {
        private readonly DbConnection connection;
        
        /// <summary>
        /// init appsettings connection
        /// </summary>
        /// <param name="options"></param>
        public JIUDbContext(DbContextOptions options) : base(options)
        {

        }

        /// <summary>
        /// init manual connection
        /// </summary>
        /// <param name="connection"></param>
        public JIUDbContext(DbConnection connection) : base(connection)
        {
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //base.OnConfiguring(optionsBuilder);
            if (this.connection != null)
            {
                optionsBuilder.UseSqlServer(this.connection, options =>
                {
                    options.EnableRetryOnFailure();
                    // options.ExecutionStrategy(dependencies =>
                    // {
                    //     dependencies.
                    // })
                });
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TB_ROLE>()
                .Property(e => e.ROLE_TYPE)
                .HasConversion(
                    v => v.ToString(),
                    v => XEnumBase<ENUM_ROLE_TYPE>.Parse(v, true));

            Func<string, List<ENUM_ROLE_PERMISSION_TYPE>> func;
            func = (v) =>
            {
                var items = v.Split(',', StringSplitOptions.RemoveEmptyEntries);
                var result = new List<ENUM_ROLE_PERMISSION_TYPE>();
                items.xForEach(item =>
                {
                    result.Add(XEnumBase<ENUM_ROLE_PERMISSION_TYPE>.Parse(item));
                });
                return result;
            };
            
            modelBuilder.Entity<TB_ROLE_PERMISSION>()
                .Property(e => e.ROLE_PERMISSION_TYPES)
                .HasConversion(
                    v => string.Join(',', v),
                    v => func(v));
        }

        #region [account]

        public DbSet<TB_USER> Users { get; set; }
        public DbSet<TB_ROLE> Roles { get; set; }
        public DbSet<TB_ROLE_PERMISSION> RolePermissions { get; set; }

        #endregion

        #region [biz]

        public DbSet<TB_TODO> Todos { get; set; }
        

        #endregion

        #region [config]
        public DbSet<TB_MIGRAION> Migrations { get; set; }
        #endregion
    }
}