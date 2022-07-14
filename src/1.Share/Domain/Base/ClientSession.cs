using InjectionExtension;

namespace Domain.Base;

public interface IContextBase
{
    IUserInfo UserInfo { get; set; }
    IRoleInfo RoleInfo { get; set; }
    IApplicationInfo ApplicationInfo { get; set; }
    IAuthorizeInfo AuthorizeInfo { get; set; }
}

[AddService(ENUM_LIFE_TIME_TYPE.Singleton, typeof(IContextBase))]
public class ContextBase : IContextBase
{
    public string SessionCode { get; set; }
    public IUserInfo UserInfo { get; set; }
    public IRoleInfo RoleInfo { get; set; }
    public IApplicationInfo ApplicationInfo { get; set; }
    public IAuthorizeInfo AuthorizeInfo { get; set; }

    public ContextBase()
    {
        SessionCode = "";
        UserInfo = new UserInfo();
        RoleInfo = new RoleInfo();
        ApplicationInfo = new ApplicationInfo();
        AuthorizeInfo = new AuthorizeInfo();
    }
}

public class UserInfo : IUserInfo
{
    public string UserId { get; set; }
    public string UserName { get; set; }

    public UserInfo()
    {
        
    }
}

public class RoleInfo : IRoleInfo
{
    public bool IsAdmin { get; set; }
    public bool HasDelete { get; set; }
    public bool HasModify { get; set; }
    public bool HasInsert { get; set; }
    public bool HasSearch { get; set; }
}

public class ApplicationInfo : IApplicationInfo
{
    
}

public class AuthorizeInfo : IAuthorizeInfo
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}

public interface IUserInfo
{
    string UserId { get; set; }
    string UserName { get; set; }
}

public interface IRoleInfo
{
    bool IsAdmin { get; set; }
    bool HasDelete { get; set; }
    bool HasModify { get; set; }
    bool HasInsert { get; set; }
    bool HasSearch { get; set; }
}

public interface IApplicationInfo
{
    
}

public interface IAuthorizeInfo
{
    string AccessToken { get; set; }
    string RefreshToken { get; set; }
}