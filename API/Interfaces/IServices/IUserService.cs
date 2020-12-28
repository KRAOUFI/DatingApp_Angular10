using API.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.IServices
{
    public interface IUserService
    {
        Task<ActionResult<IEnumerable<User>>> GetUsers();

        Task<ActionResult<User>> GetUser(int id);
    }
}
