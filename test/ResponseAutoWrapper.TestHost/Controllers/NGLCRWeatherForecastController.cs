using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ResponseAutoWrapper.TestHost.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NGLCRWeatherForecastController : GenericWeatherForecastController
{
    #region Public 方法

    [HttpGet]
    [Route("get-direct-api-response")]
    public LegacyCustomResponse GetDirectApiResponse()
    {
        return new LegacyCustomResponse() { Datas = WeatherForecast.GenerateData() };
    }

    [HttpGet]
    [Route("get-direct-inherited-api-response")]
    public InheritedLegacyCustomResponseNotGeneric GetDirectInheritedApiResponse()
    {
        return new InheritedLegacyCustomResponseNotGeneric()
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
            1 => new LegacyCustomResponse() { Datas = WeatherForecast.GenerateData() },
            2 => new InheritedLegacyCustomResponseNotGeneric() { Datas = WeatherForecast.GenerateData() },
            _ => WeatherForecast.GenerateData(),
        };
    }

    [HttpGet]
    [Route("get-task-direct-api-response")]
    public async Task<LegacyCustomResponse> GetTaskDirectApiResponseAsync()
    {
        await Task.Delay(1);
        return new LegacyCustomResponse() { Datas = WeatherForecast.GenerateData() };
    }

    [HttpGet]
    [Route("get-valuetask-direct-api-response")]
    public async ValueTask<LegacyCustomResponse> GetValueTaskDirectApiResponseAsync()
    {
        await Task.Delay(1);
        return new LegacyCustomResponse() { Datas = WeatherForecast.GenerateData() };
    }

    #endregion Public 方法
}
