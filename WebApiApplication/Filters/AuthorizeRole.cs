using System.Collections.Generic;
using System.Linq;
using Domain.Entities;
using eXtensionSharp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using WebApiApplication.Services.Abstract;

namespace WebApiApplication.Filters
{
    public class AuthorizeRole : AuthorizeAttribute, IAuthorizationFilter
    {
        /// <summary>
        /// enable split
        /// </summary>
        public string RoleType { get; set; } = ENUM_ROLE_TYPE.USER.ToString();
        /// <summary>
        /// enable split
        /// </summary>
        public string PermissionType { get; set; } = ENUM_ROLE_PERMISSION_TYPE.VIEW.ToString();

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var sessionContextService =
                context.HttpContext.RequestServices.GetRequiredService<ISessionContextService>();
            if (RoleType == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var roleTypes = RoleType.xSplit(',');
            var assignedUser = sessionContextService.GetSessionAsync().GetAwaiter().GetResult();
            var roleMatch = roleTypes.Contains(assignedUser.RoleType.ToString());
            if (!roleMatch)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            if (assignedUser.RoleType == ENUM_ROLE_TYPE.SUPER ||
                assignedUser.RoleType == ENUM_ROLE_TYPE.ADMIN)
            {
                //role_permission 조회로 변경되어야 함.
                var permissions = new[]
                {
                    ENUM_ROLE_PERMISSION_TYPE.CREATE,
                    ENUM_ROLE_PERMISSION_TYPE.DELETE,
                    ENUM_ROLE_PERMISSION_TYPE.UPDATE,
                    ENUM_ROLE_PERMISSION_TYPE.VIEW
                };

                var exists = assignedUser.RolePermissionTypes.xContains(permissions);
                if(!exists)
                {
                    context.Result = new UnauthorizedResult();
                    return;
                }
            }

            if (PermissionType.xIsNotEmpty())
            {
                var permissionItems = PermissionType.xSplit(',');
                var permissions = new List<ENUM_ROLE_PERMISSION_TYPE>();
                permissionItems.xForEach(x => permissions.Add(ENUM_ROLE_PERMISSION_TYPE.Parse(x)));
                var exists = assignedUser.RolePermissionTypes.xContains(permissions);
                if (!exists)
                {
                    context.Result = new UnauthorizedResult();
                    return;
                }
            }
        }
    }
}