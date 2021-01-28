using API.Entities;
using API.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API.Controllers
{
    public class BuggyController : BaseApiController
    {
        private readonly IUserService _userService;

        public BuggyController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize]
        [HttpGet("auth")]
        public ActionResult<string> GetSecret() {
            return "secret text";
        }
        
        [HttpGet("not-found")]
        public async Task<ActionResult<AppUser>> GetNotFoundSecret()
        {
            var thing = await _userService.GetUserAsync(-1);
            if (thing == null) return NotFound();
            return Ok(thing);
        }
        
        [HttpGet("server-error")]
        public async Task<ActionResult<string>> GetServerError()
        {            
            var thing = await _userService.GetUserAsync(-1);
            var thingtoreturn = thing.ToString();
            return thingtoreturn;
        }
        
        [HttpGet("bad-request")]
        public ActionResult<string> GetBadRequest()
        {
            return BadRequest("This was not a good request.");
        }

    }
}