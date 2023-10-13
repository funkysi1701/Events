using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace EventsTest.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IPublishEndpoint _eventPublisher;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IPublishEndpoint eventPublisher)
        {
            _logger = logger;
            _eventPublisher = eventPublisher;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet(Name = "SendEvent")]
        public async Task SendEvent(CancellationToken cancellationToken)
        {
            try
            {
                await _eventPublisher.Publish<Event>(new 
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Event",
                    CreatedAt = DateTime.Now
                }, cancellationToken);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error sending event");
            }
        }
    }
}
