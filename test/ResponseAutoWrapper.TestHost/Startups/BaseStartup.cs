using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace ResponseAutoWrapper.TestHost;

public abstract class BaseStartup(IConfiguration configuration)
{
    #region Public 属性

    public IConfiguration Configuration { get; } = configuration;

    #endregion Public 属性

    #region Public 方法

    public virtual void Configure(IApplicationBuilder app)
    {
        app.UseSwagger();

        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ResponseAutoWrapper.TestHost v1"));

        app.UseRouting();

        app.UseAuthentication();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }

    public virtual void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "ResponseAutoWrapper.TestHost", Version = "v1" });
        });

        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    //覆盖默认的认证相关操作，改为修改状态码
                    options.Events.OnRedirectToAccessDenied = ctx => { ctx.HttpContext.Response.StatusCode = StatusCodes.Status403Forbidden; return Task.CompletedTask; };
                    options.Events.OnRedirectToLogin = ctx => { ctx.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized; return Task.CompletedTask; };
                    options.Events.OnRedirectToLogout = _ => Task.CompletedTask;
                    options.Events.OnRedirectToReturnUrl = _ => Task.CompletedTask;
                })
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    //覆盖默认的认证相关操作，改为修改状态码
                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = ctx => { ctx.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized; return Task.CompletedTask; },
                        OnChallenge = ctx => { ctx.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized; return Task.CompletedTask; },
                        OnForbidden = ctx => { ctx.HttpContext.Response.StatusCode = StatusCodes.Status403Forbidden; return Task.CompletedTask; }
                    };

                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("123456789123456789_123456789123456789")),
                        ValidIssuer = "Issuer",
                        ValidAudience = "Audience",
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("CanAccess", p => p.RequireClaim("CanAccess", "1"));
        });
    }

    #endregion Public 方法
}
