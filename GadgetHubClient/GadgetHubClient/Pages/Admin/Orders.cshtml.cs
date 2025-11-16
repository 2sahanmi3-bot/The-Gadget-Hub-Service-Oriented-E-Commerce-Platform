using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace GadgetHubClient.Pages.Admin
{
	public class OrdersModel : PageModel
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly ILogger<OrdersModel> _logger;

		public OrdersModel(IHttpClientFactory httpClientFactory, ILogger<OrdersModel> logger)
		{
			_httpClientFactory = httpClientFactory;
			_logger = logger;
		}

		public List<OrderRow> Orders { get; set; } = new();
		public string StatusMessage { get; set; } = string.Empty;
		public bool IsSuccess { get; set; }

		public async Task<IActionResult> OnGetAsync()
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
				var resp = await client.GetAsync("/api/Order");
				if (resp.IsSuccessStatusCode)
				{
					var json = await resp.Content.ReadAsStringAsync();
					var orders = JsonSerializer.Deserialize<List<OrderApiModel>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
					Orders = orders.Select(o => new OrderRow
					{
						OrderId = o.OrderId,
						OrderNumber = o.OrderNumber ?? $"ORD-{o.OrderId}",
						CustomerName = o.CustomerName,
						CustomerEmail = o.CustomerEmail,
						CustomerPhone = o.CustomerPhone,
						TotalAmount = o.TotalAmount,
						Status = MapStatus(o.Status),
						OrderDate = o.OrderDate,
						EstimatedDeliveryDate = o.EstimatedDeliveryDate,
						DeliveryAddress = o.DeliveryAddress,
						Notes = o.Notes
					}).ToList();
				}
				else
				{
					StatusMessage = $"Failed to load orders ({resp.StatusCode}).";
					IsSuccess = false;
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error loading orders");
				StatusMessage = "Error loading orders.";
				IsSuccess = false;
			}

			return Page();
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
			public string? OrderNumber { get; set; }
			public int CustomerId { get; set; }
			public DateTime? OrderDate { get; set; }
			public int? Status { get; set; }
			public decimal TotalAmount { get; set; }
			public string? DeliveryAddress { get; set; }
			public string? CustomerName { get; set; }
			public string? CustomerEmail { get; set; }
			public string? CustomerPhone { get; set; }
			public DateTime? EstimatedDeliveryDate { get; set; }
			public string? Notes { get; set; }
		}

		public class OrderRow
		{
			public int OrderId { get; set; }
			public string OrderNumber { get; set; } = string.Empty;
			public string? CustomerName { get; set; }
			public string? CustomerEmail { get; set; }
			public string? CustomerPhone { get; set; }
			public decimal TotalAmount { get; set; }
			public string Status { get; set; } = "Pending";
			public DateTime? OrderDate { get; set; }
			public DateTime? EstimatedDeliveryDate { get; set; }
			public string? DeliveryAddress { get; set; }
			public string? Notes { get; set; }
		}
	}
}
