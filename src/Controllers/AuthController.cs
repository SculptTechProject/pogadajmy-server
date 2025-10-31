using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using pogadajmy_server.Infrastructure;
using pogadajmy_server.Models;
using pogadajmy_server.Services.TokenService;
using System.Security.Claims;
using RegisterRequest = pogadajmy_server.Dto.RegisterRequest;

namespace pogadajmy_server.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ITokenService _tokens;

        public AuthController(AppDbContext db, ITokenService tokens)
        {
            _db = db;
            _tokens = tokens;
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest body)
        {
            var name = body.Name.Trim().ToLowerInvariant();
            var user = await _db.User
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Name.ToLower() == name);

            if (user is not null)
            {
                return BadRequest(new ProblemDetails
                    {
                        Title = "User already exists",
                        Detail = "User already exists.",
                    }
                );
            }

            const int workFactor = 12;

            var password = body.Password;
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, workFactor: workFactor);
            var city = body.City;

            var newUser = new User
            {
                Name = name,
                PasswordHash = hashedPassword,
                City = city,
                IsPro = false
            };

            _db.User.Add(newUser);
            await _db.SaveChangesAsync();

            return Created("User created successfully.", new { newUser.Id, newUser.Name });
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequest body)
        {
            var name = body.Email.Trim().ToLowerInvariant();

            var user = await _db.User
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Name.ToLower() == name);

            if (user is null || !BCrypt.Net.BCrypt.Verify(body.Password, user.PasswordHash))
            {
                return Unauthorized(new ProblemDetails
                {
                    Title = "Invalid credentials",
                    Detail = "Name or password is incorrect.",
                    Status = StatusCodes.Status401Unauthorized,
                    Type = "https://docs.trily.dev/errors/invalid_credentials"
                });
            }

            var accessToken = _tokens.CreateAccessToken(user, TimeSpan.FromHours(2));

            return Ok(new
            {
                access_token = accessToken,
                token_type = "Bearer",
                expires_in = 1800
            });
        }

        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public IActionResult Me()
        {
            var id = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                     ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var name = User.FindFirst(JwtRegisteredClaimNames.Name)?.Value
                        ?? User.FindFirst(ClaimTypes.Name)?.Value;
            
            var city = User.FindFirst("locality")?.Value;
            
            var isPro = User.FindFirst("pro")?.Value == "true";
            
            return Ok(new { sub = id, name, city, isPro });
        }
    }
}
