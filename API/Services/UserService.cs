using System.Collections.Generic;
using System.Threading.Tasks;
using API.Entities;
using API.IServices;
using API.Repositories;

namespace API.Services
{
    public class UserService : IUserService
    {
        private readonly UserRepository _userRepo;

        public UserService(UserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<IEnumerable<User>> GetUsers() 
        {
            return await _userRepo.ToListAsync();
        }

        public async Task<User> GetUser(int id)
        {
            return await _userRepo.FindByIdAsync(id);
        }

        
    }
}