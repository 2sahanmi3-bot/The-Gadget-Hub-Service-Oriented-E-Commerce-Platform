using System.Net.Http.Json;
using GadgetHubClient.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GadgetHubClient.Pages
{
    public class CompareModel : PageModel
    {
        private readonly IHttpClientFactory _httpFactory;

        public CompareModel(IHttpClientFactory httpFactory)
        {
            _httpFactory = httpFactory;
        }

        [BindProperty]
        public string ProductId { get; set; } = string.Empty;

        [BindProperty]
        public int Quantity { get; set; }

        public List<BestQuoteDTO>? Results { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var client = _httpFactory.CreateClient("GadgetHub");

            var request = new QuotationRequestDTO
            {
                GlobalIds = new List<string> { ProductId },
                Quantities = new List<int> { Quantity }
            };

            var response = await client.PostAsJsonAsync("api/quotation/compare", request);
            if (response.IsSuccessStatusCode)
                Results = await response.Content.ReadFromJsonAsync<List<BestQuoteDTO>>();

            return Page();
        }
    }
}
