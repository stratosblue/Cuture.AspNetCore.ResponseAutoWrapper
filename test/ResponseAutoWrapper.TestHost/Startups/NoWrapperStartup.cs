using Microsoft.Extensions.Configuration;

namespace ResponseAutoWrapper.TestHost;

public class NoWrapperStartup(IConfiguration configuration) : BaseStartup(configuration)
{
}
