using GadgetHubAPI.Data;
using GadgetHubAPI.Models;
using GadgetHubAPI.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace GadgetHubAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // [Authorize] // Temporarily disabled for development - client handles authentication via session
    public class CustomerController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(AppDbContext context, ILogger<CustomerController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCustomers()
        {
            var customers = await _context.Customers.AsNoTracking().OrderByDescending(c => c.CreatedAt).ToListAsync();
            return Ok(customers.Select(c => new {
                customerId = c.Id,
                name = c.Name,
                email = c.Email,
                phoneNumber = c.PhoneNumber,
                address = c.Address,
                userId = c.UserId,
                createdAt = c.CreatedAt
            }));
        }

        [HttpGet("by-user/{userId}")]
        public async Task<IActionResult> GetCustomerByUserId(int userId)
        {
            try
            {
                var customer = await _context.Customers
                    .Include(c => c.User)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (customer == null)
                {
                    return NotFound(new { message = $"Customer with UserId {userId} not found." });
                }

                return Ok(new
                {
                    customerId = customer.Id,
                    name = customer.Name,
                    email = customer.Email,
                    phoneNumber = customer.PhoneNumber,
                    address = customer.Address,
                    userId = customer.UserId,
                    createdAt = customer.CreatedAt
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving customer for userId {UserId}", userId);
                return StatusCode(500, new { message = "Internal server error while retrieving customer." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerDTO customerDto)
        {
            try
            {
                // Check if customer already exists for this user
                var existingCustomer = await _context.Customers
                    .FirstOrDefaultAsync(c => c.UserId == customerDto.UserId);

                if (existingCustomer != null)
                {
                    return Conflict(new { message = $"Customer already exists for UserId {customerDto.UserId}" });
                }

                var customer = new Customer
                {
                    Name = customerDto.Name,
                    Email = customerDto.Email,
                    PhoneNumber = customerDto.PhoneNumber,
                    Address = customerDto.Address,
                    UserId = customerDto.UserId,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Customer {CustomerId} created for UserId {UserId}", customer.Id, customerDto.UserId);

                return CreatedAtAction(nameof(GetCustomerById), new { id = customer.Id }, new
                {
                    customerId = customer.Id,
                    name = customer.Name,
                    email = customer.Email,
                    phoneNumber = customer.PhoneNumber,
                    address = customer.Address,
                    userId = customer.UserId,
                    createdAt = customer.CreatedAt
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating customer for UserId {UserId}", customerDto.UserId);
                return StatusCode(500, new { message = "Internal server error while creating customer." });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            try
            {
                var customer = await _context.Customers
                    .Include(c => c.User)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (customer == null)
                {
                    return NotFound(new { message = $"Customer with ID {id} not found." });
                }

                return Ok(new
                {
                    customerId = customer.Id,
                    name = customer.Name,
                    email = customer.Email,
                    phoneNumber = customer.PhoneNumber,
                    address = customer.Address,
                    userId = customer.UserId,
                    createdAt = customer.CreatedAt
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving customer {CustomerId}", id);
                return StatusCode(500, new { message = "Internal server error while retrieving customer." });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            try
            {
                var customer = await _context.Customers.FindAsync(id);
                if (customer == null)
                {
                    return NotFound(new { message = $"Customer with ID {id} not found." });
                }

                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
                return Ok(new { message = $"Customer {id} deleted." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting customer {CustomerId}", id);
                return StatusCode(500, new { message = "Internal server error while deleting customer." });
            }
        }
    }
}
