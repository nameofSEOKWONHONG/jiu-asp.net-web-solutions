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