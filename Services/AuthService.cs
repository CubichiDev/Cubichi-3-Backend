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
        if (user == null)
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


        user = new User
        {
            UserName = request.UserName,
            Email = request.Email,
            Password = request.Password
        };

        // Save the user to the database
        user = await _dataBaseInteractor.CreateUserAsync(user);

        return new AuthResponse
        {
            Token = (string)GenerateJwtToken(user)
        };

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