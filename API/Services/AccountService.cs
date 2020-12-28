using API.DTOs;
using API.Entities;
using API.Interfaces.IServices;
using API.IServices;
using API.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace API.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserRepository _userRepo;
        private readonly ITokenService _tokenservice;

        public AccountService(
            UserRepository userRepo, 
            ITokenService tokenservice)
        {
            _userRepo = userRepo;
            _tokenservice = tokenservice;
        }

        public async Task<ActionResult<UserDto>> RegisterAsync(RegisterDto dto)
        {
            if (await _userRepo.UserExistAsync(dto.Username)) return null;
            
            using var hmac = new HMACSHA512();
            var user = new User
            {
                UserName = dto.Username.ToLower(),
                PaswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password)),
                PaswordSalt = hmac.Key
            };

            await _userRepo.CreateAsync(user);

            return new UserDto { Username = user.UserName, Token = _tokenservice.CreateToken(user) } ;
        }

        public async Task<ActionResult<UserDto>> LoginAsync(LoginDto dto)
        {
            try 
            {
                var user = await _userRepo.FindUserAsync(dto.Username);
                if (user == null) {
                    throw new Exception("Invalid username");
                }

                using var hmac = new HMACSHA512(user.PaswordSalt);
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password));
                
                for(int i = 0; i < computedHash.Length; i++) {
                    if (computedHash[i] != user.PaswordHash[i]) {
                        throw new Exception("Invalid password");
                    }
                }

                return new UserDto {Username = user.UserName, Token = _tokenservice.CreateToken(user) };
            } 
            catch (Exception ex) 
            {
                throw ex;
            }
        }
    }
}