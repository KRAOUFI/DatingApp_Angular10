using System.Threading.Tasks;
using API.Entities;

namespace API.Interfaces.IServices
{
    public interface ITokenService
    {
        Task<string> CreateToken(AppUser user);
    }
}