using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add Swagger/OpenAPI services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TodoApi", Version = "v1" });
});

// Register MessageService for dependency injection.
// Whenever a controller or class requests IMessageService, an instance of MessageService will be provided.
builder.Services.AddScoped<IMessageService, MessageService>();

// Register ScopedService: New instance per HTTP request
builder.Services.AddScoped<IScopedService, ScopedService>();

// Register TransientService: New instance every time it is requested
builder.Services.AddTransient<ITransientService, TransientService>();

// Register SingletonService: Only one instance for the entire application lifetime
builder.Services.AddSingleton<ISingletonService, SingletonService>();

// Register UsersDbContext for dependency injection, using localdb for demonstration
builder.Services.AddDbContext<UsersDbContext>(options =>
    options.UseSqlite("Data Source=users.db")); // Using SQLite for local development

var app = builder.Build();  

// Enable Swagger middleware in development and production
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "TodoApi v1");
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
