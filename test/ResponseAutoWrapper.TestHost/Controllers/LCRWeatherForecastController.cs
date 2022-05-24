using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ResponseAutoWrapper.TestHost.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LCRWeatherForecastController : GenericWeatherForecastController
{
    #region Public 方法

    [HttpGet]
    [Route("get-direct-api-response")]
    public LegacyCustomResponse<WeatherForecast[]> GetDirectApiResponse()
    {
        return new LegacyCustomResponse<WeatherForecast[]>() { Datas = WeatherForecast.GenerateData() };
    }

    [HttpGet]
    [Route("get-direct-inherited-api-response")]
    public InheritedLegacyCustomResponse GetDirectInheritedApiResponse()
    {
        return new InheritedLegacyCustomResponse()
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
            1 => new LegacyCustomResponse<WeatherForecast[]>() { Datas = WeatherForecast.GenerateData() },
            2 => new InheritedLegacyCustomResponse() { Datas = WeatherForecast.GenerateData() },
            _ => WeatherForecast.GenerateData(),
        };
    }

    [HttpGet]
    [Route("get-task-direct-api-response")]
    public async Task<LegacyCustomResponse<WeatherForecast[]>> GetTaskDirectApiResponseAsync()
    {
        await Task.Delay(1);
        return new LegacyCustomResponse<WeatherForecast[]>() { Datas = WeatherForecast.GenerateData() };
    }

    [HttpGet]
    [Route("get-valuetask-direct-api-response")]
    public async ValueTask<LegacyCustomResponse<WeatherForecast[]>> GetValueTaskDirectApiResponseAsync()
    {
        await Task.Delay(1);
        return new LegacyCustomResponse<WeatherForecast[]>() { Datas = WeatherForecast.GenerateData() };
    }

    #endregion Public 方法
}
