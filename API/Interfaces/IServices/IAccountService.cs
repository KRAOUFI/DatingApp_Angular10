using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API.IServices
{
    public interface IAccountService
    {
        Task<ActionResult<UserDto>> RegisterAsync(RegisterDto dto);
        Task<ActionResult<UserDto>> LoginAsync(LoginDto dto);
    }
}
