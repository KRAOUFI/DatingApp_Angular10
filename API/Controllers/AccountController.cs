using System;
using System.Threading.Tasks;
using API.DTOs;
using API.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto dto) 
        {
            try
            {
                var user = await _accountService.RegisterAsync(dto);
                return user != null ? user : BadRequest("Username is already used");
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto dto) 
        {
            try 
            {
                var user = await _accountService.LoginAsync(dto);
                return user;
            } 
            catch (Exception ex) 
            {
                switch(ex.Message)
                {
                    case "unauthorized":
                        return Unauthorized();
                    default:
                    return BadRequest(ex.Message);
                }
                
            }
        }
    }
}