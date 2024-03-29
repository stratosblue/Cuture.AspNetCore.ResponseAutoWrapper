﻿// <Auto-Generated></Auto-Generated>
using System;
using System.Linq;

namespace ResponseAutoWrapper.TestHost;

public class WeatherForecast
{
    private static readonly string[] s_summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    public DateTime Date { get; set; }

    public int TemperatureC { get; set; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    public string? Summary { get; set; }

    public static WeatherForecast[] GenerateData(int count = 5)
    {
        var rng = new Random();
        return Enumerable.Range(1, count).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = rng.Next(-20, 55),
            Summary = s_summaries[rng.Next(s_summaries.Length)]
        })
        .ToArray();
    }
}
