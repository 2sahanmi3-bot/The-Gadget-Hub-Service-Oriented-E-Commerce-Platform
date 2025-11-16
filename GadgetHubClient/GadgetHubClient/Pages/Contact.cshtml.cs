using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GadgetHubClient.Pages
{
    public class ContactModel : PageModel
    {
        private readonly ILogger<ContactModel> _logger;

        public ContactModel(ILogger<ContactModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            // Contact functionality is handled via JavaScript
            // This page just renders the contact interface
        }
    }
}
