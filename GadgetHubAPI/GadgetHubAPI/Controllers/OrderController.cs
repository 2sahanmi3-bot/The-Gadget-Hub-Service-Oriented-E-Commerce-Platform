using GadgetHubAPI.DTO;
using GadgetHubAPI.Services;
using GadgetHubAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace GadgetHubAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderService orderService, ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDTO orderDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var order = await _orderService.CreateOrderAsync(orderDto);
                return CreatedAtAction(nameof(GetOrder), new { id = order.OrderId }, order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order");
                return StatusCode(500, new { message = "An error occurred while creating the order" });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null)
                {
                    return NotFound(new { message = $"Order {id} not found" });
                }

                return Ok(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving order {OrderId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the order" });
            }
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetOrdersByCustomer(int customerId)
        {
            try
            {
                var orders = await _orderService.GetOrdersByCustomerIdAsync(customerId);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving orders for customer {CustomerId}", customerId);
                return StatusCode(500, new { message = "An error occurred while retrieving orders" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            try
            {
                var orders = await _orderService.GetAllOrdersAsync();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all orders");
                return StatusCode(500, new { message = "An error occurred while retrieving orders" });
            }
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDTO statusDto)
        {
            try
            {
                var order = await _orderService.UpdateOrderStatusAsync(id, statusDto.Status);
                return Ok(order);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order {OrderId} status", id);
                return StatusCode(500, new { message = "An error occurred while updating the order status" });
            }
        }

        [HttpPost("{id}/process")]
        public async Task<IActionResult> ProcessOrder(int id)
        {
            try
            {
                var success = await _orderService.ProcessOrderWithDistributorsAsync(id);
                if (success)
                {
                    return Ok(new { message = "Order processed successfully" });
                }
                else
                {
                    return BadRequest(new { message = "Failed to process order with distributors" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing order {OrderId}", id);
                return StatusCode(500, new { message = "An error occurred while processing the order" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            try
            {
                var success = await _orderService.DeleteOrderAsync(id);
                if (success)
                {
                    return Ok(new { message = "Order deleted successfully" });
                }
                else
                {
                    return NotFound(new { message = $"Order {id} not found" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting order {OrderId}", id);
                return StatusCode(500, new { message = "An error occurred while deleting the order" });
            }
        }
    }

    public class UpdateOrderStatusDTO
    {
        public OrderStatus Status { get; set; }
    }
}