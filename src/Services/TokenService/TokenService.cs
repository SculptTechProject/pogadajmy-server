using Microsoft.IdentityModel.Tokens;
using Npgsql.EntityFrameworkCore.PostgreSQL.Storage.Internal.Mapping;
using pogadajmy_server.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace pogadajmy_server.Services.TokenService
{
    public sealed class TokenService : ITokenService
    {
        private readonly string _key;
        private readonly string _issuer;
        private readonly string _aud;
        
        public TokenService(IConfiguration configuration)
        {
            _key    = Environment.GetEnvironmentVariable("TRILY_JWT_KEY")!;
            _issuer = Environment.GetEnvironmentVariable("TRILY_JWT_ISSUER") ?? "trily";
            _aud    = Environment.GetEnvironmentVariable("TRILY_JWT_AUDIENCE") ?? "trily-api";
        }

        public string CreateAccessToken(User u, TimeSpan lifetime)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, u.Id.ToString()),
                new(JwtRegisteredClaimNames.Name, u.Name ?? string.Empty),
                new(ClaimTypes.Locality, u.City ?? string.Empty),
                new("pro", u.IsPro.ToString().ToLower()),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var creds = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key)),
                SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(
                issuer: _issuer,
                audience: _aud,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.Add(lifetime),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}
