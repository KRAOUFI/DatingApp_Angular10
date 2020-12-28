using System.Collections.Generic;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using API.IServices;
using API.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class UserService : IUserService
    {
        private readonly UserRepository _userRepo;

        public UserService(UserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<ActionResult<IEnumerable<User>>> GetUsers() 
        {
            return await _userRepo.ToListAsync();
        }

        public async Task<ActionResult<User>> GetUser(int id)
        {
            return await _userRepo.FindByIdAsync(id);
        }

        
    }
}