using Cubichi.Models;

namespace Cubichi.DataBase;

public interface IDataBaseInteractor
{
    Task<User?> GetUserAsync(string userName);
    Task<User?> GetUserAsync(int userId);
    Task<User?> GetUserAsync(string userName, string Password);
    Task<User> CreateUserAsync(User user);
    Task<User> UpdateUserAsync(User user);
    Task<User> DeleteUserAsync(User user);
    Task<User> DeteteUserAsync(string userName);
}