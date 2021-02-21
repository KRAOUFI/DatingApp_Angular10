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
        Task<bool> SetMainPhoto(string username, int photoId);
        Task<bool> DeletePhoto(string username, int photoId);

    }
}