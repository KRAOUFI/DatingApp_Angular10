using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            var users = await _userService.GetUsersAsync();
            return Ok(users);
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            return await _userService.GetUserByUsernameAsync(username);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto dtoToUpdate) {
            // FindFirst: Methode de ClaimsPrincipal qui renvoie le premier claim avec 
            // l'éléments spécifié. Ici nous demondons NameIdentifier 
            var username = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            return await _userService.UpdateUser(username,dtoToUpdate) > 0 ? 
                NoContent() : 
                BadRequest("Failed to update");
        }
    }
}
