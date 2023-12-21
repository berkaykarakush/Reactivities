using Application.Core;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Photos
{
    public class SetMain
    {
        // Command used to set the main photo, returns Result<Unit>
        public class Command : IRequest<Result<Unit>>
        {
            public string Id { get; set; }
        }
        // Handler used to set the main photo
        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;

            public Handler(DataContext context, IUserAccessor userAccessor)
            {
                _context = context;
                _userAccessor = userAccessor;
            }

            // Method handling the command to set the main photo
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                // Get the user from the database including their photos
                var user = await _context.Users.Include(u => u.Photos).FirstOrDefaultAsync(p => p.UserName == _userAccessor.GetUserName());
                // If the user is not found, return null
                if (user == null) return null;
                // Get the photo with the requested Id from the user's photo collection
                var photo = user.Photos.FirstOrDefault(u => u.Id == request.Id);
                // If the photo is not found, return null
                if (photo == null) return null;
                // Get the current main photo of the user
                var currentMain = user.Photos.FirstOrDefault(u => u.IsMain);
                // If there is a current main photo, unmark it as the main photo
                if (currentMain != null) currentMain.IsMain = false;
                // Mark the new photo as the main photo
                photo.IsMain = true;
                // Save changes to the database
                var success = await _context.SaveChangesAsync() > 0;
                // If the save is successful, return a success result
                if (success) return Result<Unit>.Success(Unit.Value);
                // If there is a problem during saving, return a failure result with an error message
                return Result<Unit>.Failure("Problem setting main photo");
            }
        }
    }
}