using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GadgetHubClient.Data;
using GadgetHubClient.Models;
using System.Text.Json;

namespace GadgetHubClient.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<LoginModel> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        [BindProperty]
        public string Username { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        [BindProperty]
        public bool RememberMe { get; set; }

        public string ErrorMessage { get; set; } = string.Empty;

		public class LoginResponse
		{
			public string Token { get; set; } = "";
			public InnerUser User { get; set; } = new();
			public int? CustomerId { get; set; }
			public class InnerUser
			{
				public int Id { get; set; }
				public string Username { get; set; } = "";
				public string Role { get; set; } = "";
			}
		}

        public void OnGet()
        {
            // Initialize the page
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                _logger.LogInformation("Login attempt started for username: {Username}", Username);
                
                // Validate input
                if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
                {
                    _logger.LogWarning("Login failed: Username or password is empty");
                    ErrorMessage = "Username and password are required.";
                    return Page();
                }

                // Prepare login data
                var loginData = new
                {
                    Username = Username,
                    Password = Password
                };

                // Get API base URL from configuration
                var apiBaseUrl = _configuration["GadgetHubAPI:BaseUrl"];
                _logger.LogInformation("API Base URL: {ApiBaseUrl}", apiBaseUrl);
                
                // Call GadgetHubAPI to login using the named HttpClient
                var httpClient = _httpClientFactory.CreateClient("GadgetHubAPI");
                _logger.LogInformation("HttpClient created. BaseAddress: {BaseAddress}", httpClient.BaseAddress);
                
                var json = JsonSerializer.Serialize(loginData);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                _logger.LogInformation("Sending login request to API with JSON: {Json}", json);
                
                HttpResponseMessage response;
                try
                {
                    response = await httpClient.PostAsync("/api/Auth/login", content);
                    _logger.LogInformation("API response status: {StatusCode}", response.StatusCode);
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError(ex, "HttpRequestException occurred. Trying fallback approach.");
                    // Fallback: try with a regular HttpClient
                    var fallbackClient = _httpClientFactory.CreateClient();
                    var fullUrl = $"{apiBaseUrl}/api/Auth/login";
                    _logger.LogInformation("Trying fallback URL: {FullUrl}", fullUrl);
                    response = await fallbackClient.PostAsync(fullUrl, content);
                    _logger.LogInformation("Fallback API response status: {StatusCode}", response.StatusCode);
                }

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var loginResponse = JsonSerializer.Deserialize<LoginResponse>(
                        responseContent,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );

                    // Extract user information from response in a robust, case-insensitive way
                    if (loginResponse?.User is not null && !string.IsNullOrWhiteSpace(loginResponse.User.Role))
                    {
                        var username = string.IsNullOrWhiteSpace(loginResponse.User.Username) ? Username : loginResponse.User.Username;
                        var role = string.IsNullOrWhiteSpace(loginResponse.User.Role) ? "Customer" : loginResponse.User.Role;
                        var userId = loginResponse.User.Id;
                        var customerId = loginResponse.CustomerId;

                        // Set session data
                        HttpContext.Session.SetString("UserId", userId.ToString());
                        HttpContext.Session.SetString("Username", username ?? Username);
                        HttpContext.Session.SetString("Role", role ?? "Customer");
                        if (customerId.HasValue)
                        {
                            HttpContext.Session.SetString("CustomerId", customerId.Value.ToString());
                        }
                        HttpContext.Session.SetString("AuthToken", loginResponse.Token ?? "");

                        // Try to get or create customer details
                        try
                        {
                            HttpResponseMessage customerResponse;
                            try
                            {
                                customerResponse = await httpClient.GetAsync($"/api/Customer/by-user/{userId}");
                            }
                            catch (HttpRequestException ex)
                            {
                                _logger.LogError(ex, "HttpRequestException in customer API call. Using fallback.");
                                var fallbackClient = _httpClientFactory.CreateClient();
                                var customerUrl = $"{apiBaseUrl}/api/Customer/by-user/{userId}";
                                customerResponse = await fallbackClient.GetAsync(customerUrl);
                            }
                            if (customerResponse.IsSuccessStatusCode)
                            {
                                var customerContent = await customerResponse.Content.ReadAsStringAsync();
                                var customerData = JsonSerializer.Deserialize<JsonElement>(customerContent);
                                
                                HttpContext.Session.SetString("CustomerName", customerData.TryGetProperty("name", out var nameProp) ? nameProp.GetString() ?? "Customer" : "Customer");
                                HttpContext.Session.SetString("CustomerEmail", customerData.TryGetProperty("email", out var emailProp) ? emailProp.GetString() ?? "customer@example.com" : "customer@example.com");
                                HttpContext.Session.SetString("CustomerPhone", customerData.TryGetProperty("phoneNumber", out var phoneProp) ? phoneProp.GetString() ?? "" : "");
                                HttpContext.Session.SetString("CustomerAddress", customerData.TryGetProperty("address", out var addressProp) ? addressProp.GetString() ?? "" : "");
                            }
                            else
                            {
                                // If no customer record exists, create one
                                var createCustomerData = new
                                {
                                    Name = username ?? "Customer",
                                    Email = $"{username}@example.com",
                                    PhoneNumber = "",
                                    Address = "",
                                    UserId = userId
                                };
                                
                                var createJson = JsonSerializer.Serialize(createCustomerData);
                                var createContent = new StringContent(createJson, System.Text.Encoding.UTF8, "application/json");
                                
                                HttpResponseMessage createResponse;
                                try
                                {
                                    createResponse = await httpClient.PostAsync($"/api/Customer", createContent);
                                }
                                catch (HttpRequestException ex)
                                {
                                    _logger.LogError(ex, "HttpRequestException in customer creation API call. Using fallback.");
                                    var fallbackClient = _httpClientFactory.CreateClient();
                                    var createUrl = $"{apiBaseUrl}/api/Customer";
                                    createResponse = await fallbackClient.PostAsync(createUrl, createContent);
                                }
                                if (createResponse.IsSuccessStatusCode)
                                {
                                    var newCustomerContent = await createResponse.Content.ReadAsStringAsync();
                                    var newCustomerData = JsonSerializer.Deserialize<JsonElement>(newCustomerContent);
                                    
                                    HttpContext.Session.SetString("CustomerName", newCustomerData.TryGetProperty("name", out var newNameProp) ? newNameProp.GetString() ?? "Customer" : "Customer");
                                    HttpContext.Session.SetString("CustomerEmail", newCustomerData.TryGetProperty("email", out var newEmailProp) ? newEmailProp.GetString() ?? "customer@example.com" : "customer@example.com");
                                    HttpContext.Session.SetString("CustomerPhone", newCustomerData.TryGetProperty("phoneNumber", out var newPhoneProp) ? newPhoneProp.GetString() ?? "" : "");
                                    HttpContext.Session.SetString("CustomerAddress", newCustomerData.TryGetProperty("address", out var newAddressProp) ? newAddressProp.GetString() ?? "" : "");
                                }
                                else
                                {
                                    // Fallback to default values
                                    HttpContext.Session.SetString("CustomerName", "Customer");
                                    HttpContext.Session.SetString("CustomerEmail", "customer@example.com");
                                    HttpContext.Session.SetString("CustomerPhone", "");
                                    HttpContext.Session.SetString("CustomerAddress", "");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error fetching/creating customer details for user {UserId}", userId);
                            // Fallback to default values
                            HttpContext.Session.SetString("CustomerName", "Customer");
                            HttpContext.Session.SetString("CustomerEmail", "customer@example.com");
                            HttpContext.Session.SetString("CustomerPhone", "");
                            HttpContext.Session.SetString("CustomerAddress", "");
                        }

                        // Set TempData to trigger JavaScript sessionStorage update
                        TempData["LoginSuccess"] = true;

                        // Check for return URL from query parameter or TempData
                        var returnUrl = Request.Query["returnUrl"].FirstOrDefault() ?? TempData["ReturnUrl"]?.ToString();

                        // Redirect based on role or return URL
                        if (role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                        {
                            _logger.LogInformation("Redirecting to Admin Dashboard");
                            return RedirectToPage("/Admin/Dashboard");
                        }
                        else
                        {
                            _logger.LogInformation("Redirecting to Index page for role: {Role}", role);
                            return RedirectToPage("/Index");
                        }

                    }
                    else
                    {
                        ErrorMessage = "Invalid response from server.";
                        return Page();
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Login failed with status {StatusCode}. Response content: {ErrorContent}", response.StatusCode, errorContent);
                    
                    try
                    {
                        var errorResponse = JsonSerializer.Deserialize<JsonElement>(errorContent);
                        
                        if (errorResponse.TryGetProperty("message", out var messageElement))
                        {
                            ErrorMessage = messageElement.GetString() ?? "Login failed.";
                        }
                        else
                        {
                            ErrorMessage = $"Login failed with status: {response.StatusCode}";
                        }
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogError(ex, "Failed to parse error response JSON");
                        ErrorMessage = $"Login failed with status: {response.StatusCode}. Raw response: {errorContent}";
                    }
                }

                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = "An error occurred during login. Please try again.";
                // Log the exception with full details
                _logger.LogError(ex, "Error during login process for username: {Username}. Exception: {ExceptionMessage}", Username, ex.Message);
                return Page();
            }
        }
    }
}