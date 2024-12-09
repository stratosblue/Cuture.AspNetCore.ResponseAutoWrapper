using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;

namespace ResponseAutoWrapper.TestHost;

public static class Hosts
{
    #region Public 方法

    public static IHostBuilder CreateHostBuilder(bool useTestServer, params string[] args)
    {
        return Host.CreateDefaultBuilder(args)
                              .ConfigureWebHostDefaults(webBuilder =>
                              {
                                  if (useTestServer)
                                  {
                                      webBuilder.UseTestServer();
                                  }

                                  webBuilder.UseStartup<EmptyStartup>();
                              });
    }

    public static IHostBuilder CreateHostBuilder<TStartup>(bool useTestServer, params string[] args) where TStartup : BaseStartup
    {
        return Host.CreateDefaultBuilder(args)
                   .ConfigureWebHostDefaults(webBuilder =>
                   {
                       if (useTestServer)
                       {
                           webBuilder.UseTestServer();
                       }

                       webBuilder.UseStartup<TStartup>();
                   });
    }

    #endregion Public 方法
}
