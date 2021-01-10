using API.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.IServices
{
    public interface IUserService
    {
        Task<IEnumerable<MemberDto>> GetUsersAsync();
        Task<MemberDto> GetUserAsync(int id);
        Task<MemberDto> GetUserByUsernameAsync(string username);
        Task<int> UpdateUser(string username, MemberUpdateDto dtoToUpdate);
    }
}
