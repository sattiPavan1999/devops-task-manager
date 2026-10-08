using Microsoft.EntityFrameworkCore;
using TaskManager.API.Data;
using TaskManager.API.Repositories;
using TaskManager.API.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Database Connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

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

// 5. Auto-Migrate and Seed Data
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<TaskDbContext>();

        // Automatically creates database and applies pending migrations if missing
        await dbContext.Database.MigrateAsync();

        // Seeds initial sample tasks once tables exist
        await DbInitializer.SeedAsync(dbContext);

        app.Logger.LogInformation("Database migration and seeding completed successfully.");
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "An error occurred while migrating or seeding the database.");
    }

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(CorsPolicy);
app.UseAuthorization();
MapControllersEndpoints(app);

app.Run();

void MapControllersEndpoints(WebApplication webApp)
{
    webApp.MapControllers();
}