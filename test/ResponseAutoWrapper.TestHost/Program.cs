using System;
using System.Linq;
using System.Reflection;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace ResponseAutoWrapper.TestHost;

public class Program
{
    #region Public 方法

    //保留CreateHostBuilder，避免某些情况下编译失败
    public static IHostBuilder CreateHostBuilder(params string[] args) => Hosts.CreateHostBuilder<DefaultStartup>(false, args);

    public static void Main(string[] args)
    {
        var startupName = args.Length > 0
                            ? args[0].Trim()
                            : "DefaultStartup";

        var startupType = Assembly.GetExecutingAssembly()
                                  .GetTypes()
                                  .Where(m => m.Name == startupName)
                                  .FirstOrDefault()!;

        Console.WriteLine($"Running with - {startupType.FullName}");

        var runMethod = typeof(Program).GetMethod("RunWithStartup", BindingFlags.Static | BindingFlags.NonPublic)!;

        runMethod.MakeGenericMethod(startupType)
                 .Invoke(null, new object[] { args });
    }

    #endregion Public 方法

    #region Private 方法

    private static void RunWithStartup<TStartup>(string[] args) where TStartup : BaseStartup
    {
        Hosts.CreateHostBuilder<TStartup>(false, args).Build().Run();
    }

    #endregion Private 方法
}
