using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Seed
    {
    public static async Task SeedUsers(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager) 
        {
            // Si un utilisateur existe dans la table AppUser, alors ne rien faire
            if(await userManager.Users.AnyAsync()) return;
            
            //Sinon on commence à ajouter des utilisateurs à partir du fichier json
            var userData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);
            if(users == null) return; // Si je json est vide, on sort
            
            // On commence par la création des rôles
            var roles = new List<AppRole>()
            {
                new AppRole{ Name = "Member" },
                new AppRole{ Name = "Admin" },
                new AppRole{ Name = "Moderator" }
            };
            
            foreach(var role in roles)
            {
                await roleManager.CreateAsync(role);
            }

            foreach(var user in users)
            {
                user.UserName = user.UserName.ToLower();
                await userManager.CreateAsync(user, "Pa$$w0rd");
                await userManager.AddToRoleAsync(user, "Member");
            }

            var admin = new AppUser() { UserName = "Admin".ToLower() };
            await userManager.CreateAsync(admin, "AdminPa$$w0rd");
            await userManager.AddToRolesAsync(admin, new [] {"Admin", "Moderator"});
        }
    }
}