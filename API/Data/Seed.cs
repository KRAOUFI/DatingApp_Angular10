using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Seed
    {
        public static async Task SeedUsers(DataContext context) 
        {
            if(await context.Users.AnyAsync()) return;
            
            var userData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);
            foreach(var user in users)
            {
                user.UserName = user.UserName.ToLower();
                // Apr�s la mise en place de Identity, on ne initialise plus le mot de passe de cette mani�re
                //using var hmac = new HMACSHA512();
                //user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$word"));
                //user.PasswordSalt = hmac.Key;

                context.Users.Add(user);
            }
            await context.SaveChangesAsync();
        }
    }
}