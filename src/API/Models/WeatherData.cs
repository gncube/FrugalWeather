using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrugalWeather.API.Models;

public class WeatherData
{
    public string Location { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public double Temperature { get; set; }
    public double FeelsLike { get; set; }
    public int Humidity { get; set; }
    public string Condition { get; set; } = string.Empty;
    public double WindSpeed { get; set; }
    public string WindDirection { get; set; } = string.Empty;
}

public class ForecastDay
{
    public DateTime Date { get; set; }
    public double HighTemp { get; set; }
    public double LowTemp { get; set; }
    public string Condition { get; set; } = string.Empty;
    public int ChanceOfRain { get; set; }
}

public class WeatherForecast
{
    public string Location { get; set; } = string.Empty;
    public List<ForecastDay> Forecast { get; set; } = new();
}
