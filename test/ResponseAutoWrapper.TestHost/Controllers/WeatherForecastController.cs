using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ResponseAutoWrapper.TestHost.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherForecastController : GenericWeatherForecastController
    {
        #region Public 方法

        [HttpGet]
        [Route("get-direct-api-response")]
        public ApiResponse<WeatherForecast[]> GetDirectApiResponse()
        {
            return ApiResponse.Create(WeatherForecast.GenerateData());
        }

        [HttpGet]
        [Route("get-direct-inherited-api-response")]
        public InheritedApiResponse GetDirectInheritedApiResponse()
        {
            return new InheritedApiResponse()
            {
                Data = WeatherForecast.GenerateData()
            };
        }

        [HttpGet]
        [Route("get-dynamic")]
        public override dynamic GetDynamic(int resultType, int type = 0)
        {
            if (type == 1)
            {
                HttpContext.DescribeResponse(CustomCode, CustomMessage);
            }

            return resultType switch
            {
                0 => WeatherForecast.GenerateData(),
                1 => ApiResponse.Create(WeatherForecast.GenerateData()),
                2 => new InheritedApiResponse() { Data = WeatherForecast.GenerateData() },
                _ => WeatherForecast.GenerateData(),
            };
        }

        [HttpGet]
        [Route("get-task-direct-api-response")]
        public async Task<ApiResponse<WeatherForecast[]>> GetTaskDirectApiResponseAsync()
        {
            await Task.Delay(1);
            return ApiResponse.Create(WeatherForecast.GenerateData());
        }

        [HttpGet]
        [Route("get-valuetask-direct-api-response")]
        public async ValueTask<ApiResponse<WeatherForecast[]>> GetValueTaskDirectApiResponseAsync()
        {
            await Task.Delay(1);
            return ApiResponse.Create(WeatherForecast.GenerateData());
        }

        #endregion Public 方法
    }
}