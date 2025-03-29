using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace UserManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        // In-memory user list (temporary storage)
        private static List<User> users = new List<User>
        {
            new User { Id = 1, Name = "Vraj", Email = "vraj@example.com" },
            new User { Id = 2, Name = "Vraj123", Email = "vraj123@example.com" }
        };

        // ✅ GET: api/users (Retrieve all users with pagination)
        [HttpGet]
        public ActionResult<IEnumerable<User>> GetUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 5)
        {
            var pagedUsers = users.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return Ok(pagedUsers);
        }

        // ✅ GET: api/users/1 (Retrieve a single user by ID with error handling)
        [HttpGet("{id}")]
        public ActionResult<User> GetUser(int id)
        {
            var user = users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return NotFound(new { message = "User not found" });

            return Ok(user);
        }

        // ✅ POST: api/users (Add a new user with validation)
        [HttpPost]
        public ActionResult<User> AddUser([FromBody] User newUser)
        {
            // Validation
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check for duplicate email
            if (users.Any(u => u.Email == newUser.Email))
                return Conflict(new { message = "Email already exists" });

            // Add user
            users.Add(newUser);
            return CreatedAtAction(nameof(GetUser), new { id = newUser.Id }, newUser);
        }

        // ✅ PUT: api/users/1 (Update a user with validation)
        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, [FromBody] User updatedUser)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return NotFound(new { message = "User not found" });

            user.Name = updatedUser.Name;
            user.Email = updatedUser.Email;
            return NoContent();
        }

        // ✅ DELETE: api/users/1 (Delete a user)
        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            var user = users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return NotFound(new { message = "User not found" });

            users.Remove(user);
            return NoContent();
        }
    }

    // ✅ User Model with Validation
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }
    }
}
