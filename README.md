# TodoApi - ASP.NET Core WebAPI Sample

This project demonstrates a simple ASP.NET Core WebAPI with a focus on Dependency Injection (DI) and service lifetimes (Singleton, Scoped, Transient).

## Project Overview

- **Controllers:** Exposes endpoints for weather forecasts and DI lifetime demonstrations.
- **Services:** Implements and registers services with different lifetimes to show how DI works in .NET Core.
- **DI Demonstration:** Endpoints show how Singleton, Scoped, and Transient services behave.

## Dependency Injection in ASP.NET Core

Dependency Injection (DI) is a design pattern that allows you to inject dependencies (services) into classes, rather than creating them directly. ASP.NET Core has built-in support for DI.

### How Services Are Registered

Services are registered in `Program.cs` using the `IServiceCollection`:

```csharp
// Registering services with different lifetimes
builder.Services.AddSingleton<ISingletonService, SingletonService>();
builder.Services.AddScoped<IScopedService, ScopedService>();
builder.Services.AddTransient<ITransientService, TransientService>();
```

- **Singleton:** Only one instance is created and shared for the application's lifetime.
- **Scoped:** A new instance is created per HTTP request and shared within that request.
- **Transient:** A new instance is created every time the service is requested.

### How Services Are Used

#### 1. Constructor Injection (Recommended)

Services are injected into controllers or other services via constructor parameters:

```csharp
public class WeatherForecastController : ControllerBase
{
    private readonly ISingletonService _singletonService;
    private readonly IScopedService _scopedService;
    private readonly ITransientService _transientService;

    public WeatherForecastController(
        ISingletonService singletonService,
        IScopedService scopedService,
        ITransientService transientService)
    {
        _singletonService = singletonService;
        _scopedService = scopedService;
        _transientService = transientService;
    }
}
```

#### 2. Outside Constructor (Service Locator Pattern)

You can also resolve services outside the constructor using `IServiceProvider`, but this is discouraged except for advanced scenarios:

```csharp
public class MyClass
{
    private readonly IServiceProvider _serviceProvider;
    public MyClass(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    public void DoSomething()
    {
        var myService = _serviceProvider.GetService<IMyService>();
        // Use myService
    }
}
```

### DI Lifetime Example Endpoints

- `GET /weatherforecast/di-lifetimes` — Shows the operation IDs (GUIDs) for singleton, scoped, and transient services.
- `GET /weatherforecast/di-lifetimes-detailed` — Injects two instances of each service type to clearly show:
    - Singleton: always the same
    - Scoped: same within a request
    - Transient: always different

### Interview Perspective: Key Points

- **Registration:** Services are registered in the DI container in `Program.cs` with a specific lifetime.
- **Usage:** Services are injected into controllers or other services, usually via constructor injection.
- **Lifetimes:**
    - **Singleton:** One instance for the whole app (e.g., configuration, caching).
    - **Scoped:** One instance per request (e.g., database context).
    - **Transient:** New instance every time (e.g., lightweight, stateless services).
- **Best Practice:** Prefer constructor injection for clarity and testability.
- **Advanced:** Use `IServiceProvider` for dynamic or late binding, but avoid overuse.

## Running the Project

1. Build and run the project:
   ```bash
   dotnet run
   ```
2. Access endpoints like:
   - `http://localhost:5099/weatherforecast`
   - `http://localhost:5099/weatherforecast/di-lifetimes`
   - `http://localhost:5099/weatherforecast/di-lifetimes-detailed`

## Summary

This project is a practical demonstration of dependency injection and service lifetimes in ASP.NET Core WebAPI. It is ideal for learning, interviews, and as a reference for best practices.
