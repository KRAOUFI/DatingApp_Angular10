using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;

namespace API.Interfaces.IServices
{
    public interface IAdminService
    {
        Task<IEnumerable<UserRoleDto>> GetUsersWithRolesAsync();
        Task<IEnumerable<string>> EditRoles(string username, string roles);
    }
}