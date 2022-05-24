using System.Net.Http.Json;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ResponseAutoWrapper.TestHost;

namespace ResponseAutoWrapper.Test;

[TestClass]
public class MiddlewareExceptionCustomResponseByResponseCreatorTest : TestServerBase
{
    #region Public 方法

    [TestMethod]
    [DataRow("/api/LCRWeatherForecast/get")]
    public async Task Should_Wrapped500(string requestPath)
    {
        var response = await Client.GetFromJsonAsync<LegacyCustomResponse<WeatherForecast[]>>(requestPath);

        Assert.AreEqual(500, response.StatusCode);
    }

    #endregion Public 方法

    #region Protected 方法

    protected override Task<IHostBuilder> CreateServerHostBuilderAsync() => CreateServerHostWithStartup<CustomResponseByResponseCreatorMiddlewareExceptionStartup>();

    #endregion Protected 方法
}

[TestClass]
public class MiddlewareExceptionTest : TestServerBase
{
    #region Public 方法

    [TestMethod]
    [DataRow("/api/WeatherForecast/get")]
    public async Task Should_Wrapped500(string requestPath)
    {
        var response = await Client.GetFromJsonAsync<ApiResponse<WeatherForecast[]>>(requestPath);

        Assert.AreEqual(500, response.Code);
    }

    #endregion Public 方法

    #region Protected 方法

    protected override Task<IHostBuilder> CreateServerHostBuilderAsync() => CreateServerHostWithStartup<MiddlewareExceptionStartup>();

    #endregion Protected 方法
}
