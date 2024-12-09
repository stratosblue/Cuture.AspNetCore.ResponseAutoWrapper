using Microsoft.Extensions.Configuration;

namespace ResponseAutoWrapper.TestHost;

public class NoWrapperStartup : BaseStartup
{
    #region Public 构造函数

    public NoWrapperStartup(IConfiguration configuration) : base(configuration)
    {
    }

    #endregion Public 构造函数
}
