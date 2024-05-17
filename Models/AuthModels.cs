using System.ComponentModel.DataAnnotations;

namespace Cubichi.Models.Auth;

public class RegisterRequest
{
    [MinLength(3), MaxLength(50)]
    public required string UserName { get; set; }
    [EmailAddress]
    public required string Email { get; set; }
    [MinLength(6), MaxLength(50)]
    public required string Password { get; set; }
    [Compare("Password")]
    public required string ConfirmPassword { get; set; }
}

public class LoginRequest
{
    [MinLength(3), MaxLength(50)]
    public required string UserName { get; set; }
    [MinLength(6), MaxLength(50)]
    public required string Password { get; set; }
}

public class AuthResponse
{
    public required string Token { get; set; }
}