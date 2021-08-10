using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ResponseAutoWrapper.TestHost.Controllers
{
    //CustomResponseByResponseCreator
    //Copy from CRIWeatherForecastController

    [ApiController]
    [Route("api/[controller]")]
    public class NGCRCWeatherForecastController : GenericWeatherForecastController
    {
        #region Public 方法

        [HttpGet]
        [Route("get-direct-api-response")]
        public CustomResponse GetDirectApiResponse()
        {
            return new CustomResponse() { Datas = WeatherForecast.GenerateData() };
        }

        [HttpGet]
        [Route("get-direct-inherited-api-response")]
        public InheritedCustomResponseNotGeneric GetDirectInheritedApiResponse()
        {
            return new InheritedCustomResponseNotGeneric()
            {
                Datas = WeatherForecast.GenerateData()
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
                1 => new CustomResponse() { Datas = WeatherForecast.GenerateData() },
                2 => new InheritedCustomResponseNotGeneric() { Datas = WeatherForecast.GenerateData() },
                _ => WeatherForecast.GenerateData(),
            };
        }

        [HttpGet]
        [Route("get-task-direct-api-response")]
        public async Task<CustomResponse> GetTaskDirectApiResponseAsync()
        {
            await Task.Delay(1);
            return new CustomResponse() { Datas = WeatherForecast.GenerateData() };
        }

        [HttpGet]
        [Route("get-valuetask-direct-api-response")]
        public async ValueTask<CustomResponse> GetValueTaskDirectApiResponseAsync()
        {
            await Task.Delay(1);
            return new CustomResponse() { Datas = WeatherForecast.GenerateData() };
        }

        #endregion Public 方法
    }
}