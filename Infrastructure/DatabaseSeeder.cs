using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Domain.Entities;
using eXtensionSharp;
using Infrastructure.Context;
using Microsoft.Extensions.Logging;
using WebApiApplication.Services.Abstract;

namespace Infrastructure.Services
{
    public interface IDatabaseSeeder
    {
        void Initialize();
    }
    public class DatabaseSeeder : IDatabaseSeeder
    {
        private readonly ILogger _logger;
        private readonly JIUDbContext _context;
        
        public DatabaseSeeder(ILogger<DatabaseSeeder> logger,
            JIUDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        
        public void Initialize()
        {
            var exists = _context.Users.FirstOrDefault(m => m.Email == "test@example.com");
            if(exists.xIsNotEmpty()) return;
            
            using (var scope = new TransactionScope(TransactionScopeOption.Required))
            {
                var roleClames = new List<RolePermission>()
                {
                    //super, admin, user
                    new RolePermission()
                    {
                        RolePermissionTypes = new List<ENUM_ROLE_PERMISSION_TYPE>()
                        {
                            ENUM_ROLE_PERMISSION_TYPE.VIEW,
                            ENUM_ROLE_PERMISSION_TYPE.CREATE,
                            ENUM_ROLE_PERMISSION_TYPE.UPDATE,
                            ENUM_ROLE_PERMISSION_TYPE.DELETE,
                        },
                        WriteDt = DateTime.Now,
                        WriteId = "system"
                    },
                    //guest
                    new RolePermission()
                    {
                        RolePermissionTypes = new List<ENUM_ROLE_PERMISSION_TYPE>()
                        {
                            ENUM_ROLE_PERMISSION_TYPE.VIEW
                        },
                        WriteDt = DateTime.Now,
                        WriteId = "system"
                    },
                };
            
                roleClames.xForEach(item =>
                {
                    _context.RolePermissions.Add(item);
                });
                _context.SaveChanges();
                
                var superRole = new Role()
                {
                    RoleType = ENUM_ROLE_TYPE.SUPER,
                    RolePermission = roleClames[0],
                    WriteDt = DateTime.Now,
                    WriteId = "system"
                };
                _context.Roles.Add(superRole);
                _context.SaveChanges();
                
                var adminRole = new Role()
                {
                    RoleType = ENUM_ROLE_TYPE.ADMIN,
                    RolePermission = roleClames[0],
                    WriteDt = DateTime.Now,
                    WriteId = "system"
                };
                _context.Roles.Add(adminRole);
                _context.SaveChanges();

                var userRole = new Role()
                {
                    RoleType = ENUM_ROLE_TYPE.USER,
                    RolePermission = roleClames[0],
                    WriteDt = DateTime.Now,
                    WriteId = "system"
                };
                _context.Roles.Add(userRole);
                _context.SaveChanges();

                var guestRole = new Role()
                {
                    RoleType = ENUM_ROLE_TYPE.GUEST,
                    RolePermission = roleClames[1],
                    WriteDt = DateTime.Now,
                    WriteId = "system"
                };
                _context.Roles.Add(guestRole);
                _context.SaveChanges();
            
                var superUser = new User()
                {
                    ActivateUser = true,
                    Email = "test@example.com",
                    Password = "test",
                    WriteDt = DateTime.Now,
                    WriteId = "system",
                    Role = superRole
                };
                superUser.Password = BCrypt.Net.BCrypt.HashPassword(superUser.Password);
                _context.Users.Add(superUser);
                _context.SaveChanges();

                scope.Complete();
            }
        }
    }
}