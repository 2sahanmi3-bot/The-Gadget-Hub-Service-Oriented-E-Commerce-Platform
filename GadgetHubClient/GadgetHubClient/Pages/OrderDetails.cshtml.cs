using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace GadgetHubClient.Pages
{
    public class OrderDetailsModel : PageModel
    {
        private readonly ILogger<OrderDetailsModel> _logger;
        public IConfiguration? Configuration { get; set; }

        public OrderDetailsModel(ILogger<OrderDetailsModel> logger, IConfiguration configuration)
        {
            _logger = logger;
            Configuration = configuration;
        }

        public IActionResult OnGet()
        {
            // Order functionality is handled via JavaScript and API calls
            // This page just renders the order details interface
            return Page();
        }
    }
}


