var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Register MessageService for dependency injection.
// Whenever a controller or class requests IMessageService, an instance of MessageService will be provided.
builder.Services.AddScoped<IMessageService, MessageService>();

// Register ScopedService: New instance per HTTP request
builder.Services.AddScoped<IScopedService, ScopedService>();

// Register TransientService: New instance every time it is requested
builder.Services.AddTransient<ITransientService, TransientService>();

// Register SingletonService: Only one instance for the entire application lifetime
builder.Services.AddSingleton<ISingletonService, SingletonService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
