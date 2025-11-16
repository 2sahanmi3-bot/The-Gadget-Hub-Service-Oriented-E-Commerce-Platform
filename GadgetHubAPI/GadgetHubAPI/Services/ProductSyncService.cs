using System.Net.Http.Json;
using GadgetHubAPI.Data;
using GadgetHubAPI.DTO;
using GadgetHubAPI.Models;
using Microsoft.Extensions.Logging;

namespace GadgetHubAPI.Services
{
    public class ProductSyncService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _config;
        private readonly ILogger<ProductSyncService> _logger;
        private readonly ProductRepo _productRepo;

        public ProductSyncService(
            IHttpClientFactory clientFactory, 
            IConfiguration config, 
            ILogger<ProductSyncService> logger,
            ProductRepo productRepo)
        {
            _clientFactory = clientFactory;
            _config = config;
            _logger = logger;
            _productRepo = productRepo;
        }

        // TechWorld API response structure
        private sealed class TechWorldProductResponse
        {
            public int ProductId { get; set; }
            public string GlobalId { get; set; } = "";
            public string ItemName { get; set; } = "";
            public string ProductDetails { get; set; } = "";
            public string Thumbnail { get; set; } = "";
            public decimal UnitPrice { get; set; }
            public int Inventory { get; set; }
            public string ProductCategory { get; set; } = "";
        }

        public async Task<ProductSyncResponseDTO> SyncProductByGlobalIdAsync(ProductSyncRequestDTO request)
        {
            try
            {
                _logger.LogInformation("Starting product sync for GlobalId: {GlobalId}", request.GlobalId);

                // Try to fetch from all distributors and find the best product
                var distributors = GetDistributorUrls();
                var productsFound = new List<(string Distributor, Product Product)>();

                foreach (var (name, url) in distributors)
                {
                    try
                    {
                        _logger.LogInformation("Attempting to fetch product from {Distributor} at {Url}", name, url);

                        var product = await FetchProductFromDistributorAsync(request.GlobalId, name, url);
                        if (product != null)
                        {
                            productsFound.Add((name, product));
                            _logger.LogInformation("Found product {GlobalId} from {Distributor}", request.GlobalId, name);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to fetch product from {Distributor}", name);
                    }
                }

                if (!productsFound.Any())
                {
                    return new ProductSyncResponseDTO
                    {
                        Success = false,
                        Message = $"Product with GlobalId '{request.GlobalId}' not found in any distributor"
                    };
                }

                // Find the best product (prefer TechWorld, then lowest price, then highest stock)
                var bestProduct = productsFound
                    .OrderByDescending(p => p.Distributor == "TechWorld") // Prefer TechWorld
                    .ThenBy(p => p.Product.Price) // Then lowest price
                    .ThenByDescending(p => p.Product.Stock) // Then highest stock
                    .First();

                // Save the best product to database (if not already exists)
                var existingProduct = await _productRepo.GetByGlobalIdAsync(request.GlobalId);
                if (existingProduct == null)
                {
                    // Ensure ImageUrl is set properly before saving
                    if (string.IsNullOrWhiteSpace(bestProduct.Product.ImageUrl))
                    {
                        bestProduct.Product.ImageUrl = "";
                    }
                    
                    await _productRepo.AddAsync(bestProduct.Product);
                    _logger.LogInformation("Saved best product {GlobalId} from {Distributor} to database with ImageUrl: {ImageUrl}", 
                        request.GlobalId, bestProduct.Distributor, bestProduct.Product.ImageUrl);
                }
                else
                {
                    // Update existing product with ImageUrl if it's empty
                    if (string.IsNullOrWhiteSpace(existingProduct.ImageUrl))
                    {
                        existingProduct.ImageUrl = "";
                        await _productRepo.UpdateAsync(existingProduct);
                        _logger.LogInformation("Updated existing product {GlobalId} with ImageUrl: {ImageUrl}", 
                            request.GlobalId, existingProduct.ImageUrl);
                    }
                }

                return new ProductSyncResponseDTO
                {
                    Success = true,
                    Message = $"Best product found from {bestProduct.Distributor}",
                    ProductName = bestProduct.Product.Name,
                    Description = bestProduct.Product.Description,
                    ImageUrl = bestProduct.Product.ImageUrl ?? "",
                    DistributorUsed = bestProduct.Distributor
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error syncing product {GlobalId}", request.GlobalId);
                return new ProductSyncResponseDTO
                {
                    Success = false,
                    Message = $"Error syncing product: {ex.Message}"
                };
            }
        }

        private async Task<Product?> FetchProductFromDistributorAsync(string globalId, string distributorName, string baseUrl)
        {
            var client = _clientFactory.CreateClient();
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(10);

            try
            {
                // Try different endpoints based on distributor
                if (distributorName.Equals("TechWorld", StringComparison.OrdinalIgnoreCase))
                {
                    return await FetchFromTechWorldAsync(client, globalId);
                }
                // Add other distributors here as needed
                else
                {
                    return await FetchFromGenericEndpointAsync(client, globalId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching from {Distributor}", distributorName);
                return null;
            }
        }

        private async Task<Product?> FetchFromTechWorldAsync(HttpClient client, string globalId)
        {
            // Try to get product by GlobalId
            var response = await client.GetAsync($"api/Product/{globalId}");
            if (response.IsSuccessStatusCode)
            {
                var techWorldProduct = await response.Content.ReadFromJsonAsync<TechWorldProductResponse>();
                if (techWorldProduct != null)
                {
                    var product = new Product
                    {
                        GlobalId = techWorldProduct.GlobalId,
                        Name = techWorldProduct.ItemName,
                        Price = techWorldProduct.UnitPrice,
                        Stock = techWorldProduct.Inventory,
                        Description = techWorldProduct.ProductDetails,
                        ImageUrl = techWorldProduct.Thumbnail
                    };
                    
                    // If Thumbnail is empty, leave it empty
                    if (string.IsNullOrWhiteSpace(product.ImageUrl))
                    {
                        product.ImageUrl = "";
                    }
                    
                    _logger.LogInformation("Fetched product {GlobalId} from TechWorld with ImageUrl: {ImageUrl}", 
                        globalId, product.ImageUrl);
                    
                    return product;
                }
            }
            return null;
        }

        private async Task<Product?> FetchFromGenericEndpointAsync(HttpClient client, string globalId)
        {
            // Generic implementation for other distributors
            // This can be customized based on each distributor's API structure
            try
            {
                var response = await client.GetAsync($"api/Product/{globalId}");
                if (response.IsSuccessStatusCode)
                {
                    var productData = await response.Content.ReadFromJsonAsync<object>();
                    // Implement generic mapping logic here
                    // For now, return null to indicate not implemented
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in generic product fetch");
            }
            return null;
        }

        private (string Name, string Url)[] GetDistributorUrls()
        {
            var distributors = new List<(string Name, string Url)>();

            var techWorld = _config["Distributors:TechWorld"];
            var electroCom = _config["Distributors:ElectroCom"];
            var gadgetCentral = _config["Distributors:GadgetCentral"];

            if (!string.IsNullOrWhiteSpace(techWorld))
                distributors.Add(("TechWorld", techWorld));
            if (!string.IsNullOrWhiteSpace(electroCom))
                distributors.Add(("ElectroCom", electroCom));
            if (!string.IsNullOrWhiteSpace(gadgetCentral))
                distributors.Add(("GadgetCentral", gadgetCentral));

            return distributors.ToArray();
        }

    }
}
