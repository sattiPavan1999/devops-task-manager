using Microsoft.EntityFrameworkCore;
using Npgsql;
using TaskManager.API.Data;
using TaskManager.API.Repositories;
using TaskManager.API.Services;

var builder = WebApplication.CreateBuilder(args);

var postgresHost = builder.Configuration["POSTGRES_HOST"] ?? builder.Configuration["POSTGRES_SERVER"];
string connectionString;
if (!string.IsNullOrWhiteSpace(postgresHost))
{
    var database = builder.Configuration["POSTGRES_DB"] ?? builder.Configuration["POSTGRES_DATABASE"]
        ?? throw new InvalidOperationException("POSTGRES_DB or POSTGRES_DATABASE must be configured.");
    var username = builder.Configuration["POSTGRES_USER"]
        ?? throw new InvalidOperationException("POSTGRES_USER must be configured.");
    var password = builder.Configuration["POSTGRES_PASSWORD"]
        ?? throw new InvalidOperationException("POSTGRES_PASSWORD must be configured.");

    if (!int.TryParse(builder.Configuration["POSTGRES_PORT"] ?? "5432", out var port))
    {
        throw new InvalidOperationException("POSTGRES_PORT must be a valid port number.");
    }

    connectionString = new NpgsqlConnectionStringBuilder
    {
        Host = postgresHost,
        Port = port,
        Database = database,
        Username = username,
        Password = password
    }.ConnectionString;
}
else
{
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException(
            "Configure ConnectionStrings:DefaultConnection or the POSTGRES_HOST, POSTGRES_DB, POSTGRES_USER, and POSTGRES_PASSWORD settings.");
}

builder.Services.AddDbContext<TaskDbContext>(options =>
    options.UseNpgsql(connectionString));

// 2. Dependency Injection
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ITaskService, TaskService>();

// 3. Controllers & Endpoints
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "Task Manager API", Version = "v1" });
});

// 4. CORS Policy for Frontend (Supports Vite local dev on 5173 and Docker container on 3000)
const string CorsPolicy = "FrontendPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: CorsPolicy, policy =>
    {
        policy.WithOrigins(
                  "http://localhost:5173", 
                  "http://localhost:3000"
              )
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// 5. Apply migrations before serving requests so startup fails if the database is unavailable.
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<TaskDbContext>();
        await dbContext.Database.MigrateAsync();

        if (app.Environment.IsDevelopment())
        {
            await DbInitializer.SeedAsync(dbContext);
        }

        app.Logger.LogInformation("Database migration completed successfully.");
    }
    catch (Exception ex)
    {
        app.Logger.LogCritical(ex, "Database migration failed; the application cannot start.");
        throw;
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/health/live", () => Results.Ok(new { status = "Healthy" }));
app.MapGet("/health/ready", async (TaskDbContext dbContext, CancellationToken cancellationToken) =>
    await dbContext.Database.CanConnectAsync(cancellationToken)
        ? Results.Ok(new { status = "Ready" })
        : Results.StatusCode(StatusCodes.Status503ServiceUnavailable));

app.UseCors(CorsPolicy);
app.UseAuthorization();
MapControllersEndpoints(app);

app.Run();

void MapControllersEndpoints(WebApplication webApp)
{
    webApp.MapControllers();
}