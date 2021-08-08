using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ResponseAutoWrapper.TestHost.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        [HttpGet]
        [Route("jwt")]
        public string Jwt([FromQuery] bool canAccess)
        {
            var claims = new[]
            {
                new Claim("CanAccess",canAccess?"1":"0"),
            };
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("123456789123456789"));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var jwtToken = new JwtSecurityToken("Issuer", "Audience", claims, expires: DateTime.Now.AddMinutes(600), signingCredentials: credentials);

            var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            return token;
        }

        [HttpGet]
        [Route("cookie")]
        public async Task CookieAsync([FromQuery] bool canAccess)
        {
            var claimsIdentity = new ClaimsIdentity(new[]
            {
                new Claim("CanAccess",canAccess?"1":"0"),
            }, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
        }
    }
}
