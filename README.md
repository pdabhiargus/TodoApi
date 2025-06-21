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

## Entity Framework Core & DbContext (Interview Perspective)

This project demonstrates how to use Entity Framework Core (EF Core) and DbContext for data persistence in a .NET Core WebAPI.

### 1. Packages Imported
- `Microsoft.EntityFrameworkCore`
- `Microsoft.EntityFrameworkCore.Sqlite` (for SQLite local database)
- `Microsoft.EntityFrameworkCore.Design` (for migrations)

These are added via:
```bash
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore.Design
```

### 2. DbContext and Entity
- `UsersDbContext` inherits from `DbContext` and manages the `User` entity.
- `User` is a POCO class with `Id`, `Name`, and `Email` properties.

**Example:**
```csharp
public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}

public class UsersDbContext : DbContext
{
    public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options) { }
    public DbSet<User> Users { get; set; }
}
```

### 3. Registration in `Program.cs`
Register the DbContext with DI and configure it to use SQLite:
```csharp
builder.Services.AddDbContext<UsersDbContext>(options =>
    options.UseSqlite("Data Source=users.db"));
```

### 4. Usage in Controller
Inject `UsersDbContext` into the controller constructor:
```csharp
private readonly UsersDbContext _context;
public UserController(UsersDbContext context)
{
    _context = context;
}
```
Use `_context.Users` for CRUD operations (Create, Read, Update, Delete).

### 5. Migrations & Database Creation
- **Create migration:**
  ```bash
  dotnet ef migrations add InitialCreate
  ```
- **Apply migration:**
  ```bash
  dotnet ef database update
  ```
This creates the `Users` table in the `users.db` SQLite file.

### 6. Data Persistence
- Data is stored in the `users.db` file in your project directory.
- All CRUD operations via the API are persisted in this SQLite database.

### 7. Key Interview Points
- **DbContext** is the primary class for interacting with the database in EF Core.
- **Registration** in DI ensures a new context per request (scoped lifetime).
- **Migrations** are used to evolve the database schema over time.
- **Data is persisted** in a local SQLite file for development/demo purposes, but the same pattern applies for SQL Server, PostgreSQL, etc.
- **Best Practice:** Always inject DbContext, never instantiate it manually.

## Entity Relationships and Foreign Key Constraints

### User–Department Relationship

In this project, each `User` belongs to a single `Department`, and each `Department` can have multiple `User` objects (a one-to-many relationship).

#### How the Relationship Is Defined

In the POCO classes:

```csharp
public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public int DepartmentId { get; set; }      // Foreign key property
    public Department Department { get; set; } // Navigation property
}

public class Department
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<User> Users { get; set; }      // Navigation property
}
```

#### Convention-Based Foreign Key

- **No explicit attribute is required.**
- Entity Framework Core infers the foreign key relationship because:
  - The `User` class has a property named `DepartmentId`.
  - The `User` class has a navigation property of type `Department`.
  - The `Department` class has a collection navigation property of type `List<User>`.

#### What Happens in the Database

- EF Core creates a `Departments` table and a `Users` table.
- The `Users` table has a `DepartmentId` column.
- A **foreign key constraint** is created from `Users.DepartmentId` to `Departments.Id`.
- This ensures referential integrity: every user must reference a valid department.

#### When to Use `[ForeignKey]` Attribute

You only need to use the `[ForeignKey]` attribute if:
- The foreign key property does **not** follow the `{NavigationPropertyName}Id` naming convention.
- You want to customize or clarify the relationship.

**Example:**
```csharp
[ForeignKey("DeptRef")]
public Department Department { get; set; }
public int DeptRef { get; set; }
```

#### Interview Point

- **By convention**, EF Core will automatically detect and enforce foreign key relationships if you follow standard naming patterns.
- Explicit attributes are only needed for custom scenarios.

---

## HTTP PUT vs POST: Differences and Best Practices

### POST
- **Purpose:** Used to create a new resource.
- **Behavior:** Submits data to the server, which creates a new record and assigns it a new unique identifier (e.g., a new user in the database).
- **Idempotency:** Not idempotent (multiple identical requests may create multiple resources).
- **Example:**
  ```http
  POST /user
  {
    "name": "Alice",
    "email": "alice@example.com"
  }
  ```

### PUT
- **Purpose:** Used to update an existing resource, or create it at a specific URI if it does not exist (upsert).
- **Behavior:** Replaces the entire resource at the given URI with the provided data.
- **Idempotency:** Idempotent (multiple identical requests have the same effect as one).
- **Example:**
  ```http
  PUT /user/1
  {
    "id": 1,
    "name": "Alice Updated",
    "email": "alice@newmail.com"
  }
  ```

### Why Not Use PUT for Insert?
- **Best Practice:** Use `POST` to create new resources and `PUT` to update existing ones.
- **Reason:** `PUT` is meant to update a resource at a known URI. If you use `PUT` to insert, you must provide the full URI (including the new ID), which is not typical for auto-incremented primary keys.
- **Is Insert Possible with PUT?** Yes, technically you can insert with `PUT` (upsert), but it's not recommended for resources with server-generated IDs. Use `POST` for creation to let the server assign the ID.

---

## About __EFMigrationsHistory Table

- **Purpose:** The `__EFMigrationsHistory` table is created and managed by Entity Framework Core in your database.
- **What It Stores:** It keeps track of all migrations that have been applied to the database. Each row represents a migration (by name and product version).
- **Why It's Needed:**
  - Ensures the database schema matches your application's model.
  - Prevents the same migration from being applied multiple times.
  - Allows EF Core to know which migrations are pending or already applied.
- **Interview Point:** This table is essential for safe, incremental schema evolution in production systems using EF Core migrations.

---

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
