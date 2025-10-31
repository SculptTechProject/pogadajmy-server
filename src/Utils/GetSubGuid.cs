using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace pogadajmy_server.Utils
{
    public static class ClaimsExtensions
    {
        public static Guid GetSubGuid(this ClaimsPrincipal user)
        {
            var sub = user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                      ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(sub, out var id)) throw new UnauthorizedAccessException("invalid sub");
            return id;
        }
    }
}
