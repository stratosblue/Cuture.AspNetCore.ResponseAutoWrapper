using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace ResponseAutoWrapper.TestHost;

public class AuthorizeMixedAttribute : AuthorizeAttribute
{
    #region Public 构造函数

    public AuthorizeMixedAttribute()
    {
        AuthenticationSchemes = $"{CookieAuthenticationDefaults.AuthenticationScheme},{JwtBearerDefaults.AuthenticationScheme}";
    }

    public AuthorizeMixedAttribute(string policy) : base(policy)
    {
        AuthenticationSchemes = $"{CookieAuthenticationDefaults.AuthenticationScheme},{JwtBearerDefaults.AuthenticationScheme}";
    }

    #endregion Public 构造函数
}
