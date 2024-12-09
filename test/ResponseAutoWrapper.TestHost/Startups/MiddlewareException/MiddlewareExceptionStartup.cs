﻿using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ResponseAutoWrapper.TestHost;

public class MiddlewareExceptionStartup : BaseStartup
{
    #region Public 构造函数

    public MiddlewareExceptionStartup(IConfiguration configuration) : base(configuration)
    {
    }

    #endregion Public 构造函数

    #region Public 方法

    public override void Configure(IApplicationBuilder app)
    {
        app.UseResponseAutoWrapper();

        app.Use(async (context, next) =>
        {
            throw new Exception("Middleware throw Exception");
            await next();
        });

        base.Configure(app);
    }

    public override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);

        services.AddResponseAutoWrapper();
    }

    #endregion Public 方法
}
