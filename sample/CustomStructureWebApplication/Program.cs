using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace CustomStructureWebApplication;

public class Program
{
    #region Public 方法

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });

    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    #endregion Public 方法
}
