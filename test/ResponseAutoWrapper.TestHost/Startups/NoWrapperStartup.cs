using Microsoft.Extensions.Configuration;

namespace ResponseAutoWrapper.TestHost;

public class NoWrapperStartup : BaseStartup
{
    public NoWrapperStartup(IConfiguration configuration) : base(configuration)
    {
    }
}
