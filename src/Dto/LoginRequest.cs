using System.ComponentModel.DataAnnotations;

namespace pogadajmy_server.Dto
{
    public sealed class LoginRequest
    {
        [Required, MaxLength(100)] public string Name { get; init; } = default!;
        [Required, MaxLength(100)] public string Password { get; init; } = default!;
    }
}
