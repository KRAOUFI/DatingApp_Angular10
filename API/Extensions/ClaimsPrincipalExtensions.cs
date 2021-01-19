using System.Security.Claims;

namespace API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserName(this ClaimsPrincipal user) 
        {
            // FindFirst: Methode de ClaimsPrincipal qui renvoie le premier claim avec 
            // l'éléments spécifié. Ici nous demondons NameIdentifier 
            return user.FindFirst(ClaimTypes.Name)?.Value;
        } 

        public static int GetUserId(this ClaimsPrincipal user) 
        {
            // FindFirst: Methode de ClaimsPrincipal qui renvoie le premier claim avec 
            // l'éléments spécifié. Ici nous demondons NameIdentifier 
            return int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }        
    }
}