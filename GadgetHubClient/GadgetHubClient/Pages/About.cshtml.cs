using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GadgetHubClient.Pages
{
    public class AboutModel : PageModel
    {
        private readonly ILogger<AboutModel> _logger;

        public AboutModel(ILogger<AboutModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            // About page is static content
            // This page just renders the about information
        }
    }
}
