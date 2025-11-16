using Microsoft.AspNetCore.Mvc;
using GadgetCentralAPI.Data;
using GadgetCentralAPI.DTO;

namespace GadgetCentralAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuotationController : ControllerBase
    {
        private readonly ProductRepo _quotationRepo;

        public QuotationController(ProductRepo repo)
        {
            _quotationRepo = repo;
        }

        [HttpPost]
        public ActionResult<List<QuotationResponse>> GetQuotation(List<QuotationRequest> quoteRequests)
        {
            var responses = new List<QuotationResponse>();

            foreach (var request in quoteRequests)
            {
                var key = request?.GlobalId?.Trim();
                if (string.IsNullOrWhiteSpace(key)) continue;
                
                // ✅ Fixed: Use simple equality comparison that EF Core can translate
                var product = _quotationRepo.GetProducts()
                    .FirstOrDefault(p => p.GlobalId == key);
                    
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
                        Supplier = "GadgetCentral",
                        SupplierAddress = "Galle",
                        IssuedAt = DateTime.Now,
                        Quantity = request?.RequestedQuantity ?? 0,
                        Price = product.UnitPrice * (request?.RequestedQuantity ?? 0),
                        Description = product.ProductDetails
                    });
                }
            }
            return Ok(responses);
        }

        [HttpPost("get-quotation")]
        public ActionResult<List<QuotationResponse>> GetQuotationLegacy(List<QuotationRequest> quoteRequests)
        {
            return GetQuotation(quoteRequests).Result;
        }
    }
}