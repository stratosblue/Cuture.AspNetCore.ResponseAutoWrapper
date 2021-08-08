using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

namespace ResponseAutoWrapper.TestHost.Controllers
{
    //CustomResponseIByResponseCreator
    //Copy from NGCRCWeatherForecastController

    [ApiController]
    [Route("api/[controller]")]
    public class NGCRIWeatherForecastController : CRIWeatherForecastController
    {
        #region Public 方法

        [NonAction]
        public override CustomResponseI<WeatherForecast[]> GetDirectApiResponse()
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("get-direct-api-response")]
        public CustomResponseI GetDirectApiResponseNG()
        {
            return new CustomResponseI() { Result = WeatherForecast.GenerateData() };
        }

        [NonAction]
        public override InheritedCustomResponseI GetDirectInheritedApiResponse()
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("get-direct-inherited-api-response")]
        public InheritedCustomResponseINotGeneric GetDirectInheritedApiResponseNG()
        {
            return new InheritedCustomResponseINotGeneric()
            {
                Result = WeatherForecast.GenerateData()
            };
        }

        public override dynamic GetDynamic(int type)
        {
            return type switch
            {
                0 => WeatherForecast.GenerateData(),
                1 => new CustomResponseI() { Result = WeatherForecast.GenerateData() },
                2 => new InheritedCustomResponseINotGeneric() { Result = WeatherForecast.GenerateData() },
                _ => WeatherForecast.GenerateData(),
            };
        }

        [NonAction]
        public override Task<CustomResponseI<WeatherForecast[]>> GetTaskDirectApiResponseAsync()
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("get-task-direct-api-response")]
        public async Task<CustomResponseI> GetTaskDirectApiResponseAsyncNG()
        {
            await Task.Delay(1);
            return new CustomResponseI() { Result = WeatherForecast.GenerateData() };
        }

        [NonAction]
        public override ValueTask<CustomResponseI<WeatherForecast[]>> GetValueTaskDirectApiResponseAsync()
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("get-valuetask-direct-api-response")]
        public async ValueTask<CustomResponseI> GetValueTaskDirectApiResponseAsyncNG()
        {
            await Task.Delay(1);
            return new CustomResponseI() { Result = WeatherForecast.GenerateData() };
        }

        #endregion Public 方法
    }
}