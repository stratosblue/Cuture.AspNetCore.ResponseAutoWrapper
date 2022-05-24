using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ResponseAutoWrapper.TestHost.Controllers;

/// <summary>
/// 通用的 WeatherForecastController
/// </summary>
[ApiController]
[Route("api/[controller]")]
public abstract class GenericWeatherForecastController : ControllerBase
{
    #region Public 字段

    public const int CustomCode = 10086;
    public const string CustomMessage = "Hello world!";

    #endregion Public 字段

    #region Public 方法

    [HttpGet]
    [Route("get")]
    public WeatherForecast[] Get(int type = 0)
    {
        return type switch
        {
            1 or 2 => DescribeResponse<WeatherForecast[]>(type == 1 ? WeatherForecast.GenerateData() : null),
            _ => WeatherForecast.GenerateData(),
        };
    }

    [HttpGet]
    [Route("get-inheritedtask")]
    public InheritedTask<WeatherForecast[]> GetByInheritedTask(int type = 0)
    {
        var task = new InheritedTask<WeatherForecast[]>(() => type switch
        {
            1 or 2 => DescribeResponse<WeatherForecast[]>(type == 1 ? WeatherForecast.GenerateData() : null),
            _ => WeatherForecast.GenerateData(),
        });

        task.Start();
        return task;
    }

    [HttpGet]
    [Route("get-inheritedtask2")]
    public TwiceInheritedTask GetByInheritedTask2(int type = 0)
    {
        var task = new TwiceInheritedTask(() => type switch
        {
            1 or 2 => DescribeResponse<WeatherForecast[]>(type == 1 ? WeatherForecast.GenerateData() : null),
            _ => WeatherForecast.GenerateData(),
        });
        task.Start();
        return task;
    }

    [HttpGet]
    [Route("get-dynamic")]
    public abstract dynamic GetDynamic(int resultType, int type = 0);

    [HttpGet]
    [Route("get-dynamic-task")]
    public async Task<dynamic> GetDynamicTaskAsync(int resultType, int type = 0)
    {
        await Task.Delay(1);

        return GetDynamic(resultType, type);
    }

    [HttpGet]
    [Route("get-dynamic-valuetask")]
    public async ValueTask<dynamic> GetDynamicValueTaskAsync(int resultType, int type = 0)
    {
        await Task.Delay(1);

        return GetDynamic(resultType, type);
    }

    [HttpGet]
    [AuthorizeMixed]
    [Route("get-authentication")]
    public WeatherForecast[] GetNeedAuthentication()
    {
        return WeatherForecast.GenerateData();
    }

    [HttpGet]
    [AuthorizeMixed("CanAccess")]
    [Route("get-authorize")]
    public WeatherForecast[] GetNeedAuthorize()
    {
        return WeatherForecast.GenerateData();
    }

    [HttpGet]
    [Route("get-nowrap")]
    [NoResponseWrap]
    public WeatherForecast[] GetNoWrap()
    {
        return WeatherForecast.GenerateData();
    }

    [HttpGet]
    [Route("get-task")]
    public async Task<WeatherForecast[]?> GetTaskAsync(int type = 0)
    {
        await Task.Delay(1);

        return type switch
        {
            1 or 2 => DescribeResponse<WeatherForecast[]>(type == 1 ? WeatherForecast.GenerateData() : null),
            _ => WeatherForecast.GenerateData(),
        };
    }

    [HttpGet]
    [Route("get-valuetask")]
    public async ValueTask<WeatherForecast[]> GetValueTaskAsync(int type = 0)
    {
        await Task.Delay(1);

        return type switch
        {
            1 or 2 => DescribeResponse<WeatherForecast[]>(type == 1 ? WeatherForecast.GenerateData() : null),
            _ => WeatherForecast.GenerateData(),
        };
    }

    [HttpGet]
    [Route("get-exception-throw")]
    public WeatherForecast[] GetWithExceptionThrow()
    {
        throw new Exception("There has some exception");
    }

    [HttpGet]
    [Route("get-with-param")]
    public WeatherForecast[] GetWithParam(int? count)
    {
        return WeatherForecast.GenerateData(count ?? 5);
    }

    [HttpGet]
    [Route("get-with-param-required")]
    public WeatherForecast[] GetWithParamRequired([Required] int? count)
    {
        return WeatherForecast.GenerateData(count!.Value);
    }

    [HttpGet]
    [Route("get-with-param-required-limit")]
    public WeatherForecast[] GetWithParamRequiredAndLimit([Required][Range(1, 10)] int? count)
    {
        return WeatherForecast.GenerateData(count!.Value);
    }

    #endregion Public 方法

    #region Protected 方法

    protected virtual TResult DescribeResponse<TResult>(TResult result)
    {
        HttpContext.DescribeResponse(CustomCode, CustomMessage);
        return result;
    }

    #endregion Protected 方法
}
