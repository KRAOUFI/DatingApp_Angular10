using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces.IServices;
using API.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace API.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserRepository _userRepo;
        private readonly UserManager<AppUser> _userMngr;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenservice;

        public AccountService(
            UserManager<AppUser> userMngr,
            SignInManager<AppUser> signInManager,
            UserRepository userRepo,
            IMapper mapper,
            ITokenService tokenservice)
        {
            _userRepo = userRepo;
            _userMngr = userMngr;
            _signInManager = signInManager;
            _mapper = mapper;
            _tokenservice = tokenservice;
        }

        public async Task<UserDto> RegisterAsync(RegisterDto dto)
        {
            try 
            {
                if (await UserExists(dto.Username.ToLower())) return null;

                var user = _mapper.Map<AppUser>(dto);
                user.UserName = dto.Username.ToLower();
                
                // Créer le user avec son mot de passe
                var result = await _userMngr.CreateAsync(user, dto.Password);
                if(!result.Succeeded) throw new Exception(result.Errors.IdentityErrorsToString());
                
                // Affecter le user enregistré au role "Member"
                var roleResult = await _userMngr.AddToRoleAsync(user, "Member");
                if(!roleResult.Succeeded) throw new Exception(roleResult.Errors.IdentityErrorsToString());

                return new UserDto 
                {
                    Username = user.UserName,
                    Token = await _tokenservice.CreateToken(user),
                    KnownAs = user.KnownAs,
                    Gender = user.Gender
                };
            } 
            catch 
            {
                throw;
            }
        }

        public async Task<UserDto> LoginAsync(LoginDto dto)
        {
            try 
            {
                var user = await _userMngr.Users
                    .Include(p => p.Photos)
                    .SingleOrDefaultAsync(x => x.UserName == dto.Username.ToLower());
                
                if (user == null) throw new Exception("Invalid username");
                
                var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
                if(!result.Succeeded) throw new Exception("unauthorized");

                return new UserDto 
                {
                    Username = user.UserName, 
                    PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain == true)?.Url, 
                    KnownAs = user.KnownAs,
                    Gender = user.Gender,
                    Token = await _tokenservice.CreateToken(user) 
                };
            } 
            catch
            {
                throw;
            }
        }

        private async Task<bool> UserExists(string username)
        {
            return await _userMngr.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}