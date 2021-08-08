
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;

namespace ResponseAutoWrapper.TestHost
{
    public static class Hosts
    {
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
    }
}