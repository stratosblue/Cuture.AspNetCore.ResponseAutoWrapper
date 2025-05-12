using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace ResponseAutoWrapper.TestHost;

public class EmptyStartup(IConfiguration configuration) : BaseStartup(configuration)
{

    #region Public 方法

    public override void Configure(IApplicationBuilder app)
    {
        app.UseResponseAutoWrapper();

        base.Configure(app);
    }

    #endregion Public 方法
}
