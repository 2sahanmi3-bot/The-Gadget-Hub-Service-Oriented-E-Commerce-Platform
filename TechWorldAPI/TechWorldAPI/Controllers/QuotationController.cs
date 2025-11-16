using Microsoft.AspNetCore.Mvc;
using TechWorldAPI.Data;
using TechWorldAPI.DTO;

namespace TechWorldAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuotationController : ControllerBase
    {
        private readonly ProductRepo _quotationRepo;
        private readonly ILogger<QuotationController> _logger;

        public QuotationController(ProductRepo repo, ILogger<QuotationController> logger)
        {
            _quotationRepo = repo;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<List<QuotationResponse>>> GetQuotation(List<QuotationRequest> quoteRequests)
        {
            try
            {
                _logger.LogInformation("Processing quotation request for {RequestCount} items", quoteRequests?.Count ?? 0);
                var responses = new List<QuotationResponse>();

                foreach (var request in quoteRequests)
                {
                    var key = request?.GlobalId?.Trim();
                    if (string.IsNullOrWhiteSpace(key)) continue;
                    
                    _logger.LogDebug("Looking up product with GlobalId: {GlobalId}", key);
                    // Use async method to avoid blocking
                    var product = await _quotationRepo.GetByGlobalIdAsync(key);
                        
                    if (product != null)
                    {
                        responses.Add(new QuotationResponse
                        {
                            ProductId = product.ProductId,
                            GlobalId = product.GlobalId,
                            ProductName = product.ItemName,
                            ProductDetails = product.ProductDetails,
                            Thumbnail = product.Thumbnail,
                            UnitPrice = product.UnitPrice,
                            Inventory = product.Inventory,
                            Supplier = "TechWorld",
                            SupplierAddress = "Colombo",
                            IssuedAt = DateTime.Now,
                            Quantity = request?.RequestedQuantity ?? 0,
                            Price = product.UnitPrice * (request?.RequestedQuantity ?? 0),
                            Description = product.ProductDetails
                        });
                    }
                }
                _logger.LogInformation("Successfully processed {ResponseCount} quotations", responses.Count);
                return Ok(responses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing quotation request for {RequestCount} items", quoteRequests?.Count ?? 0);
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        [HttpPost("get-quotation")]
        public async Task<ActionResult<List<QuotationResponse>>> GetQuotationLegacy(List<QuotationRequest> quoteRequests)
        {
            return await GetQuotation(quoteRequests);
        }
    }
}