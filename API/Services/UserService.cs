using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Helpers;
using API.IServices;
using API.Repositories;
using AutoMapper;

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
            /* 1ère façon de requêter dans les entité User et Photo mais qui n'est pas bien optimisé */
            // var user = await _userRepo.GetUserByUsernameAsync(username);
            // return _mapper.Map<MemberDto>(user);
            
            /* 2nd façon de faire et qui est mieux optimisé en terme de requête SQL générée */
            return await _userRepo.GetMemberByUsernameAsync(username);
        }

        public async Task<PagedList<MemberDto>> GetUsersAsync(MemberDto currentUser, UserParams userParams) 
        {
            /* 1ère façon de requêter dans les entité User et Photo mais qui n'est pas bien optimisé */
            // var users = await _userRepo.GetUsersAsync();
            // return _mapper.Map<IEnumerable<MemberDto>>(users);

            /* 2nd façon de faire et qui est mieux optimisé en terme de requète SQL généré */
            //return await _userRepo.GetMembersAsync();

            /* 3ème façon de requeter en ajoutant la pagination */
            userParams.CurrentUsername = currentUser.Username;
            if (string.IsNullOrEmpty(userParams.Gender))
                userParams.Gender = currentUser.Gender == "male" ? "female" : "male";
            
            return await _userRepo.GetMembersAsync(userParams);
        }

        public async Task<int> UpdateUser(string username, MemberUpdateDto dtoToUpdate)
        {
            var user = await _userRepo.GetByConditionAsync(x => x.UserName == username);
            _mapper.Map(dtoToUpdate, user);
            return await _userRepo.UpdateAsync(user);
        }

        public async Task UpdateUserLastActivity(int userId)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            user.LastActive = DateTime.Now;
            await _userRepo.UpdateAsync(user);
        }
    }
}
