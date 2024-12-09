using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace ResponseAutoWrapper.TestHost;

public class EmptyStartup : BaseStartup
{
    #region Public 构造函数

    public EmptyStartup(IConfiguration configuration) : base(configuration)
    {
    }

    #endregion Public 构造函数

    #region Public 方法

    public override void Configure(IApplicationBuilder app)
    {
        app.UseResponseAutoWrapper();

        base.Configure(app);
    }

    #endregion Public 方法
}
