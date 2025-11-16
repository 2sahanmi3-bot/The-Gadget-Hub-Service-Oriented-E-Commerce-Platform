using GadgetHubAPI.Data;
using GadgetHubAPI.DTO;
using GadgetHubAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace GadgetHubAPI.Services
{
    public interface ICartService
    {
        Task<CartResponseDTO> GetCartByCustomerIdAsync(int customerId);
        Task<CartResponseDTO> AddToCartAsync(AddToCartDTO addToCartDto);
        Task<CartResponseDTO> UpdateCartItemAsync(UpdateCartItemDTO updateDto);
        Task<CartResponseDTO> RemoveFromCartAsync(RemoveFromCartDTO removeDto);
        Task<bool> ClearCartAsync(int customerId);
        Task<CartResponseDTO> GetOrCreateCartAsync(int customerId);
    }

    public class CartService : ICartService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CartService> _logger;

        public CartService(AppDbContext context, ILogger<CartService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<CartResponseDTO> GetOrCreateCartAsync(int customerId)
        {
            try
            {
                var cart = await _context.ShoppingCarts
                    .Include(sc => sc.CartItems)
                    .FirstOrDefaultAsync(sc => sc.CustomerId == customerId);

                if (cart == null)
                {
                    cart = new ShoppingCart
                    {
                        CustomerId = customerId,
                        CreatedDate = DateTime.UtcNow,
                        LastModifiedDate = DateTime.UtcNow
                    };

                    _context.ShoppingCarts.Add(cart);
                    await _context.SaveChangesAsync();
                }

                return MapToCartResponseDTO(cart);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting or creating cart for customer {CustomerId}", customerId);
                throw;
            }
        }

        public async Task<CartResponseDTO> GetCartByCustomerIdAsync(int customerId)
        {
            try
            {
                var cart = await _context.ShoppingCarts
                    .Include(sc => sc.CartItems)
                    .FirstOrDefaultAsync(sc => sc.CustomerId == customerId);

                if (cart == null)
                {
                    return await GetOrCreateCartAsync(customerId);
                }

                return MapToCartResponseDTO(cart);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cart for customer {CustomerId}", customerId);
                throw;
            }
        }

        public async Task<CartResponseDTO> AddToCartAsync(AddToCartDTO addToCartDto)
        {
            try
            {
                var cart = await _context.ShoppingCarts
                    .Include(sc => sc.CartItems)
                    .FirstOrDefaultAsync(sc => sc.CustomerId == addToCartDto.CustomerId);

                if (cart == null)
                {
                    cart = new ShoppingCart
                    {
                        CustomerId = addToCartDto.CustomerId,
                        CreatedDate = DateTime.UtcNow,
                        LastModifiedDate = DateTime.UtcNow
                    };
                    _context.ShoppingCarts.Add(cart);
                    await _context.SaveChangesAsync();
                }

                // Check if item already exists in cart
                var existingItem = cart.CartItems.FirstOrDefault(ci => ci.GlobalId == addToCartDto.Item.GlobalId);

                if (existingItem != null)
                {
                    // Update quantity
                    existingItem.Quantity += addToCartDto.Item.Quantity;
                    existingItem.TotalPrice = existingItem.Quantity * existingItem.UnitPrice;
                }
                else
                {
                    // Add new item
                    var cartItem = new CartItem
                    {
                        CartId = cart.CartId,
                        GlobalId = addToCartDto.Item.GlobalId,
                        ProductName = addToCartDto.Item.ProductName,
                        Quantity = addToCartDto.Item.Quantity,
                        UnitPrice = addToCartDto.Item.UnitPrice,
                        TotalPrice = addToCartDto.Item.Quantity * addToCartDto.Item.UnitPrice,
                        ProductDescription = addToCartDto.Item.ProductDescription,
                        ProductImageUrl = addToCartDto.Item.ProductImageUrl,
                        AddedDate = DateTime.UtcNow
                    };

                    cart.CartItems.Add(cartItem);
                }

                cart.LastModifiedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Item {GlobalId} added to cart for customer {CustomerId}", 
                    addToCartDto.Item.GlobalId, addToCartDto.CustomerId);

                return MapToCartResponseDTO(cart);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding item to cart for customer {CustomerId}", addToCartDto.CustomerId);
                throw;
            }
        }

        public async Task<CartResponseDTO> UpdateCartItemAsync(UpdateCartItemDTO updateDto)
        {
            try
            {
                var cartItem = await _context.CartItems
                    .Include(ci => ci.ShoppingCart)
                    .ThenInclude(sc => sc.CartItems)
                    .FirstOrDefaultAsync(ci => ci.CartItemId == updateDto.CartItemId);

                if (cartItem == null)
                    throw new ArgumentException($"Cart item {updateDto.CartItemId} not found");

                cartItem.Quantity = updateDto.Quantity;
                cartItem.TotalPrice = cartItem.Quantity * cartItem.UnitPrice;

                cartItem.ShoppingCart.LastModifiedDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Cart item {CartItemId} updated to quantity {Quantity}", 
                    updateDto.CartItemId, updateDto.Quantity);

                return MapToCartResponseDTO(cartItem.ShoppingCart);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cart item {CartItemId}", updateDto.CartItemId);
                throw;
            }
        }

        public async Task<CartResponseDTO> RemoveFromCartAsync(RemoveFromCartDTO removeDto)
        {
            try
            {
                var cartItem = await _context.CartItems
                    .Include(ci => ci.ShoppingCart)
                    .ThenInclude(sc => sc.CartItems)
                    .FirstOrDefaultAsync(ci => ci.CartItemId == removeDto.CartItemId);

                if (cartItem == null)
                    throw new ArgumentException($"Cart item {removeDto.CartItemId} not found");

                var cart = cartItem.ShoppingCart;
                _context.CartItems.Remove(cartItem);
                cart.LastModifiedDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Cart item {CartItemId} removed from cart", removeDto.CartItemId);

                return MapToCartResponseDTO(cart);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cart item {CartItemId}", removeDto.CartItemId);
                throw;
            }
        }

        public async Task<bool> ClearCartAsync(int customerId)
        {
            try
            {
                var cart = await _context.ShoppingCarts
                    .Include(sc => sc.CartItems)
                    .FirstOrDefaultAsync(sc => sc.CustomerId == customerId);

                if (cart != null)
                {
                    _context.CartItems.RemoveRange(cart.CartItems);
                    cart.LastModifiedDate = DateTime.UtcNow;
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Cart cleared for customer {CustomerId}", customerId);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing cart for customer {CustomerId}", customerId);
                return false;
            }
        }

        private CartResponseDTO MapToCartResponseDTO(ShoppingCart cart)
        {
            return new CartResponseDTO
            {
                CartId = cart.CartId,
                CustomerId = cart.CustomerId,
                CreatedDate = cart.CreatedDate,
                LastModifiedDate = cart.LastModifiedDate,
                TotalAmount = cart.CartItems.Sum(ci => ci.TotalPrice),
                TotalItems = cart.CartItems.Sum(ci => ci.Quantity),
                Items = cart.CartItems.Select(ci => new CartItemResponseDTO
                {
                    CartItemId = ci.CartItemId,
                    GlobalId = ci.GlobalId,
                    ProductName = ci.ProductName,
                    Quantity = ci.Quantity,
                    UnitPrice = ci.UnitPrice,
                    TotalPrice = ci.TotalPrice,
                    ProductDescription = ci.ProductDescription,
                    ProductImageUrl = ci.ProductImageUrl,
                    AddedDate = ci.AddedDate
                }).ToList()
            };
        }
    }
}


