using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ResponseAutoWrapper.TestHost;

public class CustomResponseByResponseCreatorMiddlewareExceptionStartup : BaseStartup
{
    public CustomResponseByResponseCreatorMiddlewareExceptionStartup(IConfiguration configuration) : base(configuration)
    {
    }

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

        services.AddResponseAutoWrapper<LegacyCustomResponse<object>, int, string>()
                .ConfigureWrappers(builder => builder.AddLegacyWrappers<LegacyCustomResponseWrapper>());
    }
}
