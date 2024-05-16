using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Cubichi.DataBase;
using Cubichi.Models;
using Cubichi.Models.Auth;
using Microsoft.IdentityModel.Tokens;

namespace Cubichi.Services;

public class AuthService : IAuthService
{
    private readonly IDataBaseInteractor _dataBaseInteractor;

    public AuthService(IDataBaseInteractor dataBaseInteractor)
    {
        _dataBaseInteractor = dataBaseInteractor;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        // Get the user from the database by username and verify the password
        var user = await _dataBaseInteractor.GetUserAsync(request.UserName);

        if (user == null || !VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
        {
            throw new InvalidOperationException("No user with this username or password");
        }

        // Generate a JWT token
        var token = GenerateJwtToken(user);

        return new AuthResponse
        {
            Token = (string)token
        };
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        // Check if user with this username or email exists 
        var user = await _dataBaseInteractor.GetUserAsync(request.UserName);
        if (user != null)
        {
            throw new InvalidOperationException("User with this username already exists");
        }

        user = await _dataBaseInteractor.GetUserAsync(request.Email);
        if (user != null)
        {
            throw new InvalidOperationException("User with this email already exists");
        }


        // Create a new user
        CreatePasswordHash(request.Password, out var passwordHash, out var passwordSalt);
        user = new User
        {
            UserName = request.UserName,
            Email = request.Email,
            PasswordHash = Encoding.UTF8.GetString(passwordHash),
            PasswordSalt = Encoding.UTF8.GetString(passwordSalt)
        };

        // Save the user to the database
        user = await _dataBaseInteractor.CreateUserAsync(user);

        return new AuthResponse
        {
            Token = (string)GenerateJwtToken(user)
        };

    }

    private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using var hmac = new HMACSHA512();
        passwordSalt = hmac.Key;
        passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
    }

    public bool VerifyPasswordHash(string password, string storedHashBase64, string storedSaltBase64)
    {
        if (password == null) throw new ArgumentNullException(nameof(password));
        if (string.IsNullOrWhiteSpace(storedHashBase64)) throw new ArgumentException("Invalid stored hash.", nameof(storedHashBase64));
        if (string.IsNullOrWhiteSpace(storedSaltBase64)) throw new ArgumentException("Invalid stored salt.", nameof(storedSaltBase64));

        var storedHash = Convert.FromBase64String(storedHashBase64);
        var storedSalt = Convert.FromBase64String(storedSaltBase64);

        using (var hmac = new HMACSHA512(storedSalt))
        {
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != storedHash[i]) return false;
            }
        }

        return true;
    }

    private static object GenerateJwtToken(User user)
    {
        List<Claim> claims =
        [
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName)
        ];

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET") ?? throw new Exception("JWT secret not found")));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(7),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}