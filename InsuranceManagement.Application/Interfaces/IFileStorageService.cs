using Microsoft.AspNetCore.Http;

namespace InsuranceManagement.Application.Interfaces
{
    /// <summary>
    /// Abstraction for uploading files to persistent cloud storage.
    /// Implemented by CloudinaryStorageService in Infrastructure.
    /// </summary>
    public interface IFileStorageService
    {
        /// <summary>
        /// Uploads a file and returns the permanent public URL.
        /// </summary>
        /// <param name="file">The file to upload.</param>
        /// <param name="folder">Cloudinary folder name (e.g. "CarImages", "LifeClaims").</param>
        Task<string?> UploadAsync(IFormFile? file, string folder);
    }
}
