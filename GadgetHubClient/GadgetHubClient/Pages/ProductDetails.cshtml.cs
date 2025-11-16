using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GadgetHubClient.Pages
{
    public class ProductDetailsModel : PageModel
    {
        private readonly ILogger<ProductDetailsModel> _logger;

        public ProductDetailsModel(ILogger<ProductDetailsModel> logger, IConfiguration configuration)
        {
            _logger = logger;
            Configuration = configuration;
        }

        [BindProperty(SupportsGet = true)]
        public int ProductId { get; set; }

        public IConfiguration? Configuration { get; set; }

        public IActionResult OnGet()
        {
            // Allow all users to view product details
            return Page();
        }
    }
}