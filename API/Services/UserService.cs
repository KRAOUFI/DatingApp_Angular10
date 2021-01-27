using System;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces.IServices;
using API.Repositories;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class UserService : IUserService
    {
        private readonly UserRepository _userRepo;
        private readonly IMapper _mapper;

        public UserService(UserRepository userRepo, IMapper mapper)
        {
            _userRepo = userRepo;
            _mapper = mapper;
        }

        public async Task<MemberDto> GetUserAsync(int id)
        {
            var user =  await _userRepo.GetByIdAsync(id);
           return _mapper.Map<MemberDto>(user);
        }

        public async Task<MemberDto> GetUserByUsernameAsync(string username)
        {
            var query = _userRepo.AsQueryable();            
            return await query.Where(x => x.UserName == username.ToLower())
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }

        public async Task<PagedList<MemberDto>> GetUsersAsync(MemberDto currentUser, UserParams userParams) 
        {
            userParams.CurrentUsername = currentUser.Username;
            if (string.IsNullOrEmpty(userParams.Gender))
                userParams.Gender = currentUser.Gender == "male" ? "female" : "male";

            var query = _userRepo.AsQueryable();
            query = query.Where(u => u.UserName != userParams.CurrentUsername);
            query = query.Where(u => u.Gender == userParams.Gender);
            
            var minDateOfBirth = DateTime.Today.AddYears(-userParams.MaxAge - 1);
            var maxDateOfBirth = DateTime.Today.AddYears(-userParams.MinAge);
            query = query.Where(u => u.DateOfBirth >= minDateOfBirth && u.DateOfBirth <= maxDateOfBirth);

            query = userParams.OrderBy switch 
            {
                "created" => query.OrderByDescending(u => u.Created), 
                _ => query.OrderByDescending(u => u.LastActive)
            };
            
            var members = query.ProjectTo<MemberDto>(_mapper.ConfigurationProvider).AsNoTracking();
            return await PagedList<MemberDto>.CreateAsync(members, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<int> UpdateUser(string username, MemberUpdateDto dtoToUpdate)
        {
            var user = await _userRepo.GetByConditionAsync(x => x.UserName == username);
            _mapper.Map(dtoToUpdate, user);
            _userRepo.Update(user);
            return await _userRepo.SaveAsync(); 
        }

        public async Task UpdateUserLastActivity(int userId)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            user.LastActive = DateTime.Now;
            _userRepo.Update(user);
            await _userRepo.SaveAsync();
        }
    }
}
