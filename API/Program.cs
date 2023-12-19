using API.Extensions;
using API.Middleware;
using Microsoft.EntityFrameworkCore;
using Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.

// add custom exception middleware
app.UseMiddleware<ExceptionMiddleware>();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// use the policy
app.UseCors("CorsPolicy");
app.UseAuthorization();

app.MapControllers();

// Create a scope for dependency injection to manage the lifetime of services
using var scope = app.Services.CreateScope();
// Get the service provider from the created scope
var services = scope.ServiceProvider;
try
{
    // Attempt to retrieve the DataContext service from the service provider
    var context = services.GetRequiredService<DataContext>();
    // Apply any pending migrations asynchronously to the database
    await context.Database.MigrateAsync();
    // Seed the database with initial data using the SeedData method
    await Seed.SeedData(context);
}
catch (Exception ex)
{
    // If an exception occurs during migration or seeding, log the error using a logger
    // Get the logger service for the Program class from the service provider
    var logger = services.GetRequiredService<ILogger<Program>>();
    // Log the exception with an error message
    logger.LogError(ex, "An error occured during migration");
}

app.Run();
