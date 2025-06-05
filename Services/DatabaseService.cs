using MySql.Data.MySqlClient;
using LoginAPI.Models;

namespace LoginAPI.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
        }

        public async Task<User?> ValidateUserAsync(string username, string password)
        {
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync();

                string query = @"
    SELECT 
        user_table.loginid, 
        user_role.role AS Role 
    FROM user_table 
    LEFT JOIN user_role ON user_role.loginid = user_table.loginid
    WHERE user_table.username = @username AND user_table.password = @password";

                using var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password); // In production, use hashed passwords!

                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new User
                    {
                        Id = reader.GetInt32(0),// Assuming loginid is an integer in the first column
                        Role = reader.IsDBNull(1) ? null : reader.GetString(1)

                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}