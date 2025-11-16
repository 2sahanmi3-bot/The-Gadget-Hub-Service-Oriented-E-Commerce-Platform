using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace GadgetHubClient.Pages.Admin
{
	public class UsersModel : PageModel
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly ILogger<UsersModel> _logger;

		public UsersModel(IHttpClientFactory httpClientFactory, ILogger<UsersModel> logger)
		{
			_httpClientFactory = httpClientFactory;
			_logger = logger;
		}

		public List<CustomerRow> Customers { get; set; } = new();
		public string StatusMessage { get; set; } = string.Empty;
		public bool IsSuccess { get; set; }

		[BindProperty]
		public int DeleteCustomerId { get; set; }

		public async Task<IActionResult> OnGetAsync()
		{
			var userId = HttpContext.Session.GetString("UserId");
			var role = HttpContext.Session.GetString("Role");
			if (string.IsNullOrEmpty(userId) || !"Admin".Equals(role, StringComparison.OrdinalIgnoreCase))
			{
				return RedirectToPage("/Login");
			}

			await LoadCustomersAsync();
			return Page();
		}

		public async Task<IActionResult> OnPostDeleteAsync()
		{
			var userId = HttpContext.Session.GetString("UserId");
			var role = HttpContext.Session.GetString("Role");
			if (string.IsNullOrEmpty(userId) || !"Admin".Equals(role, StringComparison.OrdinalIgnoreCase))
			{
				return RedirectToPage("/Login");
			}

			try
			{
				var client = _httpClientFactory.CreateClient("GadgetHubAPI");
				var resp = await client.DeleteAsync($"/api/Customer/{DeleteCustomerId}");
				var body = await resp.Content.ReadAsStringAsync();
				IsSuccess = resp.IsSuccessStatusCode;
				StatusMessage = IsSuccess ? "Customer removed." : $"Delete failed ({resp.StatusCode}). {body}";
			}
			catch (Exception ex)
			{
				IsSuccess = false;
				StatusMessage = "Error removing customer.";
				_logger.LogError(ex, "Delete customer error");
			}

			await LoadCustomersAsync();
			return Page();
		}

		private async Task LoadCustomersAsync()
		{
			try
			{
				var client = _httpClientFactory.CreateClient("GadgetHubAPI");
				var resp = await client.GetAsync("/api/Customer");
				if (resp.IsSuccessStatusCode)
				{
					var json = await resp.Content.ReadAsStringAsync();
					var customers = JsonSerializer.Deserialize<List<CustomerApiModel>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
					Customers = customers.Select(c => new CustomerRow
					{
						CustomerId = c.CustomerId,
						Name = c.Name ?? "",
						Email = c.Email ?? "",
						PhoneNumber = c.PhoneNumber ?? "",
						Address = c.Address ?? "",
						UserId = c.UserId,
						CreatedAt = c.CreatedAt
					}).ToList();
				}
				else
				{
					Customers = new();
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error loading customers");
				Customers = new();
			}
		}

		public class CustomerApiModel
		{
			public int CustomerId { get; set; }
			public string? Name { get; set; }
			public string? Email { get; set; }
			public string? PhoneNumber { get; set; }
			public string? Address { get; set; }
			public int UserId { get; set; }
			public DateTime? CreatedAt { get; set; }
		}

		public class CustomerRow
		{
			public int CustomerId { get; set; }
			public string Name { get; set; } = string.Empty;
			public string Email { get; set; } = string.Empty;
			public string PhoneNumber { get; set; } = string.Empty;
			public string Address { get; set; } = string.Empty;
			public int UserId { get; set; }
			public DateTime? CreatedAt { get; set; }
		}
	}
}
