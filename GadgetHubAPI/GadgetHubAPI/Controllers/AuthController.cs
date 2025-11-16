using GadgetHubAPI.Data;
using GadgetHubAPI.Models;
using GadgetHubAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace GadgetHubAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserRepo _users;
        private readonly CustomerRepo _customers;
        private readonly AuthService _auth;
        private readonly AppDbContext _context;

        public AuthController(UserRepo users, CustomerRepo customers, AuthService auth, AppDbContext context)
        {
            _users = users;
            _customers = customers;
            _auth = auth;
            _context = context;
        }

        [HttpGet("test")]
        public ActionResult Test()
        {
            return Ok(new { message = "AuthController with dependencies is working!" });
        }

        [HttpPost("register")]
        public ActionResult Register([FromBody] RegisterDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("Username and password are required.");

            if (dto.Role != "User" && dto.Role != "Admin")
                return BadRequest("Role must be 'User' or 'Admin'.");

            if (_users.Exists(dto.Username))
                return Conflict("Username already exists.");

            var user = new User { Username = dto.Username, Role = dto.Role };
            user.PasswordHash = _auth.HashPassword(user, dto.Password);
            _users.Add(user);

            return Ok(new { message = "Registered" });
        }

        [HttpPost("register-customer")]
        public async Task<ActionResult> RegisterCustomer([FromBody] CustomerRegisterDto dto)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password) ||
                    string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Email) ||
                    string.IsNullOrWhiteSpace(dto.PhoneNumber) || string.IsNullOrWhiteSpace(dto.Address))
                {
                    return BadRequest("All fields are required.");
                }

                if (dto.Password.Length < 6)
                {
                    return BadRequest("Password must be at least 6 characters long.");
                }

                // Check if username already exists
                if (await _users.ExistsAsync(dto.Username))
                {
                    return Conflict("Username already exists. Please choose a different username.");
                }

                // Check if email already exists
                if (await _customers.ExistsByEmailAsync(dto.Email))
                {
                    return Conflict("Email already registered. Please use a different email or try logging in.");
                }

                // Check if phone number already exists
                if (await _customers.ExistsByPhoneAsync(dto.PhoneNumber))
                {
                    return Conflict("Phone number already registered. Please use a different phone number.");
                }

                // Create new user with hashed password
                var user = new User
                {
                    Username = dto.Username,
                    PasswordHash = _auth.HashPassword(new User(), dto.Password),
                    Role = "Customer", // Always set role to Customer for registration
                    CreatedAt = DateTime.UtcNow
                };

                // Save user first to get the ID
                await _users.AddAsync(user);

                // Create new customer linked to the user
                var customer = new Customer
                {
                    Name = dto.Name,
                    Email = dto.Email,
                    PhoneNumber = dto.PhoneNumber,
                    Address = dto.Address,
                    UserId = user.Id, // Link to the user
                    CreatedAt = DateTime.UtcNow
                };

                await _customers.AddAsync(customer);

                return Ok(new { message = "Customer account created successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred while creating your account: {ex.Message}" });
            }
        }

        [HttpPost("login")]
        public ActionResult Login([FromBody] LoginDto dto)
        {
            var user = _users.GetByUsername(dto.Username);
            if (user == null) return Unauthorized("Invalid credentials.");

            if (!_auth.VerifyPassword(user, dto.Password))
                return Unauthorized("Invalid credentials.");

            var token = _auth.CreateToken(user);
            
            // Get customer ID if user is a customer
            int? customerId = null;
            if (user.Role == "User")
            {
                var customer = _context.Customers.FirstOrDefault(c => c.UserId == user.Id);
                customerId = customer?.Id;
            }

            return Ok(new { 
                token, 
                user = new { user.Username, user.Role, user.Id }, 
                customerId 
            });
        }

        [HttpGet("me")]
        [Authorize]
        public ActionResult Me()
        {
            var username = User.Identity?.Name ?? "";
            var role = User.Claims.FirstOrDefault(c => c.Type == "role" || c.Type.EndsWith("/role"))?.Value ?? "User";
            return Ok(new { username, role });
        }

        [HttpDelete("{username}")]
        public async Task<ActionResult> Delete(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return BadRequest("Username is required.");
            var user = await _users.GetByUsernameAsync(username);
            if (user == null) return NotFound($"User '{username}' not found.");
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return Ok(new { message = $"User '{username}' deleted." });
        }

        public class RegisterDto
        {
            public string Username { get; set; } = "";
            public string Password { get; set; } = "";
            public string Role { get; set; } = "User"; // "User" or "Admin"
        }

        public class LoginDto
        {
            public string Username { get; set; } = "";
            public string Password { get; set; } = "";
        }

        public class CustomerRegisterDto
        {
            [Required]
            public string Username { get; set; } = "";
            
            [Required]
            [MinLength(6)]
            public string Password { get; set; } = "";
            
            [Required]
            public string Name { get; set; } = "";
            
            [Required]
            [EmailAddress]
            public string Email { get; set; } = "";
            
            [Required]
            public string PhoneNumber { get; set; } = "";
            
            [Required]
            public string Address { get; set; } = "";
        }
    }
}