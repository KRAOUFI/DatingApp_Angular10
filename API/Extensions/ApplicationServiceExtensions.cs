using API.Data;
using API.Helpers;
using API.Interfaces.IServices;
using API.Interfaces.IUnitOfWork;
using API.Repositories;
using API.Services;
using API.SignalR;
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
            services.AddSingleton<PresenceTracker>();
            
            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));

            services.AddScoped<ITokenService, TokenService>();

            services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);
            
            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });

            /*
            services.AddScoped<UserRepository>();
            services.AddScoped<PhotoRepository>();
            services.AddScoped<LikesRepository>();
            services.AddScoped<ConnectionRepository>();
            services.AddScoped<GroupRepository>();
            services.AddScoped<MessagesRepository>();
            */
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IPhotoService, PhotoService>();
            services.AddScoped<ILikesService, LikesService>();
            services.AddScoped<IMessagesService, MessagesService>();

            services.AddScoped<LogUserActivity>();
            /*********************************************************/

            return services;
        }
    }
}