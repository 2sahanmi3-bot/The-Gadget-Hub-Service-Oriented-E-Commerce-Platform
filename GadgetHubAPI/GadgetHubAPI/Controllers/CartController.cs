using GadgetHubAPI.DTO;
using GadgetHubAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace GadgetHubAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ILogger<CartController> _logger;

        public CartController(ICartService cartService, ILogger<CartController> logger)
        {
            _cartService = cartService;
            _logger = logger;
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetCart(int customerId)
        {
            try
            {
                var cart = await _cartService.GetCartByCustomerIdAsync(customerId);
                return Ok(cart);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cart for customer {CustomerId}", customerId);
                return StatusCode(500, new { message = "An error occurred while retrieving the cart" });
            }
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDTO addToCartDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var cart = await _cartService.AddToCartAsync(addToCartDto);
                return Ok(cart);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding item to cart");
                return StatusCode(500, new { message = "An error occurred while adding item to cart" });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateCartItem([FromBody] UpdateCartItemDTO updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var cart = await _cartService.UpdateCartItemAsync(updateDto);
                return Ok(cart);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cart item");
                return StatusCode(500, new { message = "An error occurred while updating cart item" });
            }
        }

        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveFromCart([FromBody] RemoveFromCartDTO removeDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var cart = await _cartService.RemoveFromCartAsync(removeDto);
                return Ok(cart);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing item from cart");
                return StatusCode(500, new { message = "An error occurred while removing item from cart" });
            }
        }

        [HttpDelete("customer/{customerId}/clear")]
        public async Task<IActionResult> ClearCart(int customerId)
        {
            try
            {
                var success = await _cartService.ClearCartAsync(customerId);
                if (success)
                {
                    return Ok(new { message = "Cart cleared successfully" });
                }
                else
                {
                    return BadRequest(new { message = "Failed to clear cart" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing cart for customer {CustomerId}", customerId);
                return StatusCode(500, new { message = "An error occurred while clearing the cart" });
            }
        }
    }
}


