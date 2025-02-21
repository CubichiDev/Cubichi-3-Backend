using Cubichi.Models;
using Npgsql;

namespace Cubichi.DataBase;

public class PsqlInteractor : IDataBaseInteractor
{
    private readonly NpgsqlConnection _connection;

    public PsqlInteractor()
    {
        var host = Environment.GetEnvironmentVariable("DB_HOST") ?? throw new Exception("DB_HOST not found");
        var port = Environment.GetEnvironmentVariable("DB_PORT") ?? throw new Exception("DB_PORT not found");
        var dbName = Environment.GetEnvironmentVariable("DB_NAME") ?? throw new Exception("DB_NAME not found");
        var user = Environment.GetEnvironmentVariable("DB_USER") ?? throw new Exception("DB_USER not found");
        var password = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? throw new Exception("DB_PASSWORD not found");

        var connectionString = $"Host={host};Port={port};Database={dbName};Username={user};Password={password}";
        _connection = new NpgsqlConnection(connectionString);
        _connection.Open();
    }

    public async Task<User> CreateUserAsync(User user)
    {
        // Create a command to insert a new user
        using var command = new NpgsqlCommand("INSERT INTO users (username, email, password) VALUES (@username, @email, encode(digest(@password, 'sha256'), 'hex')) RETURNING id", _connection);
        command.Parameters.AddWithValue("username", user.UserName);
        command.Parameters.AddWithValue("email", user.Email);
        command.Parameters.AddWithValue("password", user.Password);

        // Execute the command and get the new user's id
        user.Id = (int)(await command.ExecuteScalarAsync() ?? throw new Exception("Failed to create user"));
        return user;
    }

    public Task<User> DeleteUserAsync(User user)
    {
        throw new NotImplementedException();
    }

    public Task<User> DeteteUserAsync(string userName)
    {
        throw new NotImplementedException();
    }

    public async Task<User?> GetUserAsync(string userName)
    {
        using var command = new NpgsqlCommand("SELECT * FROM users WHERE username = @username", _connection);
        command.Parameters.AddWithValue("username", userName);

        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new User
            {
                Id = reader.GetInt32(0),
                UserName = reader.GetString(1),
                Email = reader.GetString(2),
                Password = reader.GetString(3)
            };
        }

        return null;
    }

    public async Task<User?> GetUserAsync(int userId)
    {
        using var command = new NpgsqlCommand("SELECT * FROM users WHERE id = @id", _connection);
        command.Parameters.AddWithValue("id", userId);

        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new User
            {
                Id = reader.GetInt32(0),
                UserName = reader.GetString(1),
                Email = reader.GetString(2),
                Password = reader.GetString(3)
            };
        }

        return null;
    }

    public async Task<User?> GetUserAsync(string userName, string password)
    {
        using var command = new NpgsqlCommand("SELECT * FROM users WHERE username = @username AND password = encode(digest(@password, 'sha256'), 'hex')", _connection);
        command.Parameters.AddWithValue("username", userName);
        command.Parameters.AddWithValue("password", password);

        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new User
            {
                Id = reader.GetInt32(0),
                UserName = reader.GetString(1),
                Email = reader.GetString(2),
                Password = reader.GetString(3)
            };
        }

        return null;
    }

    public async Task<User> UpdateUserAsync(User user)
    {
        using var command = new NpgsqlCommand("UPDATE users SET username = @username, email = @email, password = @password WHERE id = @id", _connection);
        command.Parameters.AddWithValue("id", user.Id);
        command.Parameters.AddWithValue("username", user.UserName);
        command.Parameters.AddWithValue("email", user.Email);
        command.Parameters.AddWithValue("password", user.Password);

        await command.ExecuteNonQueryAsync();
        return user;
    }
}