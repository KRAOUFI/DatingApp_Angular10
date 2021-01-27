using API.DTOs;
using API.Entities;
using API.Helpers;
using System.Threading.Tasks;

namespace API.Interfaces.IServices
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
