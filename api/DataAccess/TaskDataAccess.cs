using MySqlConnector;
using api.Models;
using api.DataAccess;
using TaskModel = api.Models.Task;

namespace api.DataAccess
{
    public class TaskDataAccess
    {
        private readonly Database _db;

        public TaskDataAccess()
        {
            _db = new Database();
        }

        public async Task<List<TaskModel>> GetAllTasksAsync(int userId)
        {
            var tasks = new List<TaskModel>();
            using var connection = new MySqlConnection(_db.cs);
            await connection.OpenAsync();
            
            var query = "SELECT TaskId, UserId, CategoryId, Title, Description, Priority, Status, DueDate, CompletedAt, CreatedAt, UpdatedAt FROM Tasks WHERE UserId = @UserId ORDER BY CreatedAt DESC";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", userId);
            
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var categoryIdOrdinal = reader.GetOrdinal("CategoryId");
                var descriptionOrdinal = reader.GetOrdinal("Description");
                var dueDateOrdinal = reader.GetOrdinal("DueDate");
                var completedAtOrdinal = reader.GetOrdinal("CompletedAt");
                
                tasks.Add(new TaskModel
                {
                    TaskId = reader.GetInt32("TaskId"),
                    UserId = reader.GetInt32("UserId"),
                    CategoryId = reader.IsDBNull(categoryIdOrdinal) ? null : reader.GetInt32(categoryIdOrdinal),
                    Title = reader.GetString("Title"),
                    Description = reader.IsDBNull(descriptionOrdinal) ? null : reader.GetString(descriptionOrdinal),
                    Priority = reader.GetString("Priority"),
                    Status = reader.GetString("Status"),
                    DueDate = reader.IsDBNull(dueDateOrdinal) ? null : reader.GetDateTime(dueDateOrdinal),
                    CompletedAt = reader.IsDBNull(completedAtOrdinal) ? null : reader.GetDateTime(completedAtOrdinal),
                    CreatedAt = reader.GetDateTime("CreatedAt"),
                    UpdatedAt = reader.GetDateTime("UpdatedAt")
                });
            }
            
            return tasks;
        }

        public async Task<TaskModel?> GetTaskByIdAsync(int taskId, int userId)
        {
            using var connection = new MySqlConnection(_db.cs);
            await connection.OpenAsync();
            
            var query = "SELECT TaskId, UserId, CategoryId, Title, Description, Priority, Status, DueDate, CompletedAt, CreatedAt, UpdatedAt FROM Tasks WHERE TaskId = @TaskId AND UserId = @UserId";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@TaskId", taskId);
            command.Parameters.AddWithValue("@UserId", userId);
            
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var categoryIdOrdinal = reader.GetOrdinal("CategoryId");
                var descriptionOrdinal = reader.GetOrdinal("Description");
                var dueDateOrdinal = reader.GetOrdinal("DueDate");
                var completedAtOrdinal = reader.GetOrdinal("CompletedAt");
                
                return new TaskModel
                {
                    TaskId = reader.GetInt32("TaskId"),
                    UserId = reader.GetInt32("UserId"),
                    CategoryId = reader.IsDBNull(categoryIdOrdinal) ? null : reader.GetInt32(categoryIdOrdinal),
                    Title = reader.GetString("Title"),
                    Description = reader.IsDBNull(descriptionOrdinal) ? null : reader.GetString(descriptionOrdinal),
                    Priority = reader.GetString("Priority"),
                    Status = reader.GetString("Status"),
                    DueDate = reader.IsDBNull(dueDateOrdinal) ? null : reader.GetDateTime(dueDateOrdinal),
                    CompletedAt = reader.IsDBNull(completedAtOrdinal) ? null : reader.GetDateTime(completedAtOrdinal),
                    CreatedAt = reader.GetDateTime("CreatedAt"),
                    UpdatedAt = reader.GetDateTime("UpdatedAt")
                };
            }
            
            return null;
        }

        public async Task<int> CreateTaskAsync(TaskModel task)
        {
            using var connection = new MySqlConnection(_db.cs);
            await connection.OpenAsync();
            
            var query = @"INSERT INTO Tasks (UserId, CategoryId, Title, Description, Priority, Status, DueDate) 
                         VALUES (@UserId, @CategoryId, @Title, @Description, @Priority, @Status, @DueDate);
                         SELECT LAST_INSERT_ID();";
            
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", task.UserId);
            command.Parameters.AddWithValue("@CategoryId", task.CategoryId.HasValue ? (object)task.CategoryId.Value : DBNull.Value);
            command.Parameters.AddWithValue("@Title", task.Title);
            command.Parameters.AddWithValue("@Description", task.Description ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Priority", task.Priority);
            command.Parameters.AddWithValue("@Status", task.Status);
            command.Parameters.AddWithValue("@DueDate", task.DueDate.HasValue ? (object)task.DueDate.Value : DBNull.Value);
            
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<bool> UpdateTaskAsync(TaskModel task)
        {
            using var connection = new MySqlConnection(_db.cs);
            await connection.OpenAsync();
            
            var query = @"UPDATE Tasks 
                         SET CategoryId = @CategoryId, Title = @Title, Description = @Description, 
                             Priority = @Priority, Status = @Status, DueDate = @DueDate, 
                             CompletedAt = @CompletedAt
                         WHERE TaskId = @TaskId AND UserId = @UserId";
            
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@TaskId", task.TaskId);
            command.Parameters.AddWithValue("@UserId", task.UserId);
            command.Parameters.AddWithValue("@CategoryId", task.CategoryId.HasValue ? (object)task.CategoryId.Value : DBNull.Value);
            command.Parameters.AddWithValue("@Title", task.Title);
            command.Parameters.AddWithValue("@Description", task.Description ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Priority", task.Priority);
            command.Parameters.AddWithValue("@Status", task.Status);
            command.Parameters.AddWithValue("@DueDate", task.DueDate.HasValue ? (object)task.DueDate.Value : DBNull.Value);
            command.Parameters.AddWithValue("@CompletedAt", task.CompletedAt.HasValue ? (object)task.CompletedAt.Value : DBNull.Value);
            
            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteTaskAsync(int taskId, int userId)
        {
            using var connection = new MySqlConnection(_db.cs);
            await connection.OpenAsync();
            
            var query = "DELETE FROM Tasks WHERE TaskId = @TaskId AND UserId = @UserId";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@TaskId", taskId);
            command.Parameters.AddWithValue("@UserId", userId);
            
            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
    }
}

