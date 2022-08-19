using System.Collections.Generic;
using System.Linq;
using Domain.Entities;
using Domain.Enums;
using eXtensionSharp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using WebApiApplication.Services.Abstract;

namespace WebApiApplication.Filters
{
    public class RoleAuthorize : AuthorizeAttribute, IAuthorizationFilter
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
            var assignedUser = sessionContextService.GetSession();
            var roleMatch = roleTypes.Contains(assignedUser.RoleType.ToString());
            if (!roleMatch)
            {
                context.Result = new UnauthorizedResult();
                return;
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