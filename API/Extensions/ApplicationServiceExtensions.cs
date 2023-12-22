using Application.Activities;
using Application.Core;
using Application.Interfaces;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure.Photos;
using Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddDbContext<DataContext>(opt =>
            {
                // Retrieve the connection string from the configuration to set up the database connection.
                opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
                // opt.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking); // Set tracking behavior
                // opt.EnableDetailedErrors(); // Enable detailed error messages
                // opt.UseLazyLoadingProxies(); // Enable lazy loading proxies
            });
            // add CORS policiy
            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy", policy =>
                {
                    policy.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins("http://localhost:3000","https://localhost:3000");
                });
            });
            // add CQRS 
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(List.Handler).Assembly));
            // add AutoMapper
            services.AddAutoMapper(typeof(MappingProfiles).Assembly);
            // add FluentValidation
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<Create>();
            // add HttpContextAccessor
            services.AddHttpContextAccessor();
            services.AddScoped<IUserAccessor, UserAccessor>();
            // add cloudinary
            services.Configure<CloudinarySettings>(config.GetSection("Cloudinary"));
            services.AddScoped<IPhotoAccessor, PhotoAccessor>();
            // add SignalR
            services.AddSignalR();
            return services;
        }
    }
}