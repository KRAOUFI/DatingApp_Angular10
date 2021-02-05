using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class AdminService : IAdminService
    {
        private readonly UserManager<AppUser> _userMngr;
        
        public AdminService(UserManager<AppUser> userMngr)
        {
            _userMngr = userMngr;
        }

        public async Task<IEnumerable<UserRoleDto>> GetUsersWithRolesAsync()
        {
            var users = await _userMngr.Users
                .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
                .OrderBy(u => u.UserName)
                .Select(u => new UserRoleDto
                {
                    Id = u.Id,
                    Username = u.UserName,
                    Roles = u.UserRoles.Select(r=>r.Role.Name).ToList()
                })
                .ToListAsync();

            return users;
        }

        public async Task<IEnumerable<string>> EditRoles(string username, string roles)
        {
            try 
            {
                var rolesToSetToUser = roles.Split(",").ToArray();
            
                var user = await _userMngr.FindByNameAsync(username);
                if(user == null) throw new Exception("notfound");
                var userRoles = await _userMngr.GetRolesAsync(user);
                
                var result = await _userMngr.AddToRolesAsync(user, rolesToSetToUser.Except(userRoles));
                if(!result.Succeeded) throw new Exception("Failed to add to roles");
                
                result = await _userMngr.RemoveFromRolesAsync(user, userRoles.Except(rolesToSetToUser));
                if(!result.Succeeded) throw new Exception("Failed to remove from roles");
                
                return await _userMngr.GetRolesAsync(user);
            } 
            catch (Exception ex)
            {
                throw ex;
            }
            
        }
    }
}