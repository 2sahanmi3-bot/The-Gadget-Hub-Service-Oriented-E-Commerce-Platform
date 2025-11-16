using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GadgetHubClient.Pages
{
	public class LogoutModel : PageModel
	{
		public IActionResult OnGet()
		{
			HttpContext.Session.Clear();
			// Also clear any TempData if needed
			TempData.Clear();
			return RedirectToPage("/Index");
		}
	}
}
