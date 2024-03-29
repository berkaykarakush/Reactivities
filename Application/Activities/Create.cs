using Application.Core;
using Application.Interfaces;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
    public class Create
    {
        // Command class representing the request for creating an activity
        public class Command : IRequest<Result<Unit>>
        {
            public Activity Activity { get; set; }
        }
        // Validator class for validating the Create Command
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                // Setting up a validation rule using the ActivityValidator for the Activity property
                RuleFor(c => c.Activity).SetValidator(new ActivityValidator());
            }
        }
        // Handler class for handling the creation of activities
        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;
            public Handler(DataContext context, IUserAccessor userAccessor)
            {
                _context = context;
                _userAccessor = userAccessor;
            }
            // Handling the creation of an activity based on the received command
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                // Retrieve the user from the database based on the username obtained from the UserAccessor
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == _userAccessor.GetUserName());
                // Create a new ActivityAttendee instance representing the host of the activity
                var attendee = new ActivityAttendee
                {
                    AppUser = user,
                    Activity = request.Activity,
                    IsHost = true
                };
                // Add the host (attendee) to the attendees list of the activity
                request.Activity.Attendees.Add(attendee);
                // Adding the provided activity to the context's list of activities
                _context.Activities.Add(request.Activity);
                // Saving changes to the database and checking if any changes were made
                var result = await _context.SaveChangesAsync() > 0;
                // Returning a success or failure result based on the database operation
                if (!result) return Result<Unit>.Failure("Failed to create activity");
                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}