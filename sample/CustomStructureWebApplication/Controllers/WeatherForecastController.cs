using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CustomStructureWebApplication.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController(ILogger<WeatherForecastController> logger) : ControllerBase
{
    #region Private 字段

    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    #endregion Private 字段
    #region Public 构造函数

    #endregion Public 构造函数

    #region Public 方法

    [HttpGet]
    public IEnumerable<WeatherForecast> Get()
    {
        HttpContext.DescribeResponse("E1001", new RichMessage() { Content = "SUCCESS" });
        var rng = new Random();
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = rng.Next(-20, 55),
            Summary = Summaries[rng.Next(Summaries.Length)]
        })
        .ToArray();
    }

    [HttpGet]
    [Route("cm")]
    public CommonResponse<object> GetWithCustomMessage()
    {
        return new CommonResponse<object>() { Code = "E2000", Message = new() { Content = "自定义消息" } };
    }

    [HttpGet]
    [Route("ex")]
    public CommonResponse<object> ThrowException()
    {
        throw new Exception("Some Exception Throwed.");
    }

    #endregion Public 方法
}
