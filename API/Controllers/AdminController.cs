using System;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using API.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AdminController : BaseApiController
    {
        private readonly IAdminService _admnService;
        public AdminController(IAdminService admnService)
        {
            _admnService = admnService;
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("users-with-roles")]
        public async Task<ActionResult> GetUsersWithRoles()
        {
            var users = await _admnService.GetUsersWithRolesAsync();
            return Ok(users);
        }

        [HttpPost("edit-roles/{username}")]
        public async Task<ActionResult> EditRoles(string username, [FromQuery] string roles)
        {
            try
            {
                var value = await _admnService.EditRoles(username, roles);
                return Ok(value);
            }
            catch(Exception ex)
            {
                switch(ex.Message) 
                {
                    case "notfound":
                        return NotFound("Could not find a user");
                    default:
                        return BadRequest(ex.Message);
                }
            }
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photos-to-moderate")]
        public ActionResult GetPhotoForModeration()
        {
            return Ok("Admins or moderators can see this");
        }
    }
}