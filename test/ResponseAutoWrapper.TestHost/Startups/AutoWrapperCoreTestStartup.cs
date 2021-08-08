using AutoWrapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace ResponseAutoWrapper.TestHost
{
    //Test for package AutoWrapper.Core

    public class AutoWrapperCoreTestStartup : BaseStartup
    {
        public AutoWrapperCoreTestStartup(IConfiguration configuration) : base(configuration)
        {
        }

        public override void Configure(IApplicationBuilder app)
        {
            app.UseApiResponseAndExceptionWrapper();
            base.Configure(app);
        }
    }
}