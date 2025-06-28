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
- The foreign key property does not follow the `{NavigationPropertyName}Id` naming convention.
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

## JWT Authentication and Microsoft Identity in ASP.NET Core WebAPI

### JWT Authentication

- **Purpose:** Secure your API endpoints so only authenticated users can access them.
- **How it works:**
  1. User logs in and receives a JWT (JSON Web Token) from the server.
  2. The client sends the JWT in the `Authorization: Bearer <token>` header for each API request.
  3. The server validates the token and grants access based on the claims (e.g., user ID, roles).

**Setup Steps:**
1. Add the NuGet package:
   ```bash
   dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
   ```
2. Configure authentication in `Program.cs`:
   ```csharp
   builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
       .AddJwtBearer(options =>
       {
           options.TokenValidationParameters = new TokenValidationParameters
           {
               ValidateIssuer = false,
               ValidateAudience = false,
               ValidateLifetime = true,
               ValidateIssuerSigningKey = true,
               IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YourSuperSecretKey123!"))
           };
       });
   // ...
   app.UseAuthentication();
   app.UseAuthorization();
   ```
3. Protect controllers/actions with `[Authorize]`.
4. Issue JWTs to users after login (see `/login` endpoint example in the controller).

**How to Use:**
- Send the JWT in the `Authorization` header:
  ```http
  Authorization: Bearer <your_jwt_token>
  ```

---

### Microsoft Identity

- **Purpose:** Provides a full authentication and user management system (users, roles, passwords, etc.).
- **How it works:**
  - Uses `AspNetUsers`, `AspNetRoles`, and related tables for user and role management.
  - Supports registration, login, password reset, role assignment, etc.
  - Can be combined with JWT for token-based authentication.

**Setup Steps:**
1. Add the NuGet packages:
   ```bash
   dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
   dotnet add package Microsoft.AspNetCore.Identity.UI
   ```
2. Register Identity in `Program.cs`:
   ```csharp
   builder.Services.AddIdentity<IdentityUser, IdentityRole>()
       .AddEntityFrameworkStores<UsersDbContext>();
   ```
3. Use Identity endpoints for registration, login, etc., or create your own.
4. Use `[Authorize(Roles = "Admin")]` to restrict access by role.

**Interview Points:**
- **JWT** is stateless and ideal for APIs; tokens are self-contained and sent with each request.
- **Microsoft Identity** is a full-featured system for user and role management, and can be used with or without JWT.
- **Best Practice:** Use Identity for user management and JWT for API authentication.

---

## ASP.NET Core Application Pipeline: `app.Run`, `app.Map`, and `app.Use`

### `app.Run`
- **Purpose:** Starts the web application and begins listening for incoming HTTP requests.
- **Usage:** Common in frameworks like ASP.NET Core (`app.Run()`).
- **Example:**
    ```csharp
    // ...existing code...
    app.Run();
    // ...existing code...
    ```

### `app.Map`
- **Purpose:** Maps a specific route or endpoint to a handler (function, controller, etc.).
- **Usage:** Used to define how the app responds to different URLs.
- **Example:**
    ```csharp
    // ...existing code...
    app.MapGet("/hello", () => "Hello World!");
    // ...existing code...
    ```

### `app.Use`
- **Purpose:** Adds middleware to the request processing pipeline.
- **Usage:** Middleware can handle requests, responses, logging, authentication, etc.
- **Example:**
    ```csharp
    // ...existing code...
    app.Use(async (context, next) =>
    {
        // Do something before
        await next();
        // Do something after
    });
    // ...existing code...
    ```

**Summary:**
- `app.Run` starts the app.
- `app.Map` defines routes/endpoints.
- `app.Use` adds middleware to handle requests/responses.

---

## Design Patterns: Singleton, Factory, and Repository

### Singleton Pattern
Ensures a class has only one instance and provides a global point of access to it.

```csharp
public class Logger
{
    private static Logger _instance;
    private static readonly object _lock = new object();
    private Logger() { }
    public static Logger Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                    _instance = new Logger();
                return _instance;
            }
        }
    }
    public void Log(string message) => Console.WriteLine(message);
}
```
**Usage:**
```csharp
Logger.Instance.Log("Hello");
```

---

### Factory Method Pattern (with ISendEmail Example)
Creates objects without specifying the exact class. Useful for encapsulating object creation logic.

```csharp
public interface ISendEmail
{
    void Send(string to, string subject, string body);
}

public class SMTPMail : ISendEmail
{
    public void Send(string to, string subject, string body)
    {
        Console.WriteLine($"SMTP: Sending mail to {to}");
    }
}

public class SendGridMail : ISendEmail
{
    public void Send(string to, string subject, string body)
    {
        Console.WriteLine($"SendGrid: Sending mail to {to}");
    }
}

public class EmailFactory
{
    public static ISendEmail Create(string type)
    {
        return type switch
        {
            "smtp" => new SMTPMail(),
            "sendgrid" => new SendGridMail(),
            _ => throw new ArgumentException("Unknown type")
        };
    }
}
```
**Usage:**
```csharp
var mailer = EmailFactory.Create("smtp");
mailer.Send("user@example.com", "Subject", "Body");
```

---

### Repository Pattern
Abstracts data access logic, separating it from business logic.

```csharp
public interface IUserRepository
{
    User GetById(int id);
    void Add(User user);
}

public class UserRepository : IUserRepository
{
    private readonly UsersDbContext _context;
    public UserRepository(UsersDbContext context) => _context = context;
    public User GetById(int id) => _context.Users.Find(id);
    public void Add(User user) => _context.Users.Add(user);
}
```
**Usage:**
```csharp
var user = userRepository.GetById(1);
```

---

### Repository Pattern Example Using ADO.NET
This example shows how to implement the repository pattern using raw ADO.NET for data access instead of Entity Framework.

```csharp
public interface IUserRepository
{
    User GetById(int id);
    void Add(User user);
}

public class UserRepositoryAdoNet : IUserRepository
{
    private readonly string _connectionString;
    public UserRepositoryAdoNet(string connectionString)
    {
        _connectionString = connectionString;
    }

    public User GetById(int id)
    {
        using var conn = new SqlConnection(_connectionString);
        conn.Open();
        using var cmd = new SqlCommand("SELECT Id, Name, Email FROM Users WHERE Id = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", id);
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return new User
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Email = reader.GetString(2)
            };
        }
        return null;
    }

    public void Add(User user)
    {
        using var conn = new SqlConnection(_connectionString);
        conn.Open();
        using var cmd = new SqlCommand("INSERT INTO Users (Name, Email) VALUES (@Name, @Email)", conn);
        cmd.Parameters.AddWithValue("@Name", user.Name);
        cmd.Parameters.AddWithValue("@Email", user.Email);
        cmd.ExecuteNonQuery();
    }
}
```
**Usage:**
```csharp
IUserRepository repo ; From DI.
repo.Add(new User { Name = "Alice", Email = "alice@example.com" });
var user = repo.GetById(1);
```

---
