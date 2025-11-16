using Microsoft.AspNetCore.Mvc;
using TechWorldAPI.Models;
using TechWorldAPI.Data;

namespace TechWorldAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly ProductRepo _productRepo;
        private readonly ILogger<OrderController> _logger;

        public OrderController(ProductRepo productRepo, ILogger<OrderController> logger)
        {
            _productRepo = productRepo;
            _logger = logger;
        }

        /// <summary>
        /// Get all orders
        /// </summary>
        /// <returns>List of all orders</returns>
        [HttpGet]
        public ActionResult<object> GetAllOrders()
        {
            // In a real system, you would have an Order repository
            // For now, return a mock response
            return Ok(new
            {
                Message = "Orders endpoint - In a real system, this would return all orders",
                Orders = new List<object>()
            });
        }

        /// <summary>
        /// Get order by ID
        /// </summary>
        /// <param name="id">Order ID</param>
        /// <returns>Order details</returns>
        [HttpGet("{id}")]
        public ActionResult<object> GetOrderById(int id)
        {
            // In a real system, you would fetch from database
            return Ok(new
            {
                Message = $"Order {id} details - In a real system, this would return order details",
                OrderId = id,
                Status = "Mock Order"
            });
        }

        /// <summary>
        /// Place an order with TechWorld distributor
        /// </summary>
        /// <param name="request">Order request</param>
        /// <returns>Order confirmation</returns>
        [HttpPost("place")]
        public async Task<ActionResult<object>> PlaceOrder([FromBody] PlaceOrderRequest request)
        {
            try
            {
                _logger.LogInformation("Processing order {OrderId} with TechWorld", request.OrderId);

                // Validate products and quantities
                var orderItems = new List<object>();
                decimal totalAmount = 0;

                foreach (var item in request.Items)
                {
                    var product = await _productRepo.GetByGlobalIdAsync(item.GlobalId);
                    if (product == null)
                    {
                        return BadRequest(new { Message = $"Product {item.GlobalId} not found" });
                    }

                    if (product.Inventory < item.Quantity)
                    {
                        return BadRequest(new { Message = $"Insufficient stock for product {item.GlobalId}" });
                    }

                    // Reduce stock
                    product.Inventory -= item.Quantity;
                    await _productRepo.UpdateAsync(product);

                    orderItems.Add(new
                    {
                        GlobalId = item.GlobalId,
                        ProductName = product.ItemName,
                        Quantity = item.Quantity,
                        UnitPrice = product.UnitPrice,
                        TotalPrice = item.Quantity * product.UnitPrice
                    });

                    totalAmount += item.Quantity * product.UnitPrice;
                }

                // Generate distributor order ID
                var distributorOrderId = $"TW{DateTime.UtcNow:yyyyMMddHHmmss}{new Random().Next(1000, 9999)}";

                _logger.LogInformation("Order {OrderId} confirmed with TechWorld, Distributor Order ID: {DistributorOrderId}", 
                    request.OrderId, distributorOrderId);

                return Ok(new
                {
                    Success = true,
                    OrderId = request.OrderId,
                    DistributorOrderId = distributorOrderId,
                    DistributorName = "TechWorld",
                    Status = "Confirmed",
                    TotalAmount = totalAmount,
                    ConfirmedAt = DateTime.UtcNow,
                    EstimatedDeliveryDays = 3, // TechWorld delivers in 3 days
                    Items = orderItems
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing order {OrderId}", request.OrderId);
                return StatusCode(500, new { Message = "Internal server error" });
            }
        }

        /// <summary>
        /// Update order status
        /// </summary>
        /// <param name="distributorOrderId">Distributor order ID</param>
        /// <param name="request">Status update</param>
        /// <returns>Success status</returns>
        [HttpPut("{distributorOrderId}/status")]
        public async Task<ActionResult> UpdateOrderStatus(string distributorOrderId, [FromBody] UpdateOrderStatusRequest request)
        {
            try
            {
                _logger.LogInformation("Updating order {DistributorOrderId} status to {Status}", 
                    distributorOrderId, request.Status);

                // In a real system, you would update the order status in the database
                // For now, we'll just log the status change

                return Ok(new
                {
                    Success = true,
                    DistributorOrderId = distributorOrderId,
                    Status = request.Status,
                    UpdatedAt = DateTime.UtcNow,
                    Message = "Order status updated successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order {DistributorOrderId} status", distributorOrderId);
                return StatusCode(500, new { Message = "Internal server error" });
            }
        }
    }

    public class PlaceOrderRequest
    {
        public int OrderId { get; set; }
        public List<OrderItemRequest> Items { get; set; } = new List<OrderItemRequest>();
        public DateTime OrderDate { get; set; }
    }

    public class OrderItemRequest
    {
        public string GlobalId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class UpdateOrderStatusRequest
    {
        public string Status { get; set; } = string.Empty;
        public string? TrackingNumber { get; set; }
        public string? Notes { get; set; }
    }
}
