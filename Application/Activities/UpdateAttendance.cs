using System.Data;
using Application.Core;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
    public class UpdateAttendance
    {
        // Command for updating attendance
        public class Command : IRequest<Result<Unit>>
        {
            public Guid Id { get; set; }
        }
        // Handler for the UpdateAttendance command
        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;

            public Handler(DataContext context, IUserAccessor userAccessor)
            {
                _context = context;
                _userAccessor = userAccessor;
            }
            // Handling the UpdateAttendance command
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                // Retrieve the activity from the database, including attendees and their associated users
                var activity = await _context.Activities
                    .Include(a => a.Attendees)
                    .ThenInclude(a => a.AppUser)
                    .SingleOrDefaultAsync(x => x.Id == request.Id);
                // Return null if the activity is not found
                if (activity == null) return null;
                // Retrieve the current user from the database based on the username obtained from IUserAccessor
                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUserName());
                // Return null if the user is not found
                if (user == null) return null;
                // Get the username of the host (if any)
                var hostUsername = activity.Attendees.FirstOrDefault(x => x.IsHost)?.AppUser?.UserName;
                // Find the attendance record for the current user
                var attendance = activity.Attendees.FirstOrDefault(x => x.AppUser.UserName == user.UserName);
                // Toggle the IsCancelled property if the current user is the host
                if (attendance != null && hostUsername == user.UserName)
                    activity.IsCancelled = !activity.IsCancelled;
                // Remove the attendance record if the current user is not the host
                if (attendance != null && hostUsername != user.UserName)
                    activity.Attendees.Remove(attendance);
                // Add a new attendance record if there is no existing record
                if (attendance == null)
                {
                    attendance = new ActivityAttendee
                    {
                        AppUser = user,
                        Activity = activity,
                        IsHost = false
                    };
                    activity.Attendees.Add(attendance);
                }
                // Save changes to the database and check if any changes were made
                var result = await _context.SaveChangesAsync() > 0;
                // Return a success or failure result based on the database operation
                return result ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure("Problem updatin attendance");
            }
        }

    }
}