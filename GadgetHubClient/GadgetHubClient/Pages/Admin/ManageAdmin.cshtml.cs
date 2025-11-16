using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace GadgetHubClient.Pages.Admin
{
	public class ManageAdminModel : PageModel
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly ILogger<ManageAdminModel> _logger;

		public ManageAdminModel(IHttpClientFactory httpClientFactory, ILogger<ManageAdminModel> logger)
		{
			_httpClientFactory = httpClientFactory;
			_logger = logger;
		}

		[BindProperty]
		public string RegisterUsername { get; set; } = string.Empty;
		[BindProperty]
		public string RegisterPassword { get; set; } = string.Empty;
		[BindProperty]
		public string DeleteUsername { get; set; } = string.Empty;

		public string StatusMessage { get; set; } = string.Empty;
		public bool IsSuccess { get; set; }

		public IActionResult OnGet()
		{
			var userId = HttpContext.Session.GetString("UserId");
			var role = HttpContext.Session.GetString("Role");
			if (string.IsNullOrEmpty(userId) || !"Admin".Equals(role, StringComparison.OrdinalIgnoreCase))
			{
				return RedirectToPage("/Login");
			}
			return Page();
		}

		public async Task<IActionResult> OnPostRegisterAsync()
		{
			var userId = HttpContext.Session.GetString("UserId");
			var role = HttpContext.Session.GetString("Role");
			if (string.IsNullOrEmpty(userId) || !"Admin".Equals(role, StringComparison.OrdinalIgnoreCase))
			{
				return RedirectToPage("/Login");
			}

			if (string.IsNullOrWhiteSpace(RegisterUsername) || string.IsNullOrWhiteSpace(RegisterPassword))
			{
				IsSuccess = false;
				StatusMessage = "Username and password are required.";
				return Page();
			}

			try
			{
				var client = _httpClientFactory.CreateClient("GadgetHubAPI");
				var payload = JsonSerializer.Serialize(new { Username = RegisterUsername, Password = RegisterPassword, Role = "Admin" });
				var content = new StringContent(payload, System.Text.Encoding.UTF8, "application/json");
				var resp = await client.PostAsync("/api/Auth/register", content);
				var body = await resp.Content.ReadAsStringAsync();
				IsSuccess = resp.IsSuccessStatusCode;
				StatusMessage = IsSuccess ? "Admin registered successfully." : $"Register failed ({resp.StatusCode}). {body}";
			}
			catch (Exception ex)
			{
				IsSuccess = false;
				StatusMessage = "Error while registering admin.";
				_logger.LogError(ex, "Register admin error");
			}

			RegisterUsername = string.Empty;
			RegisterPassword = string.Empty;
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

			if (string.IsNullOrWhiteSpace(DeleteUsername))
			{
				IsSuccess = false;
				StatusMessage = "Please enter a username to delete.";
				return Page();
			}

			try
			{
				var client = _httpClientFactory.CreateClient("GadgetHubAPI");
				var resp = await client.DeleteAsync($"/api/Auth/{Uri.EscapeDataString(DeleteUsername)}");
				var body = await resp.Content.ReadAsStringAsync();
				IsSuccess = resp.IsSuccessStatusCode;
				StatusMessage = IsSuccess ? "User deleted successfully." : $"Delete failed ({resp.StatusCode}). {body}";
			}
			catch (Exception ex)
			{
				IsSuccess = false;
				StatusMessage = "Error deleting user.";
				_logger.LogError(ex, "Delete user error");
			}

			DeleteUsername = string.Empty;
			return Page();
		}
	}
}
