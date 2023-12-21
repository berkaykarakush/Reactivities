using Application.Interfaces;
using Application.Photos;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Infrastructure.Photos
{
    // PhotoAccessor class implementing the IPhotoAccessor interface
    public class PhotoAccessor : IPhotoAccessor
    {
        // Cloudinary object for accessing the Cloudinary service
        private readonly Cloudinary _cloudinary;
        // Constructor injecting CloudinarySettings configuration
        public PhotoAccessor(IOptions<CloudinarySettings> config)
        {
            // Create a Cloudinary object using Cloudinary account information
            var account = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );
            _cloudinary = new Cloudinary(account);
        }
        // Method to upload a given IFormFile type photo
        public async Task<PhotoUploadResult> AddPhoto(IFormFile file)
        {
            // Check if the file is not empty
            if (file.Length > 0)
            {
                // Stream object used for reading the file
                await using var stream = file.OpenReadStream();
                // Upload parameters to Cloudinary
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill")
                };
                // Upload the file to Cloudinary and get the result
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                // Check for errors during the upload
                if (uploadResult.Error != null)
                {
                    throw new Exception(uploadResult.Error.Message);
                }
                // If the upload is successful, return the result as a PhotoUploadResult object
                return new PhotoUploadResult
                {
                    PublicId = uploadResult.PublicId,
                    Url = uploadResult.SecureUrl.ToString()
                };
            }
            // If the file is empty, return null
            return null;
        }
        // Method to delete a photo from Cloudinary using the provided publicId
        public async Task<string> DeletePhoto(string publicId)
        {
            // Parameters for deletion from Cloudinary
            var deleteParams = new DeletionParams(publicId);
            // Delete the photo from Cloudinary and get the result
            var result = await _cloudinary.DestroyAsync(deleteParams);
            // If the deletion is successful, return "ok"; otherwise, return null
            return result.Result == "ok" ? result.Result : null;
        }
    }
}