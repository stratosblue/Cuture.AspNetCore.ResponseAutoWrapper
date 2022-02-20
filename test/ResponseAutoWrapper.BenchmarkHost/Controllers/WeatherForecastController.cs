using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;

using ResponseAutoWrapper.TestHost;

namespace ResponseAutoWrapper.BenchmarkHost.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{

    [HttpGet]
    public IEnumerable<WeatherForecast> Get(int count = 5)
    {
        return WeatherForecast.GenerateData(count);
    }
}
