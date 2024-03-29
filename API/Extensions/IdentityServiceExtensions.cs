using System.Text;
using API.Services;
using Domain;
using Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing.Tree;
using Microsoft.IdentityModel.Tokens;
using Persistence;

namespace API.Extensions
{
    public static class IdentityServiceExtensions
    {
        // Identity Configuration
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
        {
            // Add Identity Service
            services.AddIdentityCore<AppUser>(opt =>
            {
                // Specifies that the password should not contain alphanumeric digits
                opt.Password.RequireNonAlphanumeric = false;
                // Specifies that the user must have a confirmed email to sign in
                opt.SignIn.RequireConfirmedEmail = true;
            })
            // joins identity tables and our tables
            .AddEntityFrameworkStores<DataContext>()
            // Adds the SignInManager service for handling sign-ins
            .AddSignInManager<SignInManager<AppUser>>()
            // Adds the default token providers for identity
            .AddDefaultTokenProviders();
            // generates the symmetric key to be used for signing the token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
            // add the authentication services
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opt =>
            {
                // TokenValidationParameters contain parameters for validating the JWT
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true, // specifies whether the signing key must be validated
                    IssuerSigningKey = key, // sets the signing key
                    ValidateIssuer = false, // does not validate the issuer
                    ValidateAudience = false, // does not validate the audience
                    ValidateLifetime = true,  // Validates the expiration time of the token
                    ClockSkew = TimeSpan.Zero // Sets the clock skew to zero, considering immediate expiration

                };
                // JwtBearerEvents is used to add a custom event
                opt.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        // Get the access token from the request query
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        // If the access token is present and the path starts with "/chat"
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chat"))
                        {
                            // Set the access token as the token
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });
            services.AddAuthorization(opt =>
            {
                // Add a custom authorization policy named "IsActivityHost"
                opt.AddPolicy("IsActivityHost", policy =>
                {
                    // Add the IsHostRequirement requirement to this policy
                    policy.Requirements.Add(new IsHostRequirement());
                });
            });
            // Add the IsHostRequirementHandler to handle the IsHostRequirement for each request
            services.AddTransient<IAuthorizationHandler, IsHostRequirementHandler>();
            // add the JWT Token service
            services.AddScoped<TokenService>();
            return services;
        }
    }
}