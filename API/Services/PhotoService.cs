using System;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces.IServices;
using API.Repositories;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace API.Services
{    
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary _cloudinary;
        private readonly PhotoRepository _photoRepo;
        private readonly UserRepository  _userRepository;
        private readonly IMapper _mapper;

        public PhotoService(
            IOptions<CloudinarySettings> config, 
            UserRepository userRepository,
            PhotoRepository photoRepo, 
            IMapper mapper)
        {
            var acc = new Account
            (
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );
            _cloudinary = new Cloudinary(acc);
            _userRepository = userRepository;
            _photoRepo = photoRepo;
            _mapper = mapper;
        }

        public async Task<PhotoDto> AddPhotoToUser(string username, ImageUploadResult image)
        {
            var user = await _userRepository.GetUserByUsernameAsync(username);
            var photo = new Photo 
            {
                Url = image.SecureUrl.AbsoluteUri,
                PublicId = image.PublicId,
                IsMain = user.Photos.Count == 0 ? true : false
            };
            user.Photos.Add(photo);
            
            return await _userRepository.UpdateAsync(user) > 0 ? _mapper.Map<PhotoDto>(photo) : null;
        }

        public async Task<int> SetMainPhoto(string username, int photoId) 
        {
            var user = await _userRepository.GetUserByUsernameAsync(username);
            
            var photoNotMain = user.Photos.FirstOrDefault(x=>x.IsMain == true);
            if(photoNotMain != null) photoNotMain.IsMain = false;

            var photoToBeMain = user.Photos.FirstOrDefault(x=>x.Id == photoId);
            if(photoToBeMain != null) photoToBeMain.IsMain = true;
            
            return await _userRepository.UpdateAsync(user);
        }

        public async Task<int> DeletePhoto(string username, int photoId) 
        {
            try 
            {
                var user = await _userRepository.GetUserByUsernameAsync(username);
                var photoToDelete = user.Photos.FirstOrDefault(x=>x.Id == photoId);
                
                if (photoToDelete == null) 
                    throw new Exception("NotFound");
                
                if (photoToDelete.IsMain) 
                    throw new Exception("BadRequest");
                
                if (photoToDelete.PublicId != null) 
                {
                    var deletionResult = await this.DeletePhotoInCloudinaryAsync(photoToDelete.PublicId);
                    if (deletionResult.Error != null) 
                        throw new Exception(deletionResult.Error.Message);
                }
                
                user.Photos.Remove(photoToDelete);
                return await _userRepository.UpdateAsync(user);
            } 
            catch(Exception ex) 
            {
                throw ex;
            }            
        }

        public async Task<ImageUploadResult> AddPhotoInCloudinaryAsync(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();
            if (file.Length > 0) {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams 
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face")
                };
                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }
            return uploadResult;
        }

        public async Task<DeletionResult> DeletePhotoInCloudinaryAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);

            return await _cloudinary.DestroyAsync(deleteParams);
        }
    }
}