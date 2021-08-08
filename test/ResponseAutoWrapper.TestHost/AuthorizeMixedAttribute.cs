using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace ResponseAutoWrapper.TestHost
{
    public class AuthorizeMixedAttribute : AuthorizeAttribute
    {
        public AuthorizeMixedAttribute()
        {
            AuthenticationSchemes = $"{CookieAuthenticationDefaults.AuthenticationScheme},{JwtBearerDefaults.AuthenticationScheme}";
        }

        public AuthorizeMixedAttribute(string policy) : base(policy)
        {
            AuthenticationSchemes = $"{CookieAuthenticationDefaults.AuthenticationScheme},{JwtBearerDefaults.AuthenticationScheme}";
        }
    }
}