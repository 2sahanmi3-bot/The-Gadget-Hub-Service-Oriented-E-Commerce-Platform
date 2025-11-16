using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace GadgetHubClient.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        [HttpGet("check")]
        public IActionResult CheckLoginStatus()
        {
            // Check if user is logged in by checking session
            var userId = HttpContext.Session.GetString("UserId");
            var username = HttpContext.Session.GetString("Username");
            
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(username))
            {
                return Unauthorized(new { message = "User not logged in" });
            }
            
            return Ok(new { 
                isLoggedIn = true, 
                userId = userId, 
                username = username 
            });
        }

        [HttpGet("customer")]
        public async Task<IActionResult> GetCustomerDetails()
        {
            var userId = HttpContext.Session.GetString("UserId");
            
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "User not logged in" });
            }

            try
            {
                var apiUrl = HttpContext.RequestServices.GetRequiredService<IConfiguration>()["GadgetHubAPI:BaseUrl"] ?? "https://localhost:7063";
                
                using var httpClient = new HttpClient();
                var response = await httpClient.GetAsync($"{apiUrl}/api/Customer/by-user/{userId}");
                
                if (response.IsSuccessStatusCode)
                {
                    // Forward the JSON payload exactly as received from the API
                    // so the client gets a proper JSON object (not a JSON string)
                    var customerJson = await response.Content.ReadAsStringAsync();
                    return Content(customerJson, "application/json");
                }
                else
                {
                    return NotFound(new { message = "Customer details not found" });
                }
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Error retrieving customer details" });
            }
        }
    }
}
