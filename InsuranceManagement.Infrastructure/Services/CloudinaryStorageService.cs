using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using InsuranceManagement.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace InsuranceManagement.Infrastructure.Services
{
    public class CloudinaryStorageService : IFileStorageService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryStorageService(IConfiguration config)
        {
            var cloudName = config["Cloudinary:CloudName"];
            var apiKey    = config["Cloudinary:ApiKey"];
            var apiSecret = config["Cloudinary:ApiSecret"];

            if (string.IsNullOrWhiteSpace(cloudName) ||
                string.IsNullOrWhiteSpace(apiKey)    ||
                string.IsNullOrWhiteSpace(apiSecret))
            {
                throw new InvalidOperationException(
                    "Cloudinary credentials are missing. Set Cloudinary:CloudName, Cloudinary:ApiKey, and Cloudinary:ApiSecret.");
            }

            var account = new Account(cloudName, apiKey, apiSecret);
            _cloudinary = new Cloudinary(account) { Api = { Secure = true } };
        }

        public async Task<string?> UploadAsync(IFormFile? file, string folder)
        {
            if (file == null || file.Length == 0)
                return null;

            await using var stream = file.OpenReadStream();

            var uploadParams = new RawUploadParams
            {
                File       = new FileDescription(file.FileName, stream),
                Folder     = folder,
                PublicId   = $"{folder}/{Guid.NewGuid()}_{Path.GetFileNameWithoutExtension(file.FileName)}",
                Overwrite  = false
            };

            // Use image upload for images, raw for documents/PDFs
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (ext is ".jpg" or ".jpeg" or ".png" or ".gif" or ".webp")
            {
                var imageParams = new ImageUploadParams
                {
                    File     = new FileDescription(file.FileName, stream),
                    Folder   = folder,
                    PublicId = uploadParams.PublicId,
                };
                // Need to reopen stream as it was consumed above
                stream.Seek(0, SeekOrigin.Begin);
                var imageResult = await _cloudinary.UploadAsync(imageParams);
                return imageResult.SecureUrl?.ToString();
            }

            // PDF / other raw files
            stream.Seek(0, SeekOrigin.Begin);
            var rawResult = await _cloudinary.UploadAsync(uploadParams);
            return rawResult.SecureUrl?.ToString();
        }
    }
}
