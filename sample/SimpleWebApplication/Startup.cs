using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace SimpleWebApplication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

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
                //options.ActionNoWrapPredicate Action的筛选委托，默认会过滤掉标记了NoResponseWrapAttribute的方法
                //options.DisableOpenAPISupport 禁用OpenAPI支持，Swagger将不会显示包装后的格式，也会解除响应类型必须为object泛型的限制
                //options.HandleAuthorizationResult 处理授权结果（可能无效，需要自行测试）
                //options.HandleInvalidModelState 处理无效模型状态
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //捕获异常、非200状态码的请求，包装响应
            app.UseResponseAutoWrapper(options =>
            {
                //配置
                //options.CatchExceptions 是否捕获异常
                //options.ThrowCaughtExceptions 捕获到异常处理结束后，是否再将异常抛出
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
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
    }
}
