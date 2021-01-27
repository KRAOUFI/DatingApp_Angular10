using System;
using System.Threading.Tasks;
using API.Extensions;
using API.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();
            
            // If not authenticated, continue without logging User activity
            if (!resultContext.HttpContext.User.Identity.IsAuthenticated) return;
            
            var userSrvc = resultContext.HttpContext.RequestServices.GetService<IUserService>();

            var userId = resultContext.HttpContext.User.GetUserId();
            await userSrvc.UpdateUserLastActivity(userId);
        }
    }
}
