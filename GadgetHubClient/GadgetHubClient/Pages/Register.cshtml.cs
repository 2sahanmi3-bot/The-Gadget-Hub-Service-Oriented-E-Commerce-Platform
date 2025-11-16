using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GadgetHubClient.Data;
using GadgetHubClient.Models;
using System.Text.Json;

namespace GadgetHubClient.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public RegisterModel(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        [BindProperty]
        public string Name { get; set; } = string.Empty;

        [BindProperty]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        public string PhoneNumber { get; set; } = string.Empty;

        [BindProperty]
        public string Address { get; set; } = string.Empty;

        [BindProperty]
        public string Username { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        [BindProperty]
        public string ConfirmPassword { get; set; } = string.Empty;

        [BindProperty]
        public string Role { get; set; } = "Customer";

        [BindProperty]
        public string PaymentMethod { get; set; } = "COD";

        public string ErrorMessage { get; set; } = string.Empty;
        public string SuccessMessage { get; set; } = string.Empty;

        public void OnGet()
        {
            // Initialize the page
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Email) || 
                    string.IsNullOrWhiteSpace(PhoneNumber) || string.IsNullOrWhiteSpace(Address) ||
                    string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
                {
                    ErrorMessage = "All fields are required.";
                    return Page();
                }

                if (Password != ConfirmPassword)
                {
                    ErrorMessage = "Passwords do not match.";
                    return Page();
                }

                if (Password.Length < 6)
                {
                    ErrorMessage = "Password must be at least 6 characters long.";
                    return Page();
                }

                // Prepare registration data
                var registrationData = new
                {
                    Username = Username,
                    Password = Password,
                    Name = Name,
                    Email = Email,
                    PhoneNumber = PhoneNumber,
                    Address = Address
                };

                // Get API base URL from configuration
                var apiBaseUrl = _configuration["GadgetHubAPI:BaseUrl"];
                var apiUrl = $"{apiBaseUrl}/api/Auth/register-customer";

                // Call GadgetHubAPI to register customer
                var json = JsonSerializer.Serialize(registrationData);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    SuccessMessage = "Account created successfully! You can now log in with your credentials.";
                    
                    // Clear form
                    Name = string.Empty;
                    Email = string.Empty;
                    PhoneNumber = string.Empty;
                    Address = string.Empty;
                    Username = string.Empty;
                    Password = string.Empty;
                    ConfirmPassword = string.Empty;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var errorResponse = JsonSerializer.Deserialize<JsonElement>(errorContent);
                    
                    if (errorResponse.TryGetProperty("message", out var messageElement))
                    {
                        ErrorMessage = messageElement.GetString() ?? "Registration failed.";
                    }
                    else
                    {
                        ErrorMessage = $"Registration failed with status: {response.StatusCode}";
                    }
                }

                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred while creating your account: {ex.Message}";
                return Page();
            }
        }
    }
}