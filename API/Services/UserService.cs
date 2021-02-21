using System;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Helpers;
using API.Interfaces.IServices;
using API.Interfaces.IUnitOfWork;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<MemberDto> GetUserAsync(int id)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
            return _mapper.Map<MemberDto>(user);
        }

        public async Task<MemberDto> GetUserByUsernameAsync(string username)
        {
            var query = _unitOfWork.UserRepository.AsQueryable();
            return await query.Where(x => x.UserName == username.ToLower())
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }

        public async Task<PagedList<MemberDto>> GetUsersAsync(string currentUsername, UserParams userParams)
        {
            userParams.CurrentUsername = currentUsername;
            var gender = await _unitOfWork.UserRepository.GetUserGender(currentUsername);
            if (string.IsNullOrEmpty(userParams.Gender))
                userParams.Gender = gender == "male" ? "female" : "male";

            var query = _unitOfWork.UserRepository.AsQueryable();
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

        public async Task<bool> UpdateUser(string username, MemberUpdateDto dtoToUpdate)
        {
            var user = await _unitOfWork.UserRepository.GetByConditionAsync(x => x.UserName == username);
            _mapper.Map(dtoToUpdate, user);
            _unitOfWork.UserRepository.Update(user);
            return await _unitOfWork.Complete();
        }

        public async Task UpdateUserLastActivity(int userId)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            user.LastActive = DateTime.Now;
            _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.Complete();
        }
    }
}
