using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ResponseAutoWrapper.TestHost.Controllers
{
    //CustomResponseIByResponseCreator
    //Copy from NGCRCWeatherForecastController

    [ApiController]
    [Route("api/[controller]")]
    public class NGCRIWeatherForecastController : GenericWeatherForecastController
    {
        #region Public 方法

        [HttpGet]
        [Route("get-direct-api-response")]
        public CustomResponseI GetDirectApiResponse()
        {
            return new CustomResponseI() { Result = WeatherForecast.GenerateData() };
        }

        [HttpGet]
        [Route("get-direct-inherited-api-response")]
        public InheritedCustomResponseINotGeneric GetDirectInheritedApiResponse()
        {
            return new InheritedCustomResponseINotGeneric()
            {
                Result = WeatherForecast.GenerateData()
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

            return type switch
            {
                0 => WeatherForecast.GenerateData(),
                1 => new CustomResponseI() { Result = WeatherForecast.GenerateData() },
                2 => new InheritedCustomResponseINotGeneric() { Result = WeatherForecast.GenerateData() },
                _ => WeatherForecast.GenerateData(),
            };
        }

        [HttpGet]
        [Route("get-task-direct-api-response")]
        public async Task<CustomResponseI> GetTaskDirectApiResponseAsync()
        {
            await Task.Delay(1);
            return new CustomResponseI() { Result = WeatherForecast.GenerateData() };
        }

        [HttpGet]
        [Route("get-valuetask-direct-api-response")]
        public async ValueTask<CustomResponseI> GetValueTaskDirectApiResponseAsync()
        {
            await Task.Delay(1);
            return new CustomResponseI() { Result = WeatherForecast.GenerateData() };
        }

        #endregion Public 方法
    }
}