using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ResponseAutoWrapper.TestHost.Controllers;

//CustomResponseByInterface
//Copy from WeatherForecastController

[ApiController]
[Route("api/[controller]")]
public class CRIWeatherForecastController : GenericWeatherForecastController
{
    #region Public 方法

    [HttpGet]
    [Route("get-direct-api-response")]
    public CustomResponseI<WeatherForecast[]> GetDirectApiResponse()
    {
        return new CustomResponseI<WeatherForecast[]>() { Result = WeatherForecast.GenerateData() };
    }

    [HttpGet]
    [Route("get-direct-inherited-api-response")]
    public InheritedCustomResponseI GetDirectInheritedApiResponse()
    {
        return new InheritedCustomResponseI()
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
            1 => new CustomResponseI<WeatherForecast[]>() { Result = WeatherForecast.GenerateData() },
            2 => new InheritedCustomResponseI() { Result = WeatherForecast.GenerateData() },
            _ => WeatherForecast.GenerateData(),
        };
    }

    [HttpGet]
    [Route("get-task-direct-api-response")]
    public async Task<CustomResponseI<WeatherForecast[]>> GetTaskDirectApiResponseAsync()
    {
        await Task.Delay(1);
        return new CustomResponseI<WeatherForecast[]>() { Result = WeatherForecast.GenerateData() };
    }

    [HttpGet]
    [Route("get-valuetask-direct-api-response")]
    public async ValueTask<CustomResponseI<WeatherForecast[]>> GetValueTaskDirectApiResponseAsync()
    {
        await Task.Delay(1);
        return new CustomResponseI<WeatherForecast[]>() { Result = WeatherForecast.GenerateData() };
    }

    #endregion Public 方法
}
