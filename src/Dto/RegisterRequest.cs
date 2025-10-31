using System.ComponentModel.DataAnnotations;

namespace pogadajmy_server.Dto
{
    public sealed class RegisterRequest
    {
        [Required, MaxLength(100)] public string Name { get; init; } = default!;
        [Required, MaxLength(100)] public string Password { get; init; } = default!;
        [MaxLength(50)] public string? City { get; init; }
    }
}
