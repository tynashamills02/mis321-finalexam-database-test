using Microsoft.AspNetCore.Mvc;
using api.Models;
using api.DataAccess;
using TaskModel = api.Models.Task;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly TaskDataAccess _taskDataAccess;

        public TasksController()
        {
            _taskDataAccess = new TaskDataAccess();
        }

        // GET: api/tasks?userId=1
        [HttpGet]
        public async Task<ActionResult<List<TaskModel>>> GetTasks([FromQuery] int userId)
        {
            try
            {
                var tasks = await _taskDataAccess.GetAllTasksAsync(userId);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving tasks", error = ex.Message });
            }
        }

        // GET: api/tasks/5?userId=1
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskModel>> GetTask(int id, [FromQuery] int userId)
        {
            try
            {
                var task = await _taskDataAccess.GetTaskByIdAsync(id, userId);
                if (task == null)
                {
                    return NotFound(new { message = "Task not found" });
                }
                return Ok(task);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving task", error = ex.Message });
            }
        }

        // POST: api/tasks
        [HttpPost]
        public async Task<ActionResult<TaskModel>> CreateTask([FromBody] TaskModel task)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(task.Title))
                {
                    return BadRequest(new { message = "Title is required" });
                }

                var taskId = await _taskDataAccess.CreateTaskAsync(task);
                task.TaskId = taskId;
                return CreatedAtAction(nameof(GetTask), new { id = taskId, userId = task.UserId }, task);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating task", error = ex.Message });
            }
        }

        // PUT: api/tasks/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskModel task)
        {
            try
            {
                if (id != task.TaskId)
                {
                    return BadRequest(new { message = "Task ID mismatch" });
                }

                if (string.IsNullOrWhiteSpace(task.Title))
                {
                    return BadRequest(new { message = "Title is required" });
                }

                // Set CompletedAt if status is Completed
                if (task.Status == "Completed" && !task.CompletedAt.HasValue)
                {
                    task.CompletedAt = DateTime.Now;
                }
                else if (task.Status != "Completed")
                {
                    task.CompletedAt = null;
                }

                var updated = await _taskDataAccess.UpdateTaskAsync(task);
                if (!updated)
                {
                    return NotFound(new { message = "Task not found" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating task", error = ex.Message });
            }
        }

        // DELETE: api/tasks/5?userId=1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id, [FromQuery] int userId)
        {
            try
            {
                var deleted = await _taskDataAccess.DeleteTaskAsync(id, userId);
                if (!deleted)
                {
                    return NotFound(new { message = "Task not found" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting task", error = ex.Message });
            }
        }
    }
}

