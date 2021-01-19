using API.DTOs;
using API.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.IServices
{
    public interface IUserService
    {
        Task<PagedList<MemberDto>> GetUsersAsync(MemberDto currentUser, UserParams userParams);
        Task<MemberDto> GetUserAsync(int id);
        Task<MemberDto> GetUserByUsernameAsync(string username);
        Task<int> UpdateUser(string username, MemberUpdateDto dtoToUpdate);
        Task UpdateUserLastActivity(int userId);
    }
}
