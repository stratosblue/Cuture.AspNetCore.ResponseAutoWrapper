using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;
using System.Threading.Tasks;

using Cuture.AspNetCore.ResponseAutoWrapper;
using Cuture.Http;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ResponseAutoWrapper.TestHost;

namespace ResponseAutoWrapper.Test;

// Copy from LCRTest

[TestClass]
public class CRTest : TestServerBase
{
    #region Public 属性

    public virtual string UriPrefix { get; set; } = "/api/CRWeatherForecast";

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

        CheckResponseCode(response, new(ResponseState.Success, 10086));

        Assert.AreEqual("Hello world!", response.Message?.Text);

        Debug.WriteLine(response.Message);
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

        CheckResponseCode(response, ResponseCode.Success);
        CheckWeatherForecast(response.Data);
    }

    [TestMethod]
    [DataRow("/get-authorize")]
    public async Task Should_Wrapped200ThroughAuthorize(string requestPath)
    {
        var response = await Client.GetFromJsonAsync<CustomResponse<WeatherForecast[]>>(CombineUri(requestPath));

        CheckResponseCode(response, new(ResponseState.Error, 25000));

        Assert.IsNull(response.Data);

        Debug.WriteLine($"No authentication Message: {response.Message.Text}");

        #region Cookie

        var cookie = await LoginAsync(false, true);

        response = await Client.CreateRequest(CombineUri(requestPath))
                               .UseCookie(cookie)
                               .GetAsObjectAsync<CustomResponse<WeatherForecast[]>>();

        CheckResponseCode(response, new(ResponseState.Error, 25000));

        Debug.WriteLine($"Cookie can not access Message: {response.Message}");

        cookie = await LoginAsync(true, true);

        response = await Client.CreateRequest(CombineUri(requestPath))
                               .UseCookie(cookie)
                               .GetAsObjectAsync<CustomResponse<WeatherForecast[]>>();

        CheckResponseCode(response, ResponseCode.Success);
        CheckWeatherForecast(response.Data);

        #endregion Cookie

        #region Jwt

        var token = await LoginAsync(false, false, "data");

        response = await Client.CreateRequest(CombineUri(requestPath))
                               .UseBearerToken(token)
                               .GetAsObjectAsync<CustomResponse<WeatherForecast[]>>();

        CheckResponseCode(response, new(ResponseState.Error, 25000));

        Debug.WriteLine($"Jwt can not access Message: {response.Message}");

        token = await LoginAsync(true, false, "data");

        response = await Client.CreateRequest(CombineUri(requestPath))
                               .AddHeader("Authorization", $"Bearer {token}")
                               .GetAsObjectAsync<CustomResponse<WeatherForecast[]>>();

        CheckResponseCode(response, ResponseCode.Success);
        CheckWeatherForecast(response.Data);

        #endregion Jwt
    }

    [TestMethod]
    [DataRow("/get-with-param-required")]
    [DataRow("/get-with-param-required-limit?count=0")]
    [DataRow("/get-with-param-required-limit?count=11")]
    public async Task Should_Wrapped400(string requestPath)
    {
        var response = await Client.GetFromJsonAsync<CustomResponse<WeatherForecast[]>>(CombineUri(requestPath));

        CheckResponseCode(response, new(ResponseState.Error, 20000));

        Debug.WriteLine(response.Message);
    }

    [TestMethod]
    [DataRow("/get-authentication")]
    [DataRow("/get-authorize")]
    public async Task Should_Wrapped401(string requestPath)
    {
        var response = await Client.GetFromJsonAsync<CustomResponse<WeatherForecast[]>>(CombineUri(requestPath));

        CheckResponseCode(response, new(ResponseState.Error, 25000));

        Assert.IsNull(response.Data);

        Debug.WriteLine(response.Message);
    }

    [TestMethod]
    [DataRow("/get-exception-throw")]
    public async Task Should_Wrapped500(string requestPath)
    {
        var response = await Client.GetFromJsonAsync<CustomResponse<WeatherForecast[]>>(CombineUri(requestPath));

        CheckResponseCode(response, new(ResponseState.Error, 30000));

        Debug.WriteLine(response.Message);
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

    protected static void CheckResponseCode([NotNull] CustomResponse<WeatherForecast[]>? apiResponse, ResponseCode code)
    {
        Assert.IsNotNull(apiResponse);
        Assert.AreEqual(code.State, apiResponse.Code.State);
        Assert.AreEqual(code.BusinessCode, apiResponse.Code.BusinessCode);
    }

    [DebuggerStepThrough]
    protected string CombineUri(string path) => $"{UriPrefix}{path}";

    protected override async Task<IHostBuilder> CreateServerHostBuilderAsync()
    {
        var builder = await CreateServerHost();
        builder.ConfigureServices(services =>
        {
            services.AddResponseAutoWrapper<CustomResponse<object>, ResponseCode, ResponseMessage>(GetOptionsSetupAction())
                    .ConfigureWrappers(builder => builder.AddWrappers<CustomResponseWrapper>());
        });
        return builder;
    }

    protected virtual Action<ResponseAutoWrapperOptions>? GetOptionsSetupAction()
    {
        return options => { };
    }

    #endregion Protected 方法
}

[TestClass]
public class CRTest_DisableOpenAPISupport : CRTest
{
    #region Protected 方法

    protected override Action<ResponseAutoWrapperOptions>? GetOptionsSetupAction()
    {
        return options => options.DisableOpenAPISupport = true;
    }

    #endregion Protected 方法
}

[TestClass]
public class CRTest_NotGeneric : CRTest
{
    #region Public 属性

    public override string UriPrefix { get; set; } = "/api/NGCRWeatherForecast";

    #endregion Public 属性

    #region Protected 方法

    protected override Task<IHostBuilder> CreateServerHostBuilderAsync() => CreateServerHostWithStartup<NotGenericCRStartup>();

    #endregion Protected 方法
}
