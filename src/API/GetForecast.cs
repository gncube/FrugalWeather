using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FrugalWeather.API.Models;

namespace FrugalWeather.API;

public class GetForecast
{
    private readonly ILogger<GetForecast> _logger;
    private static readonly Random _random = new();
    private static readonly string[] _conditions = { "Sunny", "Cloudy", "Rainy", "Partly Cloudy", "Overcast", "Stormy" };


    public GetForecast(ILogger<GetForecast> logger)
    {
        _logger = logger;
    }

    [Function("GetForecast")]
    public IActionResult Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "weather/forecast")] HttpRequest req)
    {
        _logger.LogInformation("GetForecast function processed a request");

        string location = req.Query["location"];
        string daysParam = req.Query["days"];

        if (string.IsNullOrEmpty(location))
        {
            return new BadRequestObjectResult(new
            {
                error = "Please provide a location parameter",
                example = "/api/weather/forecast?location=London&days=5"
            });
        }

        int days = 5; // Default to 5 days
        if (!string.IsNullOrEmpty(daysParam) && int.TryParse(daysParam, out int parsedDays))
        {
            days = Math.Clamp(parsedDays, 1, 14); // Limit between 1 and 14 days
        }

        var forecast = new WeatherForecast
        {
            Location = location,
            Forecast = new List<ForecastDay>()
        };

        // Generate mock forecast data
        for (int i = 0; i < days; i++)
        {
            var highTemp = _random.Next(10, 35);
            var lowTemp = highTemp - _random.Next(5, 15);

            forecast.Forecast.Add(new ForecastDay
            {
                Date = DateTime.UtcNow.AddDays(i).Date,
                HighTemp = highTemp,
                LowTemp = lowTemp,
                Condition = _conditions[_random.Next(_conditions.Length)],
                ChanceOfRain = _random.Next(0, 100)
            });
        }

        return new OkObjectResult(forecast);
    }
}
