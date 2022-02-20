using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;
using System.Threading.Tasks;

using Cuture.AspNetCore.ResponseAutoWrapper;
using Cuture.Http;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ResponseAutoWrapper.TestHost;

namespace ResponseAutoWrapper.Test;

//Copy from CRITests
[TestClass]
public class CRCTest : TestServerBase
{
    #region Public 属性

    public virtual string UriPrefix { get; set; } = "/api/CRCWeatherForecast";

    #endregion Public 属性

    #region Public 方法

    [TestMethod]
    [DataRow("/get?type=1")]
    [DataRow("/get?type=2")]
    [DataRow("/get-inheritedtask?type=1")]
    [DataRow("/get-inheritedtask?type=2")]
    [DataRow("/get-inheritedtask2?type=1")]
    [DataRow("/get-inheritedtask2?type=2")]
    [DataRow("/get-task?type=1")]
    [DataRow("/get-task?type=2")]
    [DataRow("/get-valuetask?type=1")]
    [DataRow("/get-valuetask?type=2")]
    public async Task Should_CustomDescription(string requestPath)
    {
        var response = await Client.GetFromJsonAsync<CustomResponse<WeatherForecast[]>>(CombineUri(requestPath));

        CheckResponseCode(response, 10086);

        Assert.AreEqual("Hello world!", response.Info);

        Debug.WriteLine(response.Info);
    }

    [TestMethod]
    [DataRow("/get")]
    [DataRow("/get-inheritedtask")]
    [DataRow("/get-inheritedtask2")]
    [DataRow("/get-direct-api-response")]
    [DataRow("/get-direct-inherited-api-response")]
    [DataRow("/get-dynamic?resultType=0")]
    [DataRow("/get-dynamic?resultType=1")]
    [DataRow("/get-dynamic?resultType=2")]
    [DataRow("/get-dynamic-task?resultType=0")]
    [DataRow("/get-dynamic-task?resultType=1")]
    [DataRow("/get-dynamic-task?resultType=2")]
    [DataRow("/get-dynamic-valuetask?resultType=0")]
    [DataRow("/get-dynamic-valuetask?resultType=1")]
    [DataRow("/get-dynamic-valuetask?resultType=2")]
    [DataRow("/get-task")]
    [DataRow("/get-task-direct-api-response")]
    [DataRow("/get-valuetask")]
    [DataRow("/get-valuetask-direct-api-response")]
    [DataRow("/get-with-param")]
    [DataRow("/get-with-param-required?count=5")]
    [DataRow("/get-with-param-required-limit?count=5")]
    public async Task Should_Wrapped200(string requestPath)
    {
        var response = await Client.GetFromJsonAsync<CustomResponse<WeatherForecast[]>>(CombineUri(requestPath));

        CheckResponseCode(response);
        CheckWeatherForecast(response.Datas);
    }

    [TestMethod]
    [DataRow("/get-authorize")]
    public async Task Should_Wrapped200ThroughAuthorize(string requestPath)
    {
        var response = await Client.GetFromJsonAsync<CustomResponse<WeatherForecast[]>>(CombineUri(requestPath));

        CheckResponseCode(response, 401);

        Assert.IsNull(response.Datas);

        Debug.WriteLine($"No authentication Message: {response.Info}");

        #region Cookie

        var cookie = await LoginAsync(false, true);

        response = await Client.CreateRequest(CombineUri(requestPath))
                               .UseCookie(cookie)
                               .GetAsObjectAsync<CustomResponse<WeatherForecast[]>>();

        CheckResponseCode(response, 403);

        Debug.WriteLine($"Cookie can not access Message: {response.Info}");

        cookie = await LoginAsync(true, true);

        response = await Client.CreateRequest(CombineUri(requestPath))
                               .UseCookie(cookie)
                               .GetAsObjectAsync<CustomResponse<WeatherForecast[]>>();

        CheckResponseCode(response);
        CheckWeatherForecast(response.Datas);

        #endregion Cookie

        #region Jwt

        var token = await LoginAsync(false, false, "datas");

        response = await Client.CreateRequest(CombineUri(requestPath))
                               .UseBearerToken(token)
                               .GetAsObjectAsync<CustomResponse<WeatherForecast[]>>();

        CheckResponseCode(response, 403);

        Debug.WriteLine($"Jwt can not access Message: {response.Info}");

        token = await LoginAsync(true, false, "datas");

        response = await Client.CreateRequest(CombineUri(requestPath))
                               .AddHeader("Authorization", $"Bearer {token}")
                               .GetAsObjectAsync<CustomResponse<WeatherForecast[]>>();

        CheckResponseCode(response);
        CheckWeatherForecast(response.Datas);

        #endregion Jwt
    }

    [TestMethod]
    [DataRow("/get-with-param-required")]
    [DataRow("/get-with-param-required-limit?count=0")]
    [DataRow("/get-with-param-required-limit?count=11")]
    public async Task Should_Wrapped400(string requestPath)
    {
        var response = await Client.GetFromJsonAsync<CustomResponse<WeatherForecast[]>>(CombineUri(requestPath));

        CheckResponseCode(response, 400);

        Debug.WriteLine(response.Info);
    }

    [TestMethod]
    [DataRow("/get-authentication")]
    [DataRow("/get-authorize")]
    public async Task Should_Wrapped401(string requestPath)
    {
        var response = await Client.GetFromJsonAsync<CustomResponse<WeatherForecast[]>>(CombineUri(requestPath));

        CheckResponseCode(response, 401);

        Assert.IsNull(response.Datas);

        Debug.WriteLine(response.Info);
    }

    [TestMethod]
    [DataRow("/get-exception-throw")]
    public async Task Should_Wrapped500(string requestPath)
    {
        var response = await Client.GetFromJsonAsync<CustomResponse<WeatherForecast[]>>(CombineUri(requestPath));

        CheckResponseCode(response, 500);

        Debug.WriteLine(response.Info);
    }

    [TestMethod]
    [DataRow("/get-nowrap")]
    public async Task ShouldReturnNotWrapped(string requestPath)
    {
        var response = await Client.GetFromJsonAsync<WeatherForecast[]>(CombineUri(requestPath));

        CheckWeatherForecast(response);
    }

    #endregion Public 方法

    #region Protected 方法

    protected static void CheckResponseCode([NotNull] CustomResponse<WeatherForecast[]>? apiResponse, int code = StatusCodes.Status200OK)
    {
        Assert.IsNotNull(apiResponse);
        Assert.AreEqual(code, apiResponse.StatusCode);
    }

    protected string CombineUri(string path) => $"{UriPrefix}{path}";

    protected override async Task<IHostBuilder> CreateServerHostBuilderAsync()
    {
        var builder = await CreateServerHost();
        builder.ConfigureServices(services => services.AddResponseAutoWrapper<CustomResponse<object>, CustomResponseCreator>(GetOptionsSetupAction()));
        return builder;
    }

    protected virtual Action<ResponseAutoWrapperOptions>? GetOptionsSetupAction()
    {
        return options => { };
    }

    #endregion Protected 方法
}

[TestClass]
public class CRCTest_DisableOpenAPISupport : CRCTest
{
    #region Protected 方法

    protected override Action<ResponseAutoWrapperOptions>? GetOptionsSetupAction()
    {
        return options => options.DisableOpenAPISupport = true;
    }

    #endregion Protected 方法
}

[TestClass]
public class CRCTest_NotGeneric : CRCTest
{
    #region Public 属性

    public override string UriPrefix { get; set; } = "/api/NGCRCWeatherForecast";

    #endregion Public 属性

    #region Protected 方法

    protected override Task<IHostBuilder> CreateServerHostBuilderAsync() => CreateServerHostWithStartup<NotGenericCRCStartup>();

    #endregion Protected 方法
}
