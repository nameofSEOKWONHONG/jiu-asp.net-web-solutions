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
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IRoleClaimService _roleClaimService;
        
        public DatabaseSeeder(ILogger<DatabaseSeeder> logger,
            JIUDbContext context,
            IUserService userService,
            IRoleService roleService,
            IRoleClaimService roleClaimService)
        {
            _logger = logger;
            _context = context;
            _userService = userService;
            _roleService = roleService;
            _roleClaimService = roleClaimService;
        }
        
        public void Initialize()
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required))
            {
                var roleClames = new List<RoleClaim>()
                {
                    //super, admin, user
                    new RoleClaim()
                    {
                        RoleClaimTypes = new List<ENUM_ROLE_CLAIM_TYPE>()
                        {
                            ENUM_ROLE_CLAIM_TYPE.V,
                            ENUM_ROLE_CLAIM_TYPE.C,
                            ENUM_ROLE_CLAIM_TYPE.U,
                            ENUM_ROLE_CLAIM_TYPE.D,
                        },
                        WriteDt = DateTime.Now,
                        WriteId = "system"
                    },
                    //guest
                    new RoleClaim()
                    {
                        RoleClaimTypes = new List<ENUM_ROLE_CLAIM_TYPE>()
                        {
                            ENUM_ROLE_CLAIM_TYPE.V
                        },
                        WriteDt = DateTime.Now,
                        WriteId = "system"
                    },
                };
            
                roleClames.xForEach(item =>
                {
                    _context.RoleClaims.Add(item);
                });
                _context.SaveChanges();
                
                var superRole = new Role()
                {
                    RoleType = ENUM_ROLE_TYPE.SUPER,
                    RoleClaim = roleClames[0],
                    WriteDt = DateTime.Now,
                    WriteId = "system"
                };
                _context.Roles.Add(superRole);
                _context.SaveChanges();
                
                var adminRole = new Role()
                {
                    RoleType = ENUM_ROLE_TYPE.ADMIN,
                    RoleClaim = roleClames[0],
                    WriteDt = DateTime.Now,
                    WriteId = "system"
                };
                _context.Roles.Add(adminRole);
                _context.SaveChanges();

                var userRole = new Role()
                {
                    RoleType = ENUM_ROLE_TYPE.USER,
                    RoleClaim = roleClames[0],
                    WriteDt = DateTime.Now,
                    WriteId = "system"
                };
                _context.Roles.Add(userRole);
                _context.SaveChanges();

                var guestRole = new Role()
                {
                    RoleType = ENUM_ROLE_TYPE.GUEST,
                    RoleClaim = roleClames[1],
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
                    UserRole = superRole
                };
                _context.Users.Add(superUser);
                _context.SaveChanges();

                scope.Complete();
            }
        }
    }
}