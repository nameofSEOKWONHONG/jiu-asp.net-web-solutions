using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Domain.Enums;
using eXtensionSharp;

namespace Application.Context
{
    public class JIUDbContext : DbContext
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
        public JIUDbContext(DbConnection connection)
        {
            this.connection = connection;
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
            modelBuilder.Entity<Role>()
                .Property(e => e.RoleType)
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
            
            modelBuilder.Entity<RolePermission>()
                .Property(e => e.RolePermissionTypes)
                .HasConversion(
                    v => string.Join(',', v),
                    v => func(v));
        }

        #region [account]

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }

        #endregion

        #region [biz]

        public DbSet<Todo> Todos { get; set; }
        

        #endregion
    }
}