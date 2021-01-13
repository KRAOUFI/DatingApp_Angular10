using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace API.Interfaces.IServices
{
    public interface IPhotoService
    {
        Task<ImageUploadResult> AddPhotoInCloudinaryAsync(IFormFile file);
        Task<DeletionResult> DeletePhotoInCloudinaryAsync(string publicId);
        Task<PhotoDto> AddPhotoToUser(string username, ImageUploadResult image);

        Task<int> SetMainPhoto(string username, int photoId);
        Task<int> DeletePhoto(string username, int photoId);

    }
}