using Application.Core;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Photos
{
    public class Delete
    {
        // Command used to delete a photo, returns Result<Unit>
        public class Command : IRequest<Result<Unit>>
        {
            public string Id { get; set; }
        }
        // Handler used to delete a photo
        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly IPhotoAccessor _photoAccessor;
            private readonly IUserAccessor _userAccessor;

            public Handler(DataContext context, IPhotoAccessor photoAccessor, IUserAccessor userAccessor)
            {
                _context = context;
                _photoAccessor = photoAccessor;
                _userAccessor = userAccessor;
            }
            // Method handling the command to delete a photo
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                // Get the user from the database including their photos
                var user = await _context.Users
                    .Include(u => u.Photos)
                    .FirstOrDefaultAsync(p => p.UserName == _userAccessor.GetUserName());
                // If the user is not found, return null
                if (user == null) return null;
                // Get the photo with the requested Id from the user's photo collection
                var photo = user.Photos.FirstOrDefault(u => u.Id == request.Id);
                // If the photo is not found, return null
                if (photo == null) return null;
                // Check if the photo is the main photo, and if so, return an error
                if (photo.IsMain) return Result<Unit>.Failure("You cannot delete your main photo");
                // Delete the photo from Cloudinary using the PhotoAccessor
                var result = await _photoAccessor.DeletePhoto(request.Id);
                // If deletion from Cloudinary fails, return an error result
                if (result == null) return Result<Unit>.Failure("Problem deleting photo drom Cloudinary");
                // Remove the photo from the user's photo collection
                user.Photos.Remove(photo);
                // Save changes to the database
                var success = await _context.SaveChangesAsync() > 0;
                // If the save is successful, return a success result
                if (success) return Result<Unit>.Success(Unit.Value);
                // If there is a problem during saving, return an error result
                return Result<Unit>.Failure("Problem deleting photo from API");
            }
        }
    }
}