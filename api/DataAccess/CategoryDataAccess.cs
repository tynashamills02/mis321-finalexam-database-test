using MySqlConnector;
using api.Models;
using api.DataAccess;

namespace api.DataAccess
{
    public class CategoryDataAccess
    {
        private readonly Database _db;

        public CategoryDataAccess()
        {
            _db = new Database();
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            var categories = new List<Category>();
            using var connection = new MySqlConnection(_db.cs);
            await connection.OpenAsync();
            
            var query = "SELECT CategoryId, CategoryName, Description, Color, CreatedAt, UpdatedAt FROM Categories ORDER BY CategoryName";
            using var command = new MySqlCommand(query, connection);
            
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var descriptionOrdinal = reader.GetOrdinal("Description");
                
                categories.Add(new Category
                {
                    CategoryId = reader.GetInt32("CategoryId"),
                    CategoryName = reader.GetString("CategoryName"),
                    Description = reader.IsDBNull(descriptionOrdinal) ? null : reader.GetString(descriptionOrdinal),
                    Color = reader.GetString("Color"),
                    CreatedAt = reader.GetDateTime("CreatedAt"),
                    UpdatedAt = reader.GetDateTime("UpdatedAt")
                });
            }
            
            return categories;
        }

        public async Task<Category?> GetCategoryByIdAsync(int categoryId)
        {
            using var connection = new MySqlConnection(_db.cs);
            await connection.OpenAsync();
            
            var query = "SELECT CategoryId, CategoryName, Description, Color, CreatedAt, UpdatedAt FROM Categories WHERE CategoryId = @CategoryId";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@CategoryId", categoryId);
            
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var descriptionOrdinal = reader.GetOrdinal("Description");
                
                return new Category
                {
                    CategoryId = reader.GetInt32("CategoryId"),
                    CategoryName = reader.GetString("CategoryName"),
                    Description = reader.IsDBNull(descriptionOrdinal) ? null : reader.GetString(descriptionOrdinal),
                    Color = reader.GetString("Color"),
                    CreatedAt = reader.GetDateTime("CreatedAt"),
                    UpdatedAt = reader.GetDateTime("UpdatedAt")
                };
            }
            
            return null;
        }
    }
}

