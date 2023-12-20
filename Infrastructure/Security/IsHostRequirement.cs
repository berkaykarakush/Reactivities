using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Infrastructure.Security
{
    // Custom Authorization Requirement indicating that the user must be the host of the activity
    public class IsHostRequirement : IAuthorizationRequirement
    {

    }
    // Authorization Handler for the IsHostRequirement
    public class IsHostRequirementHandler : AuthorizationHandler<IsHostRequirement>
    {
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public IsHostRequirementHandler(DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsHostRequirement requirement)
        {
            // Get the user's ID from the claims
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            // If the user ID is not available, the requirement cannot be met
            if (userId == null) return Task.CompletedTask;
            // Get the activity ID from the route values in the HTTP context
            var activictyId = Guid.Parse(_httpContextAccessor.HttpContext?.Request.RouteValues.SingleOrDefault(x => x.Key == "id").Value?.ToString());
            // Query the database to check if the user is an attendee of the activity
            var attendee = _context.ActivityAttendees.AsNoTracking().SingleOrDefaultAsync(x => x.AppUserId == userId && x.ActivityId == activictyId).Result;
            // If the attendee is not found, the requirement cannot be met
            if (attendee == null) return Task.CompletedTask;
            // If the user is the host of the activity, succeed the authorization requirement
            if (attendee.IsHost) context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}