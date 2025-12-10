using MySqlConnector;
using api.Models;
using api.DataAccess;

namespace api.DataAccess
{
    public class UserDataAccess
    {
        private readonly Database _db;

        public UserDataAccess()
        {
            _db = new Database();
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            using var connection = new MySqlConnection(_db.cs);
            await connection.OpenAsync();
            
            var query = "SELECT UserId, Username, Email, PasswordHash, CreatedAt, UpdatedAt FROM Users WHERE UserId = @UserId";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", userId);
            
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new User
                {
                    UserId = reader.GetInt32("UserId"),
                    Username = reader.GetString("Username"),
                    Email = reader.GetString("Email"),
                    PasswordHash = reader.GetString("PasswordHash"),
                    CreatedAt = reader.GetDateTime("CreatedAt"),
                    UpdatedAt = reader.GetDateTime("UpdatedAt")
                };
            }
            
            return null;
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            using var connection = new MySqlConnection(_db.cs);
            await connection.OpenAsync();
            
            var query = "SELECT UserId, Username, Email, PasswordHash, CreatedAt, UpdatedAt FROM Users WHERE Username = @Username";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Username", username);
            
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new User
                {
                    UserId = reader.GetInt32("UserId"),
                    Username = reader.GetString("Username"),
                    Email = reader.GetString("Email"),
                    PasswordHash = reader.GetString("PasswordHash"),
                    CreatedAt = reader.GetDateTime("CreatedAt"),
                    UpdatedAt = reader.GetDateTime("UpdatedAt")
                };
            }
            
            return null;
        }

        public async Task<int> CreateUserAsync(User user)
        {
            using var connection = new MySqlConnection(_db.cs);
            await connection.OpenAsync();
            
            var query = @"INSERT INTO Users (Username, Email, PasswordHash) 
                         VALUES (@Username, @Email, @PasswordHash);
                         SELECT LAST_INSERT_ID();";
            
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Username", user.Username);
            command.Parameters.AddWithValue("@Email", user.Email);
            command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
            
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }
    }
}

