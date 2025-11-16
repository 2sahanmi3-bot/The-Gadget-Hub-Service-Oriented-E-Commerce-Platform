using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GadgetHubClient.Models;
using System.Text.Json;

namespace GadgetHubClient.Pages.Admin
{
	public class ProductsModel : PageModel
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IConfiguration _configuration;
		private readonly ILogger<ProductsModel> _logger;

		public ProductsModel(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<ProductsModel> logger)
		{
			_httpClientFactory = httpClientFactory;
			_configuration = configuration;
			_logger = logger;
		}

		public List<Product> Products { get; set; } = new();

		[BindProperty]
		public string GlobalIdInput { get; set; } = string.Empty;

		public string StatusMessage { get; set; } = string.Empty;
		public bool IsSuccess { get; set; }

		public async Task<IActionResult> OnGetAsync()
		{
			var userId = HttpContext.Session.GetString("UserId");
			var role = HttpContext.Session.GetString("Role");
			_logger.LogInformation("Admin Products access - UserId: {UserId}, Role: {Role}", userId ?? "(null)", role ?? "(null)");
			if (string.IsNullOrEmpty(userId) || !"Admin".Equals(role, StringComparison.OrdinalIgnoreCase))
			{
				return RedirectToPage("/Login");
			}

			await LoadProductsAsync();
			return Page();
		}

		public async Task<IActionResult> OnPostAddAsync()
		{
			var userId = HttpContext.Session.GetString("UserId");
			var role = HttpContext.Session.GetString("Role");
			if (string.IsNullOrEmpty(userId) || !"Admin".Equals(role, StringComparison.OrdinalIgnoreCase))
			{
				return RedirectToPage("/Login");
			}

			if (string.IsNullOrWhiteSpace(GlobalIdInput))
			{
				IsSuccess = false;
				StatusMessage = "Please enter a Global Id.";
				await LoadProductsAsync();
				return Page();
			}

			try
			{
				var client = _httpClientFactory.CreateClient("GadgetHubAPI");
				var payload = JsonSerializer.Serialize(new { GlobalId = GlobalIdInput });
				var content = new StringContent(payload, System.Text.Encoding.UTF8, "application/json");
				var resp = await client.PostAsync("/api/Product/sync", content);
				var body = await resp.Content.ReadAsStringAsync();
				if (resp.IsSuccessStatusCode)
				{
					IsSuccess = true;
					StatusMessage = "Product synced successfully.";
					_logger.LogInformation("Sync success for GlobalId {GlobalId}: {Body}", GlobalIdInput, body);
				}
				else
				{
					IsSuccess = false;
					StatusMessage = $"Sync failed ({resp.StatusCode}). {body}";
					_logger.LogWarning("Sync failed for GlobalId {GlobalId}: {Status} {Body}", GlobalIdInput, resp.StatusCode, body);
				}
			}
			catch (Exception ex)
			{
				IsSuccess = false;
				StatusMessage = "Error syncing product. See logs.";
				_logger.LogError(ex, "Error syncing product with GlobalId {GlobalId}", GlobalIdInput);
			}

			GlobalIdInput = string.Empty;
			await LoadProductsAsync();
			return Page();
		}

		private async Task LoadProductsAsync()
		{
			try
			{
				var client = _httpClientFactory.CreateClient("GadgetHubAPI");
				var resp = await client.GetAsync("/api/Product");
				if (resp.IsSuccessStatusCode)
				{
					var json = await resp.Content.ReadAsStringAsync();
					Products = JsonSerializer.Deserialize<List<Product>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
				}
				else
				{
					_logger.LogWarning("Failed to load products for admin. Status: {Status}", resp.StatusCode);
					Products = new();
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error loading admin products");
				Products = new();
			}
		}
	}
}
