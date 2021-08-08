using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;
using System.Threading.Tasks;

using Cuture.AspNetCore.ResponseAutoWrapper;
using Cuture.Http;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ResponseAutoWrapper.TestHost;

namespace ResponseAutoWrapper.Test
{
    [TestClass]
    public class GenericApiTest : TestServerBase
    {
        #region Public 方法

        [TestMethod]
        [DataRow("/api/WeatherForecast/get")]
        [DataRow("/api/WeatherForecast/get-inheritedtask")]
        [DataRow("/api/WeatherForecast/get-inheritedtask2")]
        [DataRow("/api/WeatherForecast/get-direct-api-response")]
        [DataRow("/api/WeatherForecast/get-direct-inherited-api-response")]
        [DataRow("/api/WeatherForecast/get-dynamic?type=0")]
        [DataRow("/api/WeatherForecast/get-dynamic?type=1")]
        [DataRow("/api/WeatherForecast/get-dynamic?type=2")]
        [DataRow("/api/WeatherForecast/get-dynamic-task?type=0")]
        [DataRow("/api/WeatherForecast/get-dynamic-task?type=1")]
        [DataRow("/api/WeatherForecast/get-dynamic-task?type=2")]
        [DataRow("/api/WeatherForecast/get-dynamic-valuetask?type=0")]
        [DataRow("/api/WeatherForecast/get-dynamic-valuetask?type=1")]
        [DataRow("/api/WeatherForecast/get-dynamic-valuetask?type=2")]
        [DataRow("/api/WeatherForecast/get-task")]
        [DataRow("/api/WeatherForecast/get-task-direct-api-response")]
        [DataRow("/api/WeatherForecast/get-valuetask")]
        [DataRow("/api/WeatherForecast/get-valuetask-direct-api-response")]
        [DataRow("/api/WeatherForecast/get-with-param")]
        [DataRow("/api/WeatherForecast/get-with-param-required?count=5")]
        [DataRow("/api/WeatherForecast/get-with-param-required-limit?count=5")]
        public async Task Should_Wrapped200(string requestPath)
        {
            var response = await Client.GetFromJsonAsync<ApiResponse<WeatherForecast[]>>(requestPath);

            CheckResponseCode(response);
            CheckWeatherForecast(response.Data);
        }

        [TestMethod]
        [DataRow("/api/WeatherForecast/get-authorize")]
        public async Task Should_Wrapped200ThroughAuthorize(string requestPath)
        {
            var response = await Client.GetFromJsonAsync<ApiResponse<WeatherForecast[]>>(requestPath);

            CheckResponseCode(response, 401);

            Assert.IsNull(response.Data);

            Debug.WriteLine($"No authentication Message: {response.Message}");

            #region Cookie

            var cookie = await LoginAsync(false, true);

            response = await Client.CreateRequest(requestPath)
                                   .UseCookie(cookie)
                                   .GetAsObjectAsync<ApiResponse<WeatherForecast[]>>();

            CheckResponseCode(response, 403);

            Debug.WriteLine($"Cookie can not access Message: {response.Message}");

            cookie = await LoginAsync(true, true);

            response = await Client.CreateRequest(requestPath)
                                   .UseCookie(cookie)
                                   .GetAsObjectAsync<ApiResponse<WeatherForecast[]>>();

            CheckResponseCode(response);
            CheckWeatherForecast(response.Data);

            #endregion Cookie

            #region Jwt

            var token = await LoginAsync(false, false);

            response = await Client.CreateRequest(requestPath)
                                   .UseBearerToken(token)
                                   .GetAsObjectAsync<ApiResponse<WeatherForecast[]>>();

            CheckResponseCode(response, 403);

            Debug.WriteLine($"Jwt can not access Message: {response.Message}");

            token = await LoginAsync(true, false);

            response = await Client.CreateRequest(requestPath)
                                   .AddHeader("Authorization", $"Bearer {token}")
                                   .GetAsObjectAsync<ApiResponse<WeatherForecast[]>>();

            CheckResponseCode(response);
            CheckWeatherForecast(response.Data);

            #endregion Jwt
        }

        [TestMethod]
        [DataRow("/api/WeatherForecast/get-with-param-required")]
        [DataRow("/api/WeatherForecast/get-with-param-required-limit?count=0")]
        [DataRow("/api/WeatherForecast/get-with-param-required-limit?count=11")]
        public async Task Should_Wrapped400(string requestPath)
        {
            var response = await Client.GetFromJsonAsync<ApiResponse<WeatherForecast[]>>(requestPath);

            CheckResponseCode(response, 400);

            Debug.WriteLine(response.Message);
        }

        [TestMethod]
        [DataRow("/api/WeatherForecast/get-authentication")]
        [DataRow("/api/WeatherForecast/get-authorize")]
        public async Task Should_Wrapped401(string requestPath)
        {
            var response = await Client.GetFromJsonAsync<ApiResponse<WeatherForecast[]>>(requestPath);

            CheckResponseCode(response, 401);

            Assert.IsNull(response.Data);

            Debug.WriteLine(response.Message);
        }

        [TestMethod]
        [DataRow("/api/WeatherForecast/get-exception-throw")]
        public async Task Should_Wrapped500(string requestPath)
        {
            var response = await Client.GetFromJsonAsync<ApiResponse<WeatherForecast[]>>(requestPath);

            CheckResponseCode(response, 500);

            Debug.WriteLine(response.Message);
        }

        [TestMethod]
        [DataRow("/api/WeatherForecast/get-nowrap")]
        public async Task ShouldReturnNotWrapped(string requestPath)
        {
            var response = await Client.GetFromJsonAsync<WeatherForecast[]>(requestPath);

            CheckWeatherForecast(response);
        }

        #endregion Public 方法

        #region Protected 方法

        protected static void CheckResponseCode([NotNull] ApiResponse? apiResponse, int code = StatusCodes.Status200OK)
        {
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual(code, apiResponse.Code);
        }

        protected override async Task<IHostBuilder> CreateServerHostBuilderAsync()
        {
            var builder = await CreateServerHost();
            builder.ConfigureServices(services => services.AddResponseAutoWrapper(GetOptionsSetupAction()));
            return builder;
        }

        protected virtual Action<ResponseAutoWrapperOptions>? GetOptionsSetupAction()
        {
            return options => { };
        }

        #endregion Protected 方法
    }

    [TestClass]
    public class GenericApiTest_DisableOpenAPISupport : GenericApiTest
    {
        #region Protected 方法

        protected override Action<ResponseAutoWrapperOptions>? GetOptionsSetupAction()
        {
            return options => options.DisableOpenAPISupport = true;
        }

        #endregion Protected 方法
    }
}