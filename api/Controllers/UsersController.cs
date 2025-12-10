using Microsoft.AspNetCore.Mvc;
using api.Models;
using api.DataAccess;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserDataAccess _userDataAccess;

        public UsersController()
        {
            _userDataAccess = new UserDataAccess();
        }

        // GET: api/users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            try
            {
                var user = await _userDataAccess.GetUserByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }
                // Don't return password hash
                user.PasswordHash = string.Empty;
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving user", error = ex.Message });
            }
        }

        // POST: api/users/login
        [HttpPost("login")]
        public async Task<ActionResult<User>> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Username))
                {
                    return BadRequest(new { message = "Username is required" });
                }

                var user = await _userDataAccess.GetUserByUsernameAsync(request.Username);
                if (user == null)
                {
                    return Unauthorized(new { message = "Invalid username or password" });
                }

                // Simple password check (in production, use proper hashing)
                if (user.PasswordHash != request.Password)
                {
                    return Unauthorized(new { message = "Invalid username or password" });
                }

                // Don't return password hash
                user.PasswordHash = string.Empty;
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error during login", error = ex.Message });
            }
        }

        // POST: api/users/register
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register([FromBody] RegisterRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Username))
                {
                    return BadRequest(new { message = "Username is required" });
                }

                if (string.IsNullOrWhiteSpace(request.Email))
                {
                    return BadRequest(new { message = "Email is required" });
                }

                if (string.IsNullOrWhiteSpace(request.Password))
                {
                    return BadRequest(new { message = "Password is required" });
                }

                // Check if user already exists
                var existingUser = await _userDataAccess.GetUserByUsernameAsync(request.Username);
                if (existingUser != null)
                {
                    return Conflict(new { message = "Username already exists" });
                }

                var user = new User
                {
                    Username = request.Username,
                    Email = request.Email,
                    PasswordHash = request.Password // In production, hash this properly
                };

                var userId = await _userDataAccess.CreateUserAsync(user);
                user.UserId = userId;
                user.PasswordHash = string.Empty;

                return CreatedAtAction(nameof(GetUser), new { id = userId }, user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating user", error = ex.Message });
            }
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}

