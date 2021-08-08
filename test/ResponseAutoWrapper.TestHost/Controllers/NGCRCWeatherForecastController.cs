using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

namespace ResponseAutoWrapper.TestHost.Controllers
{
    //CustomResponseByResponseCreator
    //Copy from CRIWeatherForecastController

    [ApiController]
    [Route("api/[controller]")]
    public class NGCRCWeatherForecastController : CRCWeatherForecastController
    {
        #region Public 方法

        [NonAction]
        public override CustomResponse<WeatherForecast[]> GetDirectApiResponse()
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("get-direct-api-response")]
        public CustomResponse GetDirectApiResponseNG()
        {
            return new CustomResponse() { Datas = WeatherForecast.GenerateData() };
        }

        [NonAction]
        public override InheritedCustomResponse GetDirectInheritedApiResponse()
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("get-direct-inherited-api-response")]
        public InheritedCustomResponseNotGeneric GetDirectInheritedApiResponseNG()
        {
            return new InheritedCustomResponseNotGeneric()
            {
                Datas = WeatherForecast.GenerateData()
            };
        }

        public override dynamic GetDynamic(int type)
        {
            return type switch
            {
                0 => WeatherForecast.GenerateData(),
                1 => new CustomResponse() { Datas = WeatherForecast.GenerateData() },
                2 => new InheritedCustomResponseNotGeneric() { Datas = WeatherForecast.GenerateData() },
                _ => WeatherForecast.GenerateData(),
            };
        }

        [NonAction]
        public override Task<CustomResponse<WeatherForecast[]>> GetTaskDirectApiResponseAsync()
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("get-task-direct-api-response")]
        public async Task<CustomResponse> GetTaskDirectApiResponseAsyncNG()
        {
            await Task.Delay(1);
            return new CustomResponse() { Datas = WeatherForecast.GenerateData() };
        }

        [NonAction]
        public override ValueTask<CustomResponse<WeatherForecast[]>> GetValueTaskDirectApiResponseAsync()
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("get-valuetask-direct-api-response")]
        public async ValueTask<CustomResponse> GetValueTaskDirectApiResponseAsyncNG()
        {
            await Task.Delay(1);
            return new CustomResponse() { Datas = WeatherForecast.GenerateData() };
        }

        #endregion Public 方法
    }
}