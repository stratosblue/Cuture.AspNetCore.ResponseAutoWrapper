using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using Cuture.Http;

using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ResponseAutoWrapper.TestHost;

namespace ResponseAutoWrapper.Test
{
    public abstract class TestServerBase
    {
        #region Private 字段

        private IHost _host = null!;

        #endregion Private 字段

        #region Protected 属性

        protected HttpClient Client { get; private set; } = null!;

        #endregion Protected 属性

        #region Public 方法

        [TestCleanup]
        public async Task CleanupAsync()
        {
            await _host.StopAsync();
            _host.Dispose();
            _host = null!;
        }

        [TestInitialize]
        public async Task InitAsync()
        {
            var hostBuilder = await CreateServerHostBuilderAsync();
            _host = hostBuilder.Build();
            await _host.StartAsync();
            Client = _host.GetTestClient();
        }

        #endregion Public 方法

        #region Protected 方法

        protected static void CheckWeatherForecast(WeatherForecast? weatherForecast)
        {
            Assert.IsNotNull(weatherForecast);
            Assert.IsFalse(string.IsNullOrWhiteSpace(weatherForecast.Summary));
            Assert.IsTrue(weatherForecast.Date >= DateTime.Today);
        }

        protected static void CheckWeatherForecast(IEnumerable<WeatherForecast>? weatherForecasts, int count = 5)
        {
            Assert.IsNotNull(weatherForecasts);
            Assert.AreEqual(count, weatherForecasts.Count());

            foreach (var item in weatherForecasts)
            {
                CheckWeatherForecast(item);
            }
        }

        protected Task<IHostBuilder> CreateServerHost()
            => Task.FromResult(Hosts.CreateHostBuilder(true));

        protected abstract Task<IHostBuilder> CreateServerHostBuilderAsync();

        protected Task<IHostBuilder> CreateServerHostWithStartup<TStartup>() where TStartup : BaseStartup
            => Task.FromResult(Hosts.CreateHostBuilder<TStartup>(true));

        protected async Task<string> LoginAsync(bool canAccess, bool isCookie, string jwtPropertyName = "data")
        {
            if (isCookie)
            {
                //cookie
                var responseMessage = await Client.CreateRequest($"/api/Login/cookie?canAccess={canAccess}").TryGetAsStringAsync();

                Assert.IsNotNull(responseMessage.ResponseMessage);
                return responseMessage.ResponseMessage.GetCookie();
            }
            else
            {
                //jwt
                var responseMessage = await Client.CreateRequest($"/api/Login/jwt?canAccess={canAccess}").TryGetAsJsonDocumentAsync();

                Assert.IsNotNull(responseMessage.Data?.RootElement);
                var token = responseMessage.Data.RootElement.GetProperty(jwtPropertyName).GetString();

                Assert.IsNotNull(token);
                return token;
            }
        }

        #endregion Protected 方法
    }
}