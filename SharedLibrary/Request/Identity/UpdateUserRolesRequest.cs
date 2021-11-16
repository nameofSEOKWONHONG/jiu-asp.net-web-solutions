using System.Collections.Generic;
using SharedLibrary.Dtos;

namespace SharedLibrary.Request.Identity
{
    public class UpdateUserRolesRequest
    {
        public string UserId { get; set; }
        public IList<UserRoleModel> UserRoles { get; set; }
    }
}