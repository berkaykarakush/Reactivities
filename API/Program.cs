using System.Net;
using API.Extensions;
using API.Middleware;
using API.SignalR;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(opt => 
{
    // Creates an authorization policy builder to define authorization policies
    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    // Adds an authorization filter to the controller options with the specifies
    opt.Filters.Add(new AuthorizeFilter(policy));
});
// add services
builder.Services.AddApplicationServices(builder.Configuration);
// add identity 
builder.Services.AddIdentityServices(builder.Configuration);

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
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
// SignalR
app.MapHub<ChatHub>("/chat");
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
    // Attempt to retrieve the UserManager<AppUser> service from the service provider
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    // Seed the database with initial data using the SeedData method
    await Seed.SeedData(context, userManager);
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
