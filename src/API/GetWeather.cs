using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FrugalWeather.API.Models;

namespace FrugalWeather.API;

public class GetWeather
{
    private readonly ILogger<GetWeather> _logger;
    private static readonly Random _random = new();
    private static readonly string[] _conditions = { "Sunny", "Cloudy", "Rainy", "Partly Cloudy", "Overcast" };
    private static readonly string[] _directions = { "N", "NE", "E", "SE", "S", "SW", "W", "NW" };

    public GetWeather(ILogger<GetWeather> logger)
    {
        _logger = logger;
    }

    [Function("GetWeather")]
    public IActionResult Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "weather")] HttpRequest req)
    {
        _logger.LogInformation("GetWeather function processed a request");

        string location = req.Query["location"];

        if (string.IsNullOrEmpty(location))
        {
            return new BadRequestObjectResult(new
            {
                error = "Please provide a location parameter",
                example = "/api/weather?location=London"
            });
        }

        // Generate mock weather data
        var temperature = _random.Next(-10, 35);
        var weatherData = new WeatherData
        {
            Location = location,
            Timestamp = DateTime.UtcNow,
            Temperature = temperature,
            FeelsLike = temperature + _random.Next(-5, 5),
            Humidity = _random.Next(30, 90),
            Condition = _conditions[_random.Next(_conditions.Length)],
            WindSpeed = Math.Round(_random.NextDouble() * 30, 1),
            WindDirection = _directions[_random.Next(_directions.Length)]
        };

        return new OkObjectResult(weatherData);
    }
}
