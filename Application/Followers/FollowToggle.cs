using Application.Core;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Followers
{
    public class FollowToggle
    {
        // Command class to handle the follow/unfollow operation
        public class Command : IRequest<Result<Unit>>
        {
            public string TargetUsername { get; set; }
        }
        // Handler class to implement the follow/unfollow logic
        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;

            public Handler(DataContext context, IUserAccessor userAccessor)
            {
                _context = context;
                _userAccessor = userAccessor;
            }
            // Handle method to execute the follow/unfollow logic
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                // Get the observer (the user initiating the follow/unfollow) from the database
                var observer = await _context.Users.FirstOrDefaultAsync(u => u.UserName == _userAccessor.GetUserName());
                if (observer == null) return null;
                // Get the target user to follow/unfollow from the database
                var target = await _context.Users.FirstOrDefaultAsync(u => u.UserName == request.TargetUsername);
                if (target == null) return null;
                // Check if there is an existing following relationship between the observer and target
                var following = await _context.UserFollowings.FindAsync(observer.Id, target.Id);
                // If there is no existing following relationship, create a new one; otherwise, remove it
                if (following == null)
                {
                    following = new UserFollowing
                    {
                        Observer = observer,
                        Target = target
                    };
                    _context.UserFollowings.Add(following);
                }
                else
                {
                    _context.UserFollowings.Remove(following);
                }
                // Save changes to the database and check if the operation was successful
                var success = await _context.SaveChangesAsync() > 0;
                // Return a success result or a failure result based on the operation's success
                if (success) return Result<Unit>.Success(Unit.Value);
                return Result<Unit>.Failure("Failed to update following");
            }
        }

    }
}