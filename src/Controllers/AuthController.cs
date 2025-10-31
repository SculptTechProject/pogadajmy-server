using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using pogadajmy_server.Infrastructure;
using pogadajmy_server.Models;
using pogadajmy_server.Services.TokenService;
using System.Security.Claims;
using LoginRequest = pogadajmy_server.Dto.LoginRequest;
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
        
        [AllowAnonymous]
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

        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequest body)
        {
            if (body is null || string.IsNullOrWhiteSpace(body.Name) || string.IsNullOrWhiteSpace(body.Password))
                return Unauthorized(Problem("Invalid credentials", 401, "Name or password is incorrect."));

            // case-insensitive po stronie Postgresa
            var user = await _db.User
                .AsNoTracking()
                .FirstOrDefaultAsync(u => EF.Functions.ILike(u.Name, body.Name));

            if (user is null || !BCrypt.Net.BCrypt.Verify(body.Password, user.PasswordHash))
                return Unauthorized(Problem("Invalid credentials", 401, "Name or password is incorrect."));

            var token = _tokens.CreateAccessToken(user, TimeSpan.FromHours(1));

            return Ok(new
            {
                token,
                user = new { id = user.Id, name = user.Name, city = user.City, pro = user.IsPro }
            });
        }

        private static ProblemDetails Problem(string title, int status, string detail) =>
            new() {
                Title = title,
                Status = status,
                Detail = detail,
                Type = "https://docs.pogadajmy.dev/errors/invalid_credentials"
            };

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
