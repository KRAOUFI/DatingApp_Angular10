using API.Data;
using API.Helpers;
using API.Interfaces.IServices;
using API.IServices;
using API.Repositories;
using API.Services;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config) 
        {
            /**************** Injection de dépendance ****************/
            services.AddScoped<ITokenService, TokenService>();

            services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);
            
            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });

            services.AddScoped<UserRepository>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAccountService, AccountService>();
            /*********************************************************/

            return services;
        }
    }
}