using System;

namespace CustomStructureWebApplication;

public class WeatherForecast
{
    #region Public 属性

    public DateTime Date { get; set; }

    public string Summary { get; set; }

    public int TemperatureC { get; set; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    #endregion Public 属性
}
