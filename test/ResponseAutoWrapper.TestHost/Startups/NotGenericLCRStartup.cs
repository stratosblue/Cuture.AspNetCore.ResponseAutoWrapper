﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ResponseAutoWrapper.TestHost;

public class NotGenericLCRStartup : BaseStartup
{
    #region Public 构造函数

    public NotGenericLCRStartup(IConfiguration configuration) : base(configuration)
    {
    }

    #endregion Public 构造函数

    #region Public 方法

    public override void Configure(IApplicationBuilder app)
    {
        app.UseResponseAutoWrapper();

        base.Configure(app);
    }

    public override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);

        services.AddResponseAutoWrapper<LegacyCustomResponse, int, string>(options => options.DisableOpenAPISupport = true)
                .ConfigureWrappers(builder => builder.AddLegacyWrappers<NotGenericLegacyCustomResponseWrapper>());
    }

    #endregion Public 方法
}
