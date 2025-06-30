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

## Delegates in C#: `Func` vs `Predicate`

### What is a Delegate?
A delegate is a type that represents references to methods with a particular parameter list and return type. Delegates are used to pass methods as arguments to other methods.

### `Func<T, TResult>`
- Represents a delegate that takes one or more input parameters and returns a value.
- Can have up to 16 input parameters, with the last type parameter being the return type.

**Example:**
```csharp
Func<int, int, int> add = (a, b) => a + b;
int result = add(2, 3); // result = 5

Func<string, int> getLength = s => s.Length;
int len = getLength("hello"); // len = 5
```

### `Predicate<T>`
- Represents a delegate that takes a single parameter and returns a `bool`.
- Used for conditions and filtering (e.g., in `List<T>.Find`, `List<T>.RemoveAll`).

**Example:**
```csharp
Predicate<int> isEven = x => x % 2 == 0;
bool check = isEven(4); // check = true

List<int> numbers = new List<int> { 1, 2, 3, 4, 5 };
List<int> evens = numbers.FindAll(isEven); // evens = [2, 4]
```

### Comparison Table
| Type           | Parameters         | Return Type |
|----------------|--------------------|-------------|
| `Func<T,...,TResult>` | 1 or more         | Any type    |
| `Predicate<T>` | 1                  | `bool`      |

### Summary
- Use `Func<T, TResult>` when you need a delegate that returns any type.
- Use `Predicate<T>` when you need a delegate that returns a boolean (for conditions or filtering).

---

## String Interning in C#

**String interning** is a process where only one copy of each unique literal string value is stored in memory. The .NET runtime maintains a string intern pool to optimize memory usage for strings.

- When you create a string literal, it is automatically interned.
- If you create a new string with the same value, .NET can reuse the interned instance.
- You can use `string.Intern()` to manually intern a string.

**Example:**
```csharp
string a = "hello";
string b = "hello";
string c = new string("hello".ToCharArray());

bool refEqual1 = object.ReferenceEquals(a, b); // true (both interned)
bool refEqual2 = object.ReferenceEquals(a, c); // false (c is not interned)

string d = string.Intern(c);
bool refEqual3 = object.ReferenceEquals(a, d); // true (d is now interned)
```

---

## Extension Methods in C#

**Extension methods** allow you to add new methods to existing types without modifying their source code or creating a new derived type.
- Defined as static methods in a static class.
- The first parameter specifies the type to extend, preceded by the `this` keyword.
- Commonly used to add utility methods to .NET types (e.g., LINQ methods).

**Example:**
```csharp
public static class StringExtensions
{
    public static bool IsCapitalized(this string str)
    {
        if (string.IsNullOrEmpty(str)) return false;
        return char.IsUpper(str[0]);
    }
}

// Usage:
string word = "Hello";
bool isCap = word.IsCapitalized(); // true
```

---

## IEnumerable vs IQueryable in C#

### IEnumerable
- Defined in `System.Collections` namespace.
- Represents a forward-only cursor of a collection (can be used with `foreach`).
- **Executes queries in memory** (client-side).
- Suitable for working with in-memory collections (e.g., `List<T>`, arrays).
- Does not support query translation to a data source (like a database).

**Example:**
```csharp
IEnumerable<int> numbers = new List<int> { 1, 2, 3, 4 };
var evens = numbers.Where(x => x % 2 == 0); // LINQ to Objects, executed in memory
```

### IQueryable
- Defined in `System.Linq` namespace.
- Inherits from `IEnumerable`.
- **Executes queries remotely** (e.g., in a database) by building an expression tree.
- Suitable for querying data sources like Entity Framework, LINQ to SQL, etc.
- Supports query translation and deferred execution on the data source.

**Example:**
```csharp
IQueryable<User> users = dbContext.Users;
var filtered = users.Where(u => u.Name.StartsWith("A")); // Query translated to SQL and executed in DB
```

### Key Differences
| Feature         | IEnumerable                | IQueryable                  |
|----------------|----------------------------|-----------------------------|
| Execution      | In-memory (client-side)    | At data source (server-side)|
| Query Building | Not supported              | Supported (expression tree) |
| Use Case       | In-memory collections      | Remote data sources         |

### Summary
- Use `IEnumerable` for in-memory data.
- Use `IQueryable` for remote data sources to enable efficient, server-side querying.

---

## Achieving Multithreading in C#

Multithreading allows your application to perform multiple operations concurrently, improving responsiveness and performance for CPU-bound or I/O-bound tasks.

### 1. Using `Thread` Class
You can create and start threads manually:
```csharp
using System.Threading;

void PrintNumbers()
{
    for (int i = 1; i <= 5; i++)
        Console.WriteLine(i);
}

Thread t = new Thread(PrintNumbers);
t.Start();
```

### 2. Using `Task` Parallel Library (TPL)
Recommended for most scenarios. `Task` abstracts thread management and supports async/await:
```csharp
using System.Threading.Tasks;

Task.Run(() =>
{
    // Your code here
    Console.WriteLine("Running in a task");
});
```

### 3. Using `Parallel` Class
For parallel loops and data processing:
```csharp
using System.Threading.Tasks;

Parallel.For(0, 10, i =>
{
    Console.WriteLine($"Processing {i}");
});
```

### 4. Using `async` and `await` (for I/O-bound concurrency)
```csharp
public async Task DownloadAsync()
{
    await Task.Delay(1000); // Simulate async work
    Console.WriteLine("Download complete");
}
```

### Summary
- Use `Thread` for low-level control (rarely needed).
- Use `Task` and `async`/`await` for most modern, scalable multithreading.
- Use `Parallel` for data parallelism.

---

## Async/Await in C#: Explanation and Control Flow

### What is `async`/`await`?
- `async` and `await` are C# keywords that simplify writing asynchronous, non-blocking code.
- They allow you to perform long-running operations (like I/O, network calls) without blocking the main thread.

### How It Works
- Mark a method with `async` and return `Task` or `Task<T>`.
- Use `await` to asynchronously wait for a `Task` to complete.
- The method is split into parts: code before `await` runs synchronously, code after `await` resumes when the awaited task completes.

### Example
```csharp
public async Task<string> GetDataAsync()
{
    // Code before await runs synchronously
    await Task.Delay(1000); // Simulate async work (e.g., network call)
    // Code after await runs when the above task completes
    return "Data loaded";
}

// Usage:
string result = await GetDataAsync();
Console.WriteLine(result); // Output: Data loaded
```

### Control Flow
1. The method starts and runs until it hits the first `await`.
2. The awaited task (e.g., `Task.Delay`) starts running asynchronously.
3. The method returns control to the caller (does not block the thread).
4. When the awaited task completes, the method resumes after the `await` line.
5. The method can have multiple `await` statements, each pausing and resuming as needed.

**Visual Flow:**
```
Start method
   |
[Code before await]
   |
[await Task]  <--- returns control to caller, does not block
   |
[Code after await resumes when task completes]
   |
End
```

### Summary
- Use `async`/`await` for non-blocking, readable asynchronous code.
- Control flow is paused at each `await` and resumed when the awaited task finishes.

---

## Difference Between Abstract Class and Interface in C#

| Feature                | Abstract Class                        | Interface                          |
|------------------------|---------------------------------------|-------------------------------------|
| Syntax                 | `abstract class`                      | `interface`                        |
| Members                | Can have fields, properties, methods  | Only declarations (no fields, except static/const in C# 8+) |
| Implementation         | Can have method implementations       | No implementation (C# 8+ allows default methods) |
| Constructors           | Can have constructors                 | Cannot have constructors           |
| Multiple Inheritance   | Only one abstract/base class allowed  | Multiple interfaces can be implemented |
| Access Modifiers       | Can have access modifiers             | All members are public by default   |
| Use Case               | Base class with shared code/logic     | Contract for capabilities/behavior  |

### Example: Abstract Class
```csharp
public abstract class Animal
{
    public abstract void Speak(); // Must be implemented by derived class
    public void Eat() => Console.WriteLine("Eating"); // Optional implementation
}

public class Dog : Animal
{
    public override void Speak() => Console.WriteLine("Woof!");
}
```

### Example: Interface
```csharp
public interface IFlyable
{
    void Fly();
}

public class Bird : IFlyable
{
    public void Fly() => Console.WriteLine("Flying");
}
```

### Summary
- Use **abstract class** for base classes with shared code and partial implementation.
- Use **interface** for defining contracts that multiple unrelated classes can implement.

---

## Default Project Structure of a .NET Core Application

A typical .NET Core WebAPI project has the following structure (example from this project):

```
TodoApi/
├── Controllers/
│   └── WeatherForecastController.cs
├── Properties/
│   └── launchSettings.json
├── appsettings.json
├── appsettings.Development.json
├── Program.cs
├── TodoApi.csproj
├── UsersDbContext.cs
├── ...other files
```

### Key Files and Folders

- **Controllers/**: Contains API controllers (e.g., `WeatherForecastController.cs`) that handle HTTP requests.
- **Program.cs**: Entry point of the application. Configures services, middleware, and starts the app.
- **appsettings.json**: Main configuration file for settings like connection strings, logging, etc.
- **appsettings.Development.json**: Overrides settings in `appsettings.json` for the Development environment.
- **Properties/launchSettings.json**: Stores launch profiles for running and debugging the app locally.
- **TodoApi.csproj**: Project file with dependencies and build settings.
- **UsersDbContext.cs**: Entity Framework Core DbContext for database access.

### Example: `appsettings.json`
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```
- Used for application configuration (logging, connection strings, etc.).

### Example: `appsettings.Development.json`
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```
- Used to override settings in `appsettings.json` when running in Development environment.

### Example: `Properties/launchSettings.json`
```json
{
  "$schema": "https://json.schemastore.org/launchsettings.json",
  "profiles": {
    "http": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": false,
      "applicationUrl": "http://localhost:5099",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    },
    "https": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": false,
      "applicationUrl": "https://localhost:7119;http://localhost:5099",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```
- Defines how the app is launched (URLs, environment, browser launch, etc.) for local development and debugging.

---

## What is .NET Core?
- **.NET Core** is a free, open-source, cross-platform framework for building modern, cloud-based, and internet-connected applications (web, desktop, mobile, microservices, etc.).
- Developed by Microsoft, it runs on Windows, Linux, and macOS.
- Supports C#, F#, and Visual Basic languages.
- Successor to the traditional .NET Framework, designed for flexibility and performance.

## .NET Core vs .NET 5
- **.NET Core** (up to version 3.1) was the cross-platform, modular, and open-source evolution of .NET.
- **.NET 5** is the next step after .NET Core 3.1, unifying .NET Core and .NET Framework into a single platform called ".NET" (sometimes called ".NET 5+" or ".NET 6/7/8").
- **Key Differences:**
  - .NET 5 and later are just called ".NET" (no "Core" in the name).
  - .NET 5+ continues cross-platform support and adds new features, performance improvements, and a unified runtime for all workloads.
  - .NET Core is no longer developed after 3.1; .NET 5+ is the future.

| Feature         | .NET Core (<=3.1)         | .NET 5 and Later           |
|----------------|---------------------------|----------------------------|
| Name           | .NET Core                  | .NET (5, 6, 7, 8, ...)     |
| Cross-platform | Yes                        | Yes                        |
| LTS            | 3.1 is LTS                 | Even versions are LTS      |
| Unified APIs   | Partial                    | Full (Web, Desktop, etc.)  |
| Future Support | No (after 3.1)             | Yes                        |

## Advantages of .NET Core
- **Cross-platform:** Runs on Windows, Linux, and macOS.
- **High performance:** Optimized for speed and scalability.
- **Open source:** Source code is available on GitHub.
- **Modular:** Use only the packages you need (via NuGet).
- **Unified development:** Build web, cloud, desktop, IoT, and mobile apps.
- **Side-by-side versioning:** Multiple versions can run on the same machine.
- **Modern tooling:** Supports Visual Studio, VS Code, CLI, and CI/CD.
- **Cloud-ready:** Designed for microservices and containerization (Docker, Kubernetes).

---

## Routing in .NET Core

**Routing** is the process of mapping incoming HTTP requests to the corresponding controller actions or endpoints in your application.

### Types of Routing
1. **Attribute Routing**
   - Define routes directly on controllers and actions using attributes.
   - Example from current code:
     ```csharp
     [ApiController]
     [Route("[controller]")]
     public class WeatherForecastController : ControllerBase
     {
         [HttpGet("message")]
         public string GetMessage() { ... }
     }
     ```
   - Here, `[Route("[controller]")]` means the route is `/WeatherForecast`, and `[HttpGet("message")]` maps to `/WeatherForecast/message`.

2. **Convention-based Routing**
   - Define routes in `Program.cs` or `Startup.cs` using route templates.
   - Example from current code:
     ```csharp
     app.MapControllers();
     ```
   - This enables attribute routing for all controllers.
   - You can also use:
     ```csharp
     app.MapGet("/hello", () => "Hello World!");
     ```
   - This maps the `/hello` route to a minimal API endpoint.

3. **Minimal APIs (from .NET 6+)**
   - Define routes directly in `Program.cs` using methods like `MapGet`, `MapPost`, etc.
   - Example:
     ```csharp
     app.MapGet("/ping", () => "pong");
     ```

### Example from Current Code
- In `WeatherForecastController.cs`:
  ```csharp
  [ApiController]
  [Route("[controller]")]
  public class WeatherForecastController : ControllerBase
  {
      [HttpGet("di-lifetimes")] // Route: /WeatherForecast/di-lifetimes
      public object GetDiLifetimes() { ... }
  }
  ```
- In `Program.cs`:
  ```csharp
  app.MapControllers(); // Enables attribute routing for controllers
  // app.MapGet("/hello", () => "Hello World!"); // Example of minimal API routing
  ```

### Summary
- Use **attribute routing** for fine-grained control on controllers/actions.
- Use **convention-based routing** for centralized route templates.
- Use **minimal APIs** for lightweight, function-based endpoints.

---

## In-Memory vs Distributed Caching in .NET Core

### In-Memory Caching
- Stores cache data in the memory of the application server (process memory).
- Fastest access (no network latency).
- Data is lost if the application restarts or scales out to multiple servers (each server has its own cache).
- Suitable for single-server or development scenarios.
- Implemented using `IMemoryCache` in .NET Core.

**Example:**
```csharp
// Register in Program.cs
builder.Services.AddMemoryCache();

// Usage in a service or controller
private readonly IMemoryCache _cache;
public MyService(IMemoryCache cache) => _cache = cache;

public string GetData()
{
    return _cache.GetOrCreate("myKey", entry => "cached value");
}
```

### Distributed Caching
- Stores cache data outside the application server (e.g., Redis, SQL Server, NCache).
- Data is shared across multiple servers/instances (good for cloud and scaled-out apps).
- Survives application restarts and enables consistent cache for all app instances.
- Implemented using `IDistributedCache` in .NET Core.

**Example:**
```csharp
// Register in Program.cs (for Redis)
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
});

// Usage in a service or controller
private readonly IDistributedCache _cache;
public MyService(IDistributedCache cache) => _cache = cache;

public async Task<string> GetDataAsync()
{
    var value = await _cache.GetStringAsync("myKey");
    if (value == null)
    {
        value = "cached value";
        await _cache.SetStringAsync("myKey", value);
    }
    return value;
}
```

### Comparison Table
| Feature           | In-Memory Cache         | Distributed Cache         |
|-------------------|------------------------|--------------------------|
| Storage Location  | App server memory      | External (Redis, etc.)   |
| Scalability       | Single server          | Multi-server/cloud       |
| Persistence       | Lost on restart        | Survives restarts        |
| Performance       | Fastest                | Fast (network involved)  |
| Use Case          | Dev, small apps        | Cloud, scaled-out apps   |

---

## Serilog and NLog: Structured Logging in .NET

### What is Serilog?
- **Serilog** is a popular structured logging library for .NET.
- It allows you to log events as structured data (not just plain text), making logs easier to search, filter, and analyze.
- Supports many "sinks" (outputs), such as console, files, databases, and cloud services.

### What is NLog?
- **NLog** is another widely used logging framework for .NET.
- It is highly configurable and supports various targets (file, database, email, etc.).
- Both Serilog and NLog are used for advanced logging scenarios, but Serilog is especially known for structured logging.

### Why Use Serilog?
- **Structured logs:** Store logs as key-value pairs (JSON, etc.) for better querying and analysis.
- **Multiple outputs:** Easily write logs to files, databases, cloud, etc.
- **Enrichers:** Add extra context (e.g., machine name, thread ID) to every log entry.
- **Easy integration:** Works with ASP.NET Core, .NET Console apps, and more.

### Basic Serilog Configuration Example
1. **Install NuGet packages:**
   - `Serilog.AspNetCore`
   - `Serilog.Sinks.Console` (or any other sink you need)

2. **Configure in Program.cs:**
   ```csharp
   using Serilog;

   Log.Logger = new LoggerConfiguration()
       .WriteTo.Console()
       .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
       .CreateLogger();

   var builder = WebApplication.CreateBuilder(args);
   builder.Host.UseSerilog();
   // ...existing code...
   var app = builder.Build();
   // ...existing code...
   ```

3. **Log in your code:**
   ```csharp
   private readonly ILogger<MyClass> _logger;
   public MyClass(ILogger<MyClass> logger) => _logger = logger;

   public void DoSomething()
   {
       _logger.LogInformation("Doing something at {Time}", DateTime.Now);
   }
   ```

### How Serilog Works
- You configure Serilog at application startup (in `Program.cs`).
- You specify one or more "sinks" (where logs go: console, file, etc.).
- You can enrich logs with additional context.
- Throughout your code, you use `ILogger<T>` to write logs.

### Summary
- **Serilog** and **NLog** are advanced logging frameworks for .NET.
- Serilog is especially useful for structured, queryable logs.
- Configuration is simple and flexible, supporting many output targets.
- Use logging to monitor, debug, and audit your applications effectively.

---

## File Uploads in .NET: Client and Server Example

### 1. Uploading a File from .NET Code (Client Side)

Below is a C# example that reads a file from disk and uploads it to an API endpoint using `HttpClient` and `MultipartFormDataContent`:

```csharp
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        string filePath = "sample.txt"; // Path to your file
        string apiUrl = "https://localhost:5001/api/upload"; // API endpoint

        using var httpClient = new HttpClient();
        using var form = new MultipartFormDataContent();
        using var fileStream = File.OpenRead(filePath);
        var fileContent = new StreamContent(fileStream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        form.Add(fileContent, "file", Path.GetFileName(filePath));

        HttpResponseMessage response = await httpClient.PostAsync(apiUrl, form);
        string result = await response.Content.ReadAsStringAsync();

        Console.WriteLine($"API Response: {result}");
    }
}
```

#### **Technical Details**
- The file is read from disk using `File.OpenRead`.
- `MultipartFormDataContent` is used to send the file as form data.
- The file is attached to the form with a field name (`"file"`).
- The request is sent to the API endpoint using `HttpClient.PostAsync`.

---

### 2. Receiving and Storing the File on the API (Server Side)

Here’s an example of an ASP.NET Core minimal API endpoint that accepts and saves the uploaded file:

```csharp
app.MapPost("/api/upload", async (IFormFile file) =>
{
    var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
    Directory.CreateDirectory(uploadsDir);

    var filePath = Path.Combine(uploadsDir, file.FileName);
    using var stream = new FileStream(filePath, FileMode.Create);
    await file.CopyToAsync(stream);

    return Results.Ok($"File {file.FileName} uploaded successfully!");
});
```

#### **Technical Details**
- The API receives the file as an `IFormFile` parameter.
- It ensures the `Uploads` directory exists.
- The file is saved to disk using a `FileStream`.
- A success message is returned to the client.

---

**Summary:**  
- The client reads and uploads the file using `HttpClient` and multipart form data.
- The server receives the file, saves it to disk, and responds with a status message.

---

## Difference Between `protected internal` and `private protected` in C#

| Modifier             | Accessible Within Same Assembly | Accessible in Derived Classes (Any Assembly) | Accessible in Derived Classes (Same Assembly) |
|----------------------|:------------------------------:|:--------------------------------------------:|:---------------------------------------------:|
| `protected internal` | Yes                            | Yes                                         | Yes                                          |
| `private protected`  | Yes                            | No                                          | Yes                                          |

### `protected internal`
- Accessible from any code in the same assembly (like `internal`).
- Also accessible from derived classes, even if they are in a different assembly (like `protected`).

### `private protected`
- Accessible only within the same assembly **and** only from derived classes.
- Not accessible from outside the assembly, even if inherited.

### Example
```csharp
public class Base
{
    protected internal int A; // Accessible in same assembly and in derived classes (any assembly)
    private protected int B;  // Accessible in same assembly, only in derived classes
}

public class Derived : Base
{
    void Test()
    {
        A = 1; // OK
        B = 2; // OK (same assembly)
    }
}

// In another assembly:
public class Other : Base
{
    void Test()
    {
        A = 1; // OK (protected internal)
        // B = 2; // Error: not accessible (private protected)
    }
}
```

---

## Difference Between `throw` and `throw ex` in C#

### `throw`
- Re-throws the current exception while preserving the original stack trace.
- Best practice for re-throwing exceptions in a catch block.
- Example:
  ```csharp
  try
  {
      // ... code ...
  }
  catch (Exception ex)
  {
      // ... logging or cleanup ...
      throw; // Preserves original stack trace
  }
  ```

### `throw ex`
- Throws the caught exception object, but **resets the stack trace** to the current line.
- Makes debugging harder because you lose the original error location.
- Example:
  ```csharp
  try
  {
      // ... code ...
  }
  catch (Exception ex)
  {
      // ... logging or cleanup ...
      throw ex; // Resets stack trace (not recommended)
  }
  ```

### Summary
- Use `throw` to preserve the original stack trace (recommended).
- Avoid `throw ex` unless you have a specific reason to reset the stack trace (rarely needed).

---

## Optimizing Bulk Updates: Changing Department for 1000 Employees

When you need to update a field (like department) for many records, making 1000 separate database calls is inefficient. Instead, use a bulk update approach.

### Entity Framework (EF Core)
- **Recommended:** Use a single query to update all records in one call.
- **Example (EF Core 7+):**
  ```csharp
  // Update all employees with IDs in the list to a new department
  var employeeIds = new List<int> { /* 1000 IDs */ };
  int newDeptId = 5;
  await dbContext.Employees
      .Where(e => employeeIds.Contains(e.Id))
      .ExecuteUpdateAsync(e => e.SetProperty(emp => emp.DepartmentId, newDeptId));
  ```
- **Older EF Core:**
  - Load all employees, update in memory, then call `SaveChanges()` (still one DB call, but less efficient for very large sets):
    ```csharp
    var employees = dbContext.Employees.Where(e => employeeIds.Contains(e.Id)).ToList();
    foreach (var emp in employees)
        emp.DepartmentId = newDeptId;
    dbContext.SaveChanges();
    ```

### ADO.NET
- **Recommended:** Use a single `UPDATE` SQL statement with a `WHERE` clause.
- **Example:**
  ```sql
  UPDATE Employees
  SET DepartmentId = @DeptId
  WHERE Id IN (SELECT Id FROM @Ids);
  ```
- **ADO.NET Code:**
  ```csharp
  using var cmd = new SqlCommand("UPDATE Employees SET DepartmentId = @DeptId WHERE Id IN (SELECT Id FROM @Ids)", connection);
  cmd.Parameters.AddWithValue("@DeptId", newDeptId);

  var tvpParam = cmd.Parameters.AddWithValue("@Ids", idTable);
  tvpParam.SqlDbType = SqlDbType.Structured;
  tvpParam.TypeName = "dbo.IdList";

  cmd.ExecuteNonQuery();
  ```

### ADO.NET with Table-Valued Parameter (TVP) for Bulk Update (SQL Server)
For very large sets of IDs, use a Table-Valued Parameter (TVP) to pass the list efficiently:

**1. Define a User-Defined Table Type in SQL Server:**
```sql
CREATE TYPE dbo.IdList AS TABLE (Id INT);
```

**2. Update Statement Using TVP:**
```sql
UPDATE Employees
SET DepartmentId = @DeptId
WHERE Id IN (SELECT Id FROM @Ids);
```

**3. ADO.NET Code to Use TVP:**
```csharp
// Prepare DataTable for TVP
var idTable = new DataTable();
idTable.Columns.Add("Id", typeof(int));
foreach (var id in employeeIds)
    idTable.Rows.Add(id);

using var cmd = new SqlCommand("UPDATE Employees SET DepartmentId = @DeptId WHERE Id IN (SELECT Id FROM @Ids)", connection);
cmd.Parameters.AddWithValue("@DeptId", newDeptId);

var tvpParam = cmd.Parameters.AddWithValue("@Ids", idTable);
tvpParam.SqlDbType = SqlDbType.Structured;
tvpParam.TypeName = "dbo.IdList";

cmd.ExecuteNonQuery();
```

**Summary:**
- TVPs are efficient for passing large lists to SQL Server.
- Reduces SQL injection risk and avoids string manipulation for ID lists.

---
