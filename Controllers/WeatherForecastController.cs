using Microsoft.AspNetCore.Mvc;
// Import the service interfaces for DI examples
// using TodoApi.Services; // Not needed since interfaces are in root namespace

namespace TodoApi.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IMessageService _messageService;
    // Inject each service type to demonstrate DI lifetimes
    private readonly IScopedService _scopedService1;
    private readonly IScopedService _scopedService2;
    private readonly ITransientService _transientService1;
    private readonly ITransientService _transientService2;
    private readonly ISingletonService _singletonService;

    // The constructor receives dependencies via parameters.
    // ASP.NET Core automatically provides these instances based on their lifetimes.
    public WeatherForecastController(
        ILogger<WeatherForecastController> logger,
        IMessageService messageService,
        IScopedService scopedService1,
        IScopedService scopedService2,
        ITransientService transientService1,
        ITransientService transientService2,
        ISingletonService singletonService)
    {
        _logger = logger;
        _messageService = messageService;
        _scopedService1 = scopedService1;
        _scopedService2 = scopedService2;
        _transientService1 = transientService1;
        _transientService2 = transientService2;
        _singletonService = singletonService;
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

    // This endpoint demonstrates using the injected message service.
    [HttpGet("message")]
    public string GetMessage()
    {
        return _messageService.GetMessage();
    }

    // This endpoint demonstrates the different DI lifetimes.
    [HttpGet("di-lifetimes")]
    public object GetDiLifetimes()
    {
        // Each service returns its unique operation ID (Guid)
        // - Singleton: always the same
        // - Scoped: same within a request, different across requests
        // - Transient: always different
        return new
        {
            Singleton = _singletonService.GetOperationId(),
            Scoped = _scopedService1.GetOperationId(),
            Transient = _transientService1.GetOperationId()
        };
    }

    // This endpoint demonstrates the difference between scoped and transient lifetimes more clearly.
    [HttpGet("di-lifetimes-detailed")]
    public object GetDiLifetimesDetailed()
    {
        return new
        {
            Singleton = _singletonService.GetOperationId(),
            Scoped1 = _scopedService1.GetOperationId(),
            Scoped2 = _scopedService2.GetOperationId(),
            Transient1 = _transientService1.GetOperationId(),
            Transient2 = _transientService2.GetOperationId()
        };
    }
}
