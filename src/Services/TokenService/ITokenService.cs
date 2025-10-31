using pogadajmy_server.Models;

namespace pogadajmy_server.Services.TokenService
{
    public interface ITokenService
    {
        string CreateAccessToken(User user, TimeSpan expiresIn);
    }
}
