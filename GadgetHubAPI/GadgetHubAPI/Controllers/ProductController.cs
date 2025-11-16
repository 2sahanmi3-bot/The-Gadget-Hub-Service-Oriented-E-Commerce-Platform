using Microsoft.AspNetCore.Mvc;
using GadgetHubAPI.DTO;
using GadgetHubAPI.Services;
using GadgetHubAPI.Data;

namespace GadgetHubAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ProductSyncService _productSyncService;
        private readonly ProductRepo _productRepo;

        public ProductController(ProductSyncService productSyncService, ProductRepo productRepo)
        {
            _productSyncService = productSyncService;
            _productRepo = productRepo;
        }

        /// <summary>
        /// Get the best product from any distributor by GlobalId
        /// </summary>
        /// <param name="request">Product sync request containing GlobalId</param>
        /// <returns>Product details: ProductName, Description, ImageUrl</returns>
        [HttpPost("sync")]
        public async Task<ActionResult<ProductSyncResponseDTO>> SyncProduct([FromBody] ProductSyncRequestDTO request)
        {
            if (string.IsNullOrWhiteSpace(request.GlobalId))
            {
                return BadRequest(new ProductSyncResponseDTO
                {
                    Success = false,
                    Message = "GlobalId is required"
                });
            }

            var result = await _productSyncService.SyncProductByGlobalIdAsync(request);
            
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        /// <summary>
        /// Get all products from GadgetHub database
        /// </summary>
        /// <returns>List of all products in the database</returns>
        [HttpGet]
        public async Task<ActionResult<List<ProductResponseDTO>>> GetAllProducts()
        {
            var products = await _productRepo.GetAllAsync();
            var response = products.Select(p => new ProductResponseDTO
            {
                ProductId = p.ProductId,
                GlobalId = p.GlobalId,
                Name = p.Name,
                UnitPrice = p.Price,
                Inventory = p.Stock,
                Description = p.Description,
                Category = p.Category,
                ImageUrl = p.ImageUrl,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.CreatedAt
            }).ToList();

            return Ok(response);
        }

        /// <summary>
        /// Get a specific product by GlobalId from GadgetHub database
        /// </summary>
        /// <param name="globalId">The GlobalId of the product</param>
        /// <returns>Product details or NotFound</returns>
        [HttpGet("{globalId}")]
        public async Task<ActionResult<ProductResponseDTO>> GetProduct(string globalId)
        {
            var product = await _productRepo.GetByGlobalIdAsync(globalId);
            if (product == null)
            {
                return NotFound($"Product with GlobalId '{globalId}' not found");
            }

            var response = new ProductResponseDTO
            {
                ProductId = product.ProductId,
                GlobalId = product.GlobalId,
                Name = product.Name,
                UnitPrice = product.Price,
                Inventory = product.Stock,
                Description = product.Description,
                Category = product.Category,
                ImageUrl = product.ImageUrl,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.CreatedAt
            };

            return Ok(response);
        }

        /// <summary>
        /// Delete a product from GadgetHub database
        /// </summary>
        /// <param name="globalId">The GlobalId of the product to delete</param>
        /// <returns>Success or NotFound</returns>
        [HttpDelete("{globalId}")]
        public async Task<ActionResult> DeleteProduct(string globalId)
        {
            var success = await _productRepo.DeleteAsync(globalId);
            if (!success)
            {
                return NotFound($"Product with GlobalId '{globalId}' not found");
            }

            return Ok(new { Message = $"Product '{globalId}' deleted successfully" });
        }

        /// <summary>
        /// Fix ImageUrl for existing products that have empty ImageUrl
        /// </summary>
        /// <returns>Number of products updated</returns>
        [HttpPost("fix-image-urls")]
        public async Task<ActionResult> FixImageUrls()
        {
            var products = await _productRepo.GetAllAsync();
            var productsToUpdate = products.Where(p => string.IsNullOrWhiteSpace(p.ImageUrl)).ToList();
            
            int updatedCount = 0;
            foreach (var product in productsToUpdate)
            {
                // Set default image URL based on product name or use generic placeholder
                product.ImageUrl = GetDefaultImageUrl(product.Name);
                await _productRepo.UpdateAsync(product);
                updatedCount++;
            }

            return Ok(new { 
                Message = $"Updated {updatedCount} products with ImageUrl",
                UpdatedCount = updatedCount,
                TotalProducts = products.Count
            });
        }


        private string GetDefaultImageUrl(string productName)
        {
            // Return empty string instead of placeholder image
            return "";
        }
    }
}
