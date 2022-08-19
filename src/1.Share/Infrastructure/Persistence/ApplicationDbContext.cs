using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Application.Context;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Domain.Entities.KeyValueStore;
using Domain.Entities.Storage;
using Domain.Entities.System.Config;
using Domain.Enums;
using eXtensionSharp;

namespace Infrastructure.Persistence
{
    public sealed class ApplicationDbContext : DbContextBase
    {   
        /// <summary>
        /// init appsettings connection
        /// 수동 설정 부분은 제거함.
        /// OnConfiguring 삭제함. 기본 설정 방법으로만 사용함.
        /// </summary>
        /// <param name="options"></param>
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
            if (this.Database != null)
            {
                
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // get all composite keys (entity decorated by more than 1 [Key] attribute
            foreach (var entity in modelBuilder.Model.GetEntityTypes()
                         .Where(t => 
                             t.ClrType.GetProperties()
                                 .Count(p => p.CustomAttributes.Any(a => a.AttributeType == typeof(KeyAttribute))) > 1))
            {
                // get the keys in the appropriate order
                var orderedKeys = entity.ClrType
                    .GetProperties()
                    .Where(p => p.CustomAttributes.Any(a => a.AttributeType == typeof(KeyAttribute)))
                    .OrderBy(p => 
                        p.CustomAttributes.Single(x => x.AttributeType == typeof(ColumnAttribute))?
                            .NamedArguments?.Single(y => y.MemberName == nameof(ColumnAttribute.Order))
                            .TypedValue.Value ?? 0)
                    .Select(x => x.Name)
                    .ToArray();

                // apply the keys to the model builder
                modelBuilder.Entity(entity.ClrType).HasKey(orderedKeys);
            }
            
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

        #region [kv store]

        public DbSet<TB_KV_STORE> KvStores { get; set; }
        public DbSet<TB_GRP_KV_STORE> GrpKvStores { get; set; }

        #endregion
        
        #region [storage]

        public DbSet<TB_STORAGE> Storages { get; set; }

        #endregion

    }
}