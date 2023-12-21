using Application.Core;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Photos
{
    public class Add
    {
        // Command for adding a photo, returns Result<Photo>
        public class Command : IRequest<Result<Photo>>
        {
            public IFormFile File { get; set; }
        }
        public class Handler : IRequestHandler<Command, Result<Photo>>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;
            private readonly IPhotoAccessor _photoAccessor;

            public Handler(DataContext context, IUserAccessor userAccessor, IPhotoAccessor photoAccessor)
            {
                _context = context;
                _userAccessor = userAccessor;
                _photoAccessor = photoAccessor;
            }
            // Handling the photo addition command
            public async Task<Result<Photo>> Handle(Command request, CancellationToken cancellationToken)
            {
                // Get the user from the database including their photos
                var user = await _context.Users
                    .Include(u => u.Photos)
                    .FirstOrDefaultAsync(u => u.UserName == _userAccessor.GetUserName());
                // If the user is not found, return null
                if (user == null) return null;
                // Add the photo to Cloudinary and get the result
                var photoUploadResult = await _photoAccessor.AddPhoto(request.File);
                // Create a Photo entity with the obtained URL and PublicId
                var photo = new Photo
                {
                    Url = photoUploadResult.Url,
                    Id = photoUploadResult.PublicId
                };
                // If the user has no main photo, set the added photo as the main photo
                if (!user.Photos.Any(p => p.IsMain)) photo.IsMain = true;
                // Add the photo to the user's photo collection
                user.Photos.Add(photo);
                // Save changes to the database
                var result = await _context.SaveChangesAsync() > 0;
                // If the changes are saved successfully, return a success result with the added photo
                if (result) return Result<Photo>.Success(photo);
                // If there is a problem saving the photo, return a failure result with an error message
                return Result<Photo>.Failure("Problem adding photo");
            }
        }
    }
}