using System;
using System.Collections.Generic;
using System.Transactions;
using System.Linq;
using Domain.Entities;
using Domain.Entities.System.Config;
using Domain.Enums;
using eXtensionSharp;
using Microsoft.Extensions.Logging;

namespace Application.Context
{
    public interface IDatabaseSeeder
    {
        void Initialize();
    }
    
    /// <summary>
    /// database seeder
    /// </summary>
    public class DatabaseSeeder : IDatabaseSeeder
    {
        private readonly ILogger _logger;
        private readonly DatabaseMigration _migration;
        private readonly ApplicationDbContext _dbContext;
        public DatabaseSeeder(ILogger<DatabaseSeeder> logger,
            DatabaseMigration migration,
            ApplicationDbContext dbContext)
        {
            _logger = logger;
            _migration = migration;
            _dbContext = dbContext;
        }
        
        public void Initialize()
        {
            #region [migration setting]

            _migration.Migration().GetAwaiter().GetResult();
            
            #endregion

            #region [init data]

            var superUserExists = _dbContext.Users.FirstOrDefault(m => m.EMAIL == "test@example.com");
            if(superUserExists.xIsNotEmpty()) return;
            
            using (var scope = new TransactionScope(TransactionScopeOption.Required))
            {
                var roleClames = new List<TB_ROLE_PERMISSION>()
                {
                    //super, admin, user
                    new TB_ROLE_PERMISSION()
                    {
                        ROLE_PERMISSION_TYPES = new List<ENUM_ROLE_PERMISSION_TYPE>()
                        {
                            ENUM_ROLE_PERMISSION_TYPE.VIEW,
                            ENUM_ROLE_PERMISSION_TYPE.CREATE,
                            ENUM_ROLE_PERMISSION_TYPE.UPDATE,
                            ENUM_ROLE_PERMISSION_TYPE.DELETE,
                        },
                        WRITE_DT = DateTime.Now,
                        WRITE_ID = "system"
                    },
                    //guest
                    new TB_ROLE_PERMISSION()
                    {
                        ROLE_PERMISSION_TYPES = new List<ENUM_ROLE_PERMISSION_TYPE>()
                        {
                            ENUM_ROLE_PERMISSION_TYPE.VIEW
                        },
                        WRITE_DT = DateTime.Now,
                        WRITE_ID = "system"
                    },
                };
            
                roleClames.xForEach(item =>
                {
                    _dbContext.RolePermissions.Add(item);
                });
                _dbContext.SaveChanges();
                
                var superRole = new TB_ROLE()
                {
                    ROLE_TYPE = ENUM_ROLE_TYPE.SUPER,
                    ROLE_PERMISSION = roleClames[0],
                    WRITE_DT = DateTime.Now,
                    WRITE_ID = "system"
                };
                _dbContext.Roles.Add(superRole);
                _dbContext.SaveChanges();
                
                var adminRole = new TB_ROLE()
                {
                    ROLE_TYPE = ENUM_ROLE_TYPE.ADMIN,
                    ROLE_PERMISSION = roleClames[0],
                    WRITE_DT = DateTime.Now,
                    WRITE_ID = "system"
                };
                _dbContext.Roles.Add(adminRole);
                _dbContext.SaveChanges();

                var userRole = new TB_ROLE()
                {
                    ROLE_TYPE = ENUM_ROLE_TYPE.USER,
                    ROLE_PERMISSION = roleClames[0],
                    WRITE_DT = DateTime.Now,
                    WRITE_ID = "system"
                };
                _dbContext.Roles.Add(userRole);
                _dbContext.SaveChanges();

                var guestRole = new TB_ROLE()
                {
                    ROLE_TYPE = ENUM_ROLE_TYPE.GUEST,
                    ROLE_PERMISSION = roleClames[1],
                    WRITE_DT = DateTime.Now,
                    WRITE_ID = "system"
                };
                _dbContext.Roles.Add(guestRole);
                _dbContext.SaveChanges();
            
                var superUser = new TB_USER()
                {
                    ACTIVE_USER_YN = true,
                    EMAIL = "test@example.com",
                    PASSWORD = "test",
                    WRITE_DT = DateTime.Now,
                    WRITE_ID = "system",
                    ROLE = superRole
                };
                superUser.PASSWORD = BCrypt.Net.BCrypt.HashPassword(superUser.PASSWORD);
                _dbContext.Users.Add(superUser);
                _dbContext.SaveChanges();

                scope.Complete();
            }            

            #endregion
        }
    }

    #region [chloe sample]
    // [Obsolete("no use", true)]
    // public class DatabaseSeederUseChloe : IDatabaseSeeder
    // {
    //     private readonly ApplicationDbContext _dbContext;
    //     public DatabaseSeederUseChloe(ApplicationDbContext dbContext)
    //     {
    //         _dbContext = dbContext;
    //     }
    //     
    //     public void Initialize()
    //     {
    //         var migrationExists = _dbContext.UseChloeDbContext().Query<TB_MIGRAION>()
    //             .Where(m => m.MIGRATION_YN == true && m.COMPLETE_YN == false).FirstOrDefault();
    //         migrationExists.xIfNotEmpty(() =>
    //         {
    //             migrationExists.UPDATE_DT = DateTime.UtcNow;
    //             migrationExists.UPDATE_ID = "SYSTEM";
    //             migrationExists.COMPLETE_YN = true;
    //             _dbContext.UseChloeDbContext().Update(migrationExists);
    //         });
    //     }
    // }
    #endregion
    
}