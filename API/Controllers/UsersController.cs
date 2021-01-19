using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using API.DTOs;
using API.Extensions;
using API.Helpers;
using API.Interfaces.IServices;
using API.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserService _userService;
        private readonly IPhotoService _photoService;

        public UsersController(IUserService userService, IPhotoService photoService)
        {
            _photoService = photoService;
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<MemberDto>>> GetUsers([FromQuery]UserParams userParams)
        {
            var currentUser = await _userService.GetUserByUsernameAsync(User.GetUserName());
            var users = await _userService.GetUsersAsync(currentUser, userParams);
            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);
            return Ok(users);
        }

        [HttpGet("{username}", Name="GetUser")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            return await _userService.GetUserByUsernameAsync(username);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto dtoToUpdate)
        {
            return await _userService.UpdateUser(User.GetUserName(), dtoToUpdate) > 0 ?
                NoContent() :
                BadRequest("Failed to update");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var cloudinaryImageResult = await _photoService.AddPhotoInCloudinaryAsync(file);
            if (cloudinaryImageResult.Error != null) 
                return BadRequest(cloudinaryImageResult.Error.Message); 
            
            var username = User.GetUserName();
            var image = await _photoService.AddPhotoToUser(username, cloudinaryImageResult);
            return image != null ? 
                CreatedAtRoute("GetUser", new { username = username }, image) : 
                BadRequest("Problem adding photo to database");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var updated = await _photoService.SetMainPhoto(User.GetUserName(), photoId);
            return updated > 0 ? NoContent() : BadRequest("Failed to set main photo");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId) 
        {
            try
            {
                var deletion = await _photoService.DeletePhoto(User.GetUserName(), photoId);
                return Ok();
            }
            catch(Exception ex) 
            {
                switch(ex.Message) 
                {
                    case "BadRequest":
                        return BadRequest("You can not delete the main photo");
                    case "NotFound":
                        return NotFound();
                    default:
                        return BadRequest(ex.Message);
                }
                
            }
        }
    }
}
