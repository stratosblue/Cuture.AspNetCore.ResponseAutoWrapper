using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ResponseAutoWrapper.TestHost;

public class NotGenericCRStartup(IConfiguration configuration) : BaseStartup(configuration)
{

    #region Public 方法

    public override void Configure(IApplicationBuilder app)
    {
        app.UseResponseAutoWrapper();

        base.Configure(app);
    }

    public override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);

        services.AddResponseAutoWrapper<CustomResponse, ResponseCode, ResponseMessage>(options => options.DisableOpenAPISupport = true)
                .ConfigureWrappers(builder => builder.AddWrappers<NotGenericCustomResponseWrapper>());
    }

    #endregion Public 方法
}
