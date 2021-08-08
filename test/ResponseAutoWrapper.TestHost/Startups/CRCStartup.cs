using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ResponseAutoWrapper.TestHost
{
    public class CRCStartup : BaseStartup
    {
        public CRCStartup(IConfiguration configuration) : base(configuration)
        {
        }

        public override void Configure(IApplicationBuilder app)
        {
            app.UseResponseAutoWrapper();

            base.Configure(app);
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            services.AddResponseAutoWrapper<CustomResponse<object>, CustomResponseCreator>();
        }
    }
}