using API.DTOs;
using System.Threading.Tasks;

namespace API.Interfaces.IServices
{
    public interface IAccountService
    {
        Task<UserDto> RegisterAsync(RegisterDto dto);
        Task<UserDto> LoginAsync(LoginDto dto);
    }
}
