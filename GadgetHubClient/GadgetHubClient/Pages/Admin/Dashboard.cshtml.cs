using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GadgetHubClient.Data;
using GadgetHubClient.Models;
using System.Text.Json;

namespace GadgetHubClient.Pages.Admin
{
	public class DashboardModel : PageModel
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IConfiguration _configuration;
		private readonly ILogger<DashboardModel> _logger;

		public DashboardModel(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<DashboardModel> logger)
		{
			_httpClientFactory = httpClientFactory;
			_configuration = configuration;
			_logger = logger;
		}

		public int TotalProducts { get; set; }
		public List<AdminOrder> RecentOrders { get; set; } = new();

		public async Task<IActionResult> OnGetAsync()
		{
			// Check if user is logged in and is admin
			var userId = HttpContext.Session.GetString("UserId");
			var role = HttpContext.Session.GetString("Role");
			_logger.LogInformation("Admin Dashboard access attempt - UserId: {UserId}, Role: {Role}", userId ?? "(null)", role ?? "(null)");

			if (string.IsNullOrEmpty(userId) || !"Admin".Equals(role, StringComparison.OrdinalIgnoreCase))
			{
				return RedirectToPage("/Login");
			}

			try
			{
				var httpClient = _httpClientFactory.CreateClient("GadgetHubAPI");

				// Products count
				try
				{
					var productsResponse = await httpClient.GetAsync("/api/Product");
					if (productsResponse.IsSuccessStatusCode)
					{
						var productsJson = await productsResponse.Content.ReadAsStringAsync();
						var products = JsonSerializer.Deserialize<List<Product>>(productsJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
						TotalProducts = products?.Count ?? 0;
					}
					else
					{
						TotalProducts = 0;
					}
				}
				catch { TotalProducts = 0; }

				// Recent orders from API (latest 5)
				var ordersResp = await httpClient.GetAsync("/api/Order");
				if (ordersResp.IsSuccessStatusCode)
				{
					var json = await ordersResp.Content.ReadAsStringAsync();
					var orders = JsonSerializer.Deserialize<List<OrderApiModel>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
					RecentOrders = orders
						.OrderByDescending(o => o.OrderDate)
						.Take(5)
						.Select(o => new AdminOrder
						{
							Id = o.OrderId,
							CustomerName = o.CustomerName ?? "",
							CreatedAt = o.OrderDate ?? DateTime.UtcNow,
							Status = MapStatus(o.Status),
							Total = o.TotalAmount
						})
						.ToList();
				}
				else
				{
					RecentOrders = new();
				}

				return Page();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error loading admin dashboard");
				return RedirectToPage("/Login");
			}
		}

		private static string MapStatus(int? status)
		{
			return status switch
			{
				1 => "Processing",
				2 => "Confirmed",
				3 => "Shipped",
				4 => "Delivered",
				5 => "Cancelled",
				6 => "Failed",
				_ => "Pending"
			};
		}

		public class OrderApiModel
		{
			public int OrderId { get; set; }
			public DateTime? OrderDate { get; set; }
			public int? Status { get; set; }
			public decimal TotalAmount { get; set; }
			public string? CustomerName { get; set; }
		}

		public class AdminOrder
		{
			public int Id { get; set; }
			public string CustomerName { get; set; } = string.Empty;
			public DateTime CreatedAt { get; set; }
			public string Status { get; set; } = string.Empty;
			public decimal Total { get; set; }
		}
	}
}