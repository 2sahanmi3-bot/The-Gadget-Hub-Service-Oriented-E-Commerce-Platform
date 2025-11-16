using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GadgetHubClient.Pages
{
    public class CartModel : PageModel
    {
        private readonly ILogger<CartModel> _logger;
        public IConfiguration? Configuration { get; set; }

        public CartModel(ILogger<CartModel> logger, IConfiguration configuration)
        {
            _logger = logger;
            Configuration = configuration;
        }

        public void OnGet()
        {
            // Cart functionality is handled via JavaScript and session storage
            // This page just renders the cart interface
        }
    }
}
