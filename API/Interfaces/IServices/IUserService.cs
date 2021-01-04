using API.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.IServices
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetUsers();

        Task<User> GetUser(int id);
    }
}
