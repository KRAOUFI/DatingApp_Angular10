using API.Entities;

namespace API.Interfaces.IServices
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}