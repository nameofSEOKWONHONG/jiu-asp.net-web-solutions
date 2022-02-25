using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Application.Infrastructure.Configuration;
using Microsoft.AspNetCore.Http;
using Domain.Entities;
using Domain.Enums;
using eXtensionSharp;
using Microsoft.Extensions.DependencyInjection;
using WebApiApplication.Services.Abstract;

namespace Infrastructure.Services.Account
{
    public class SessionContext : ISessionContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public SessionContext(IHttpContextAccessor httpContextAccessor, 
            Guid userId, 
            ENUM_ROLE_TYPE roleType, 
            IEnumerable<ENUM_ROLE_PERMISSION_TYPE> rolePermissionTypes,
            FileSetting fileSetting)
        {
            UserId = userId;
            RoleType = roleType;
            RolePermissionTypes = rolePermissionTypes;
            AllowFileSetting = fileSetting;
            _httpContextAccessor = httpContextAccessor;
        }
        
        public Guid UserId { get; }
        public ENUM_ROLE_TYPE RoleType { get; }
        public IEnumerable<ENUM_ROLE_PERMISSION_TYPE> RolePermissionTypes { get; }
        public FileSetting AllowFileSetting { get; }

        private TB_USER _tbUser;
        public TB_USER TbUser
        {
            get
            {
                if (_tbUser.xIsEmpty())
                {
                    var svc = _httpContextAccessor.HttpContext.RequestServices.GetService<IUserService>();
                    _tbUser = svc.FindUserByIdAsync(this.UserId).GetAwaiter().GetResult();    
                }
                
                return _tbUser;
            }
        }
    }

    public class SessionContextService : ISessionContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly FileSetting _fileSetting;
        public SessionContextService(IHttpContextAccessor httpContextAccessor, FileSetting fileSetting)
        {
            this._httpContextAccessor = httpContextAccessor;
            this._fileSetting = fileSetting;
        }
        
        public async Task<ISessionContext> GetSessionAsync()
        {
            var userId = this._httpContextAccessor?.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = this._httpContextAccessor?.HttpContext?.User?.FindFirstValue(ClaimsIdentity.DefaultRoleClaimType);
            var permission = this._httpContextAccessor?.HttpContext?.User?.FindFirstValue(role);
            var permissions = permission.xSplit(',').Select(m => ENUM_ROLE_PERMISSION_TYPE.Parse(m));
            
            return new SessionContext(this._httpContextAccessor, 
                Guid.Parse(userId), 
                ENUM_ROLE_TYPE.Parse(role), 
                permissions,
                _fileSetting);
        }
    }
}