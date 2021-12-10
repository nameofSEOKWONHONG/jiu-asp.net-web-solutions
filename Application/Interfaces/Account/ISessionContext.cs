using System;
using System.Collections.Generic;
using Domain.Entities;
using Domain.Enums;

namespace WebApiApplication.Services.Abstract
{
    public interface ISessionContext
    {
        Guid UserId { get; }
        ENUM_ROLE_TYPE RoleType { get; }
        User User { get; }
        IEnumerable<ENUM_ROLE_PERMISSION_TYPE> RolePermissionTypes { get; }
    }
}