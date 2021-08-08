using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace ResponseAutoWrapper.TestHost
{
    public class EmptyStartup : BaseStartup
    {
        public EmptyStartup(IConfiguration configuration) : base(configuration)
        {
        }

        public override void Configure(IApplicationBuilder app)
        {
            app.UseResponseAutoWrapper();

            base.Configure(app);
        }
    }
}