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
    public class LikesController : BaseApiController
    {
        private readonly IUserService _userService;
        private readonly ILikesService _likesService;

        public LikesController(IUserService userService, ILikesService likesService)
        {
            _likesService = likesService;
            _userService = userService;

        }

        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username) 
        {
            var sourceUserId = User.GetUserId();
            var likedUser = await _userService.GetUserByUsernameAsync(username);
            switch(await _likesService.AddLike(sourceUserId, likedUser)) 
            {
                case "notfound":
                    return NotFound();

                case "You can not like yourself":
                    return BadRequest("You can not like yourself");

                case "You already like this member":
                    return BadRequest("You already like this member");

                case "ok":
                    return Ok();

                default:
                    return BadRequest("Failed to like user");
            }
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<LikeDto>>> GetUserLikes([FromQuery] LikesParams likesParams) 
        {
            likesParams.UserId =  User.GetUserId();
            
            var usersLikes = await _likesService.GetUserLikes(likesParams);
            
            Response.AddPaginationHeader(usersLikes.CurrentPage, usersLikes.PageSize, 
                usersLikes.TotalCount, usersLikes.TotalPages);

            return Ok(usersLikes);
        }

    }
}
