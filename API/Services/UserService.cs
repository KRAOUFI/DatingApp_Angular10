using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
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

        public async Task<IEnumerable<MemberDto>> GetUsersAsync() 
        {
            /* 1ère façon de requêter dans les entité User et Photo mais qui n'est pas bien optimisé */
            // var users = await _userRepo.GetUsersAsync();
            // return _mapper.Map<IEnumerable<MemberDto>>(users);

            /* 2nd façon de faire et qui est mieux optimisé en terme de requète SQL généré */
            return await _userRepo.GetMembersAsync();
        }
    }
}
