using Application.Photos;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces
{
    public interface IPhotoAccessor
    {
        // Method to add a photo to the storage
        Task<PhotoUploadResult> AddPhoto(IFormFile file);
        // Method to delete a photo from the storage based on its public ID
        Task<string> DeletePhoto(string publicId);
    }
}