using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace SimpleWebApplication;

public class Startup
{
    #region Public 属性

    public IConfiguration Configuration { get; }

    #endregion Public 属性

    #region Public 构造函数

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    #endregion Public 构造函数

    #region Public 方法

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        //捕获异常、非200状态码的请求，包装响应
        app.UseResponseAutoWrapper(options =>
        {
            //配置
            //options.CatchExceptions 是否捕获异常
            //options.ThrowCaughtExceptions 捕获到异常处理结束后，是否再将异常抛出
            //options.DefaultOutputFormatterSelector 默认输出格式化器选择委托，选择在请求中无 Accept 时，用于格式化响应的 IOutputFormatter
        });

        if (env.IsDevelopment())
        {
            //app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SimpleWebApplication v1"));
        }

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "SimpleWebApplication", Version = "v1" });
        });

        //自动包装ActionResult
        services.AddResponseAutoWrapper(options =>
        {
            //配置
            //options.ActionNoWrapPredicate     //Action的筛选委托，默认会过滤掉标记了NoResponseWrapAttribute的方法
            //options.DisableOpenAPISupport     //禁用OpenAPI支持，Swagger将不会显示包装后的格式，也会解除响应类型必须为object泛型的限制
            //options.HandleAuthorizationResult     //处理授权结果（可能无效，需要自行测试）
            //options.HandleInvalidModelState       //处理无效模型状态
            //options.RewriteStatusCode = null;     //包装时不覆写非200的HTTP状态码
        });
    }

    #endregion Public 方法
}
