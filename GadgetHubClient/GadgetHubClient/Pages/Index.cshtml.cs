using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GadgetHubClient.Models;
using System.Text.Json;

namespace GadgetHubClient.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public IndexModel(ILogger<IndexModel> logger, HttpClient httpClient, IConfiguration configuration)
        {
            _logger = logger;
            _httpClient = httpClient;
            _configuration = configuration;
        }

    public List<Product> Products { get; set; } = new List<Product>();
    public IConfiguration? Configuration { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Configuration = _configuration;
            
            try
            {
                var apiUrl = _configuration["GadgetHubAPI:BaseUrl"] ?? "https://localhost:7063";
                _logger.LogInformation("Attempting to fetch products from: {ApiUrl}", apiUrl);
                
                var response = await _httpClient.GetAsync($"{apiUrl}/api/Product");
                
                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("API Response: {JsonContent}", jsonContent);
                    
                    Products = JsonSerializer.Deserialize<List<Product>>(jsonContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new List<Product>();
                    
                    _logger.LogInformation("Successfully loaded {Count} products from API", Products.Count);
                }
                else
                {
                    _logger.LogWarning("Failed to fetch products. Status: {StatusCode}, Reason: {ReasonPhrase}", 
                        response.StatusCode, response.ReasonPhrase);
                    Products = new List<Product>(); // No fallback - show empty list
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error fetching products from API");
                Products = new List<Product>(); // No fallback - show empty list
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Request timeout fetching products from API");
                Products = new List<Product>(); // No fallback - show empty list
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching products from API");
                Products = new List<Product>(); // No fallback - show empty list
            }

            return Page();
        }
    }
}
