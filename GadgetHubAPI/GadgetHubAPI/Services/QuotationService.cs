using System.Net.Http.Json;
using GadgetHubAPI.DTO;
using Microsoft.Extensions.Logging;

namespace GadgetHubAPI.Services
{
    public class QuotationService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _config;
        private readonly ILogger<QuotationService> _logger;

        public QuotationService(IHttpClientFactory clientFactory, IConfiguration config, ILogger<QuotationService> logger)
        {
            _clientFactory = clientFactory;
            _config = config;
            _logger = logger;
        }

        // matches TechWorldAPI QuotationResponse contract
        private sealed class TechWorldQuotationResponse
        {
            public int ProductId { get; set; }
            public string GlobalId { get; set; } = "";
            public string ProductName { get; set; } = "";
            public string ProductDetails { get; set; } = "";
            public string Thumbnail { get; set; } = "";
            public decimal UnitPrice { get; set; }
            public int Inventory { get; set; }
            public string Supplier { get; set; } = "";
            public string SupplierAddress { get; set; } = "";
            public DateTime IssuedAt { get; set; }
            public int Quantity { get; set; }
            public decimal Price { get; set; }
            public string Description { get; set; } = "";
        }

        public async Task<List<QuotationResponseDTO>> GetQuotationsAsync(QuotationRequestDTO request)
        {
            var distributors = new[]
            {
                ("TechWorld", _config["Distributors:TechWorld"]),
                ("ElectroCom", _config["Distributors:ElectroCom"]),     // add when live
                ("GadgetCentral", _config["Distributors:GadgetCentral"]) // add when live
            };

            var results = new List<QuotationResponseDTO>();

            foreach (var (name, baseUrl) in distributors)
            {
                if (string.IsNullOrWhiteSpace(baseUrl))
                {
                    _logger.LogWarning("Distributor {Name} URL is not configured", name);
                    continue;
                }

                try
                {
                    var client = _clientFactory.CreateClient();
                    client.BaseAddress = new Uri(baseUrl);
                    client.Timeout = TimeSpan.FromSeconds(10);

                    // ✅ Transform to TechWorldAPI format: List<QuotationRequest>
                    var techWorldRequests = new List<object>();
                    foreach (var item in request.Items)
                    {
                        techWorldRequests.Add(new
                        {
                            GlobalId = item.GlobalId,
                            RequestedQuantity = item.Quantity
                        });
                    }

                    _logger.LogInformation("Sending request to {Name}: {Request}", name, System.Text.Json.JsonSerializer.Serialize(techWorldRequests));

                    // ✅ correct endpoint for TechWorldAPI
                    using var resp = await client.PostAsJsonAsync("api/quotation/get-quotation", techWorldRequests);
                    
                    if (!resp.IsSuccessStatusCode)
                    {
                        _logger.LogWarning("Distributor {Name} returned {Status}", name, resp.StatusCode);
                        continue;
                    }

                    // ✅ strong-typed read matching TechWorldAPI response
                    var data = await resp.Content.ReadFromJsonAsync<List<TechWorldQuotationResponse>>() ?? new();

                    _logger.LogInformation("Received {Count} quotations from {Name}", data.Count, name);

                    // map to your hub DTO
                    results.AddRange(data.Select(x => new QuotationResponseDTO
                    {
                        Distributor = name,
                        GlobalId = x.GlobalId,
                        ProductId = x.ProductId,
                        Price = x.Price,
                        AvailableQuantity = x.Inventory,
                        EstimatedDeliveryDays = 3, // Default delivery days
                        ProductName = x.ProductName,
                        Thumbnail = x.Thumbnail
                    }));

                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError(ex, "Failed to reach distributor {Name} at {Url}", name, baseUrl);
                    // Skip this distributor instead of providing mock data
                }
                catch (TaskCanceledException ex)
                {
                    _logger.LogError(ex, "Request to distributor {Name} timed out", name);
                    // Skip this distributor instead of providing mock data
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error calling distributor {Name}", name);
                    // Skip this distributor instead of providing mock data
                }
            }

            _logger.LogInformation("Total quotations collected: {Count}", results.Count);
            return results;
        }

    }
}
