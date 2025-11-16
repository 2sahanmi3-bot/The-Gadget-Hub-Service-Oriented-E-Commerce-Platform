using Microsoft.AspNetCore.Mvc;
using GadgetHubAPI.DTO;
using GadgetHubAPI.Services;
using GadgetHubAPI.Data;

namespace GadgetHubAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuotationController : ControllerBase
    {
        private readonly QuotationService _quotationService;

        public QuotationController(QuotationService quotationService)
        {
            _quotationService = quotationService;
        }

        /// <summary>
        /// Get quotations for specific products from all distributors
        /// </summary>
        /// <param name="globalIds">Comma-separated list of Global IDs</param>
        /// <param name="quantities">Comma-separated list of quantities (optional, defaults to 1)</param>
        /// <returns>List of quotations from all distributors</returns>
        [HttpGet]
        public async Task<IActionResult> GetQuotations([FromQuery] string globalIds, [FromQuery] string? quantities = null)
        {
            if (string.IsNullOrWhiteSpace(globalIds))
            {
                return BadRequest("GlobalIds parameter is required");
            }

            try
            {
                // Parse global IDs
                var globalIdList = globalIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(id => id.Trim())
                    .Where(id => !string.IsNullOrWhiteSpace(id))
                    .ToList();

                if (!globalIdList.Any())
                {
                    return BadRequest("At least one valid GlobalId is required");
                }

                // Parse quantities (default to 1 if not provided)
                var quantityList = new List<int>();
                if (!string.IsNullOrWhiteSpace(quantities))
                {
                    var quantityStrings = quantities.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    if (quantityStrings.Length != globalIdList.Count)
                    {
                        return BadRequest("Number of quantities must match number of GlobalIds");
                    }

                    foreach (var qtyStr in quantityStrings)
                    {
                        if (int.TryParse(qtyStr.Trim(), out int qty) && qty > 0)
                        {
                            quantityList.Add(qty);
                        }
                        else
                        {
                            return BadRequest($"Invalid quantity: {qtyStr}");
                        }
                    }
                }
                else
                {
                    // Default to quantity 1 for each product
                    quantityList = Enumerable.Repeat(1, globalIdList.Count).ToList();
                }

                // Create quotation request
                var request = new QuotationRequestDTO
                {
                    Items = globalIdList.Zip(quantityList, (globalId, quantity) => new QuotationItemDTO
                    {
                        GlobalId = globalId,
                        Quantity = quantity
                    }).ToList()
                };

                // Get quotations from all distributors
                var quotes = await _quotationService.GetQuotationsAsync(request);

                return Ok(new
                {
                    RequestedProducts = request.Items.Select(item => new { GlobalId = item.GlobalId, Quantity = item.Quantity }),
                    TotalQuotations = quotes.Count,
                    Quotations = quotes.GroupBy(q => q.GlobalId).Select(g => new
                    {
                        GlobalId = g.Key,
                        RequestedQuantity = request.Items.First(item => item.GlobalId == g.Key).Quantity,
                        AvailableQuotes = g.Select(q => new
                        {
                            Distributor = q.Distributor,
                            ProductId = q.ProductId,
                            Price = q.Price,
                            AvailableQuantity = q.AvailableQuantity,
                            EstimatedDeliveryDays = q.EstimatedDeliveryDays,
                            ProductName = q.ProductName,
                            Thumbnail = q.Thumbnail
                        }).ToList(),
                        BestQuote = g.OrderBy(x => x.Price).ThenBy(x => x.EstimatedDeliveryDays).First()
                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "Internal server error", Message = ex.Message });
            }
        }

        /// <summary>
        /// Get quotations for a single product by Global ID
        /// </summary>
        /// <param name="globalId">The Global ID of the product</param>
        /// <param name="quantity">Quantity needed (optional, defaults to 1)</param>
        /// <returns>Quotations for the specified product</returns>
        [HttpGet("{globalId}")]
        public async Task<IActionResult> GetQuotationByGlobalId(string globalId, [FromQuery] int quantity = 1)
        {
            if (string.IsNullOrWhiteSpace(globalId))
            {
                return BadRequest("GlobalId is required");
            }

            if (quantity <= 0)
            {
                return BadRequest("Quantity must be greater than 0");
            }

            try
            {
                var request = new QuotationRequestDTO
                {
                    Items = new List<QuotationItemDTO>
                    {
                        new QuotationItemDTO
                        {
                            GlobalId = globalId,
                            Quantity = quantity
                        }
                    }
                };

                var quotes = await _quotationService.GetQuotationsAsync(request);

                if (!quotes.Any())
                {
                    return NotFound($"No quotations found for product {globalId}");
                }

                var bestQuote = quotes.OrderBy(x => x.Price).ThenBy(x => x.EstimatedDeliveryDays).First();

                return Ok(new
                {
                    GlobalId = globalId,
                    RequestedQuantity = quantity,
                    TotalQuotations = quotes.Count,
                    BestQuote = new
                    {
                        Distributor = bestQuote.Distributor,
                        ProductId = bestQuote.ProductId,
                        Price = bestQuote.Price,
                        AvailableQuantity = bestQuote.AvailableQuantity,
                        EstimatedDeliveryDays = bestQuote.EstimatedDeliveryDays,
                        ProductName = bestQuote.ProductName,
                        Thumbnail = bestQuote.Thumbnail
                    },
                    AllQuotes = quotes.Select(q => new
                    {
                        Distributor = q.Distributor,
                        ProductId = q.ProductId,
                        Price = q.Price,
                        AvailableQuantity = q.AvailableQuantity,
                        EstimatedDeliveryDays = q.EstimatedDeliveryDays,
                        ProductName = q.ProductName,
                        Thumbnail = q.Thumbnail
                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "Internal server error", Message = ex.Message });
            }
        }

        /// <summary>
        /// Get detailed product information with best quote for product details page
        /// </summary>
        /// <param name="productId">The Product ID of the product</param>
        /// <param name="quantity">Quantity needed (optional, defaults to 1)</param>
        /// <returns>Detailed product information with best quote</returns>
        [HttpGet("product-details/{productId}")]
        public async Task<IActionResult> GetProductDetails(int productId, [FromQuery] int quantity = 1)
        {
            if (productId <= 0)
            {
                return BadRequest("ProductId must be greater than 0");
            }

            if (quantity <= 0)
            {
                return BadRequest("Quantity must be greater than 0");
            }

            try
            {
                // First, get the product from our database to get the GlobalId
                var productRepo = HttpContext.RequestServices.GetRequiredService<ProductRepo>();
                var product = await productRepo.GetByIdAsync(productId);
                
                if (product == null)
                {
                    return NotFound(new { 
                        Error = "Product not found", 
                        Message = $"Product with ID {productId} not found in our database",
                        ProductId = productId
                    });
                }

                var request = new QuotationRequestDTO
                {
                    Items = new List<QuotationItemDTO>
                    {
                        new QuotationItemDTO
                        {
                            GlobalId = product.GlobalId,
                            Quantity = quantity
                        }
                    }
                };

                var quotes = await _quotationService.GetQuotationsAsync(request);

                if (!quotes.Any())
                {
                    return NotFound(new { 
                        Error = "Product not found", 
                        Message = $"No quotations found for product {product.GlobalId}",
                        ProductId = productId,
                        GlobalId = product.GlobalId
                    });
                }

                // Find the best quote (lowest price, then fastest delivery)
                var bestQuote = quotes.OrderBy(x => x.Price).ThenBy(x => x.EstimatedDeliveryDays).First();

                return Ok(new
                {
                    ProductId = productId,
                    GlobalId = product.GlobalId,
                    RequestedQuantity = quantity,
                    ProductName = bestQuote.ProductName,
                    Description = product.Description ?? $"High-quality product with excellent features and performance. Global ID: {product.GlobalId}",
                    BestQuote = new
                    {
                        Distributor = bestQuote.Distributor,
                        ProductId = bestQuote.ProductId,
                        Price = bestQuote.Price,
                        AvailableQuantity = bestQuote.AvailableQuantity,
                        EstimatedDeliveryDays = bestQuote.EstimatedDeliveryDays,
                        ProductName = bestQuote.ProductName,
                        Thumbnail = bestQuote.Thumbnail
                    },
                    AllQuotes = quotes.Select(q => new
                    {
                        Distributor = q.Distributor,
                        ProductId = q.ProductId,
                        Price = q.Price,
                        AvailableQuantity = q.AvailableQuantity,
                        EstimatedDeliveryDays = q.EstimatedDeliveryDays,
                        ProductName = q.ProductName,
                        Thumbnail = q.Thumbnail
                    }).ToList(),
                    TotalQuotations = quotes.Count
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    Error = "Internal server error", 
                    Message = ex.Message,
                    ProductId = productId
                });
            }
        }

        [HttpPost("compare")]
        public async Task<IActionResult> Compare([FromBody] QuotationRequestDTO request)
        {
            if (request.Items == null || !request.Items.Any())
            {
                return BadRequest("Items array is required and cannot be empty");
            }

            var quotes = await _quotationService.GetQuotationsAsync(request);
            var grouped = quotes.GroupBy(q => q.GlobalId).Select(g =>
            {
                var best = g.OrderBy(x => x.Price).ThenBy(x => x.EstimatedDeliveryDays).First();
                var requestedItem = request.Items.First(item => item.GlobalId == g.Key);
                return new
                {
                    GlobalId = g.Key,
                    RequestedQuantity = requestedItem.Quantity,
                    BestDistributor = best.Distributor,
                    BestPrice = best.Price,
                    EstimatedDeliveryDays = best.EstimatedDeliveryDays,
                    AllQuotes = g.Select(q => new
                    {
                        Distributor = q.Distributor,
                        ProductId = q.ProductId,
                        Price = q.Price,
                        AvailableQuantity = q.AvailableQuantity,
                        EstimatedDeliveryDays = q.EstimatedDeliveryDays,
                        ProductName = q.ProductName,
                        Thumbnail = q.Thumbnail
                    }).ToList()
                };
            });

            return Ok(new
            {
                RequestedItems = request.Items.Select(item => new { GlobalId = item.GlobalId, Quantity = item.Quantity }),
                TotalQuotations = quotes.Count,
                Results = grouped.ToList()
            });
        }
    }
}
