using GadgetHubAPI.Data;
using GadgetHubAPI.DTO;
using GadgetHubAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace GadgetHubAPI.Services
{
    public interface IOrderService
    {
        Task<OrderResponseDTO> CreateOrderAsync(CreateOrderDTO orderDto);
        Task<OrderResponseDTO?> GetOrderByIdAsync(int orderId);
        Task<List<OrderResponseDTO>> GetOrdersByCustomerIdAsync(int customerId);
        Task<List<OrderResponseDTO>> GetAllOrdersAsync();
        Task<OrderResponseDTO> UpdateOrderStatusAsync(int orderId, OrderStatus status);
        Task<bool> ProcessOrderWithDistributorsAsync(int orderId);
        Task<bool> DeleteOrderAsync(int orderId);
    }

    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;
        private readonly QuotationService _quotationService;
        private readonly ILogger<OrderService> _logger;

        public OrderService(AppDbContext context, QuotationService quotationService, ILogger<OrderService> logger)
        {
            _context = context;
            _quotationService = quotationService;
            _logger = logger;
        }

        public async Task<OrderResponseDTO> CreateOrderAsync(CreateOrderDTO orderDto)
        {
            try
            {
                // Create the order
                var order = new Order
                {
                    OrderNumber = $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}",
                    CustomerId = orderDto.CustomerId,
                    OrderDate = DateTime.UtcNow,
                    Status = OrderStatus.Pending,
                    DeliveryAddress = orderDto.DeliveryAddress,
                    CustomerName = orderDto.CustomerName,
                    CustomerEmail = orderDto.CustomerEmail,
                    CustomerPhone = orderDto.CustomerPhone,
                    Notes = orderDto.Notes,
                    OrderItems = new List<OrderItem>()
                };

                // Add order items
                decimal totalAmount = 0;
                foreach (var itemDto in orderDto.Items)
                {
                    var orderItem = new OrderItem
                    {
                        GlobalId = itemDto.GlobalId,
                        ProductName = itemDto.ProductName,
                        Quantity = itemDto.Quantity,
                        UnitPrice = itemDto.UnitPrice,
                        ProductDescription = itemDto.ProductDescription,
                        ImageUrl = itemDto.ProductImageUrl
                    };
                    
                    order.OrderItems.Add(orderItem);
                    totalAmount += orderItem.TotalPrice;
                }

                order.TotalAmount = totalAmount;

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Order {OrderId} created for customer {CustomerId}", order.Id, order.CustomerId);

                // Process order with distributors
                await ProcessOrderWithDistributorsAsync(order.Id);

                return await GetOrderByIdAsync(order.Id) ?? throw new InvalidOperationException("Failed to retrieve created order");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order for customer {CustomerId}", orderDto.CustomerId);
                throw;
            }
        }

        public async Task<OrderResponseDTO?> GetOrderByIdAsync(int orderId)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .Include(o => o.OrderDistributions)
                    .FirstOrDefaultAsync(o => o.Id == orderId);

                if (order == null) return null;

                return new OrderResponseDTO
                {
                    OrderId = order.Id,
                    OrderNumber = order.OrderNumber,
                    CustomerId = order.CustomerId,
                    OrderDate = order.OrderDate,
                    Status = order.Status,
                    TotalAmount = order.TotalAmount,
                    DeliveryAddress = order.DeliveryAddress,
                    CustomerName = order.CustomerName,
                    CustomerEmail = order.CustomerEmail,
                    CustomerPhone = order.CustomerPhone,
                    EstimatedDeliveryDate = order.EstimatedDeliveryDate,
                    Notes = order.Notes,
                    Items = order.OrderItems.Select(oi => new OrderItemResponseDTO
                    {
                        OrderItemId = oi.Id,
                        GlobalId = oi.GlobalId,
                        ProductName = oi.ProductName,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice,
                        TotalPrice = oi.TotalPrice,
                        ProductDescription = oi.ProductDescription,
                        ProductImageUrl = oi.ImageUrl
                    }).ToList(),
                    Distributions = order.OrderDistributions.Select(od => new OrderDistributionResponseDTO
                    {
                        OrderDistributionId = od.Id,
                        DistributorName = od.DistributorName,
                        DistributorOrderId = od.DistributorOrderId,
                        DistributorTotalAmount = od.DistributorTotalAmount,
                        DistributorOrderDate = od.DistributorOrderDate,
                        DistributorNotes = od.DistributorNotes,
                        Status = od.Status
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving order {OrderId}", orderId);
                throw;
            }
        }

        public async Task<List<OrderResponseDTO>> GetOrdersByCustomerIdAsync(int customerId)
        {
            try
            {
                var orders = await _context.Orders
                    .Include(o => o.OrderItems)
                    .Include(o => o.OrderDistributions)
                    .Where(o => o.CustomerId == customerId)
                    .OrderByDescending(o => o.OrderDate)
                    .ToListAsync();

                return orders.Select(order => new OrderResponseDTO
                {
                    OrderId = order.Id,
                    OrderNumber = order.OrderNumber,
                    CustomerId = order.CustomerId,
                    OrderDate = order.OrderDate,
                    Status = order.Status,
                    TotalAmount = order.TotalAmount,
                    DeliveryAddress = order.DeliveryAddress,
                    CustomerName = order.CustomerName,
                    CustomerEmail = order.CustomerEmail,
                    CustomerPhone = order.CustomerPhone,
                    EstimatedDeliveryDate = order.EstimatedDeliveryDate,
                    Notes = order.Notes,
                    Items = order.OrderItems.Select(oi => new OrderItemResponseDTO
                    {
                        OrderItemId = oi.Id,
                        GlobalId = oi.GlobalId,
                        ProductName = oi.ProductName,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice,
                        TotalPrice = oi.TotalPrice,
                        ProductDescription = oi.ProductDescription,
                        ProductImageUrl = oi.ImageUrl
                    }).ToList(),
                    Distributions = order.OrderDistributions.Select(od => new OrderDistributionResponseDTO
                    {
                        OrderDistributionId = od.Id,
                        DistributorName = od.DistributorName,
                        DistributorOrderId = od.DistributorOrderId,
                        DistributorTotalAmount = od.DistributorTotalAmount,
                        DistributorOrderDate = od.DistributorOrderDate,
                        DistributorNotes = od.DistributorNotes,
                        Status = od.Status
                    }).ToList()
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving orders for customer {CustomerId}", customerId);
                throw;
            }
        }

        public async Task<List<OrderResponseDTO>> GetAllOrdersAsync()
        {
            try
            {
                var orders = await _context.Orders
                    .Include(o => o.OrderItems)
                    .Include(o => o.OrderDistributions)
                    .OrderByDescending(o => o.OrderDate)
                    .ToListAsync();

                return orders.Select(order => new OrderResponseDTO
                {
                    OrderId = order.Id,
                    OrderNumber = order.OrderNumber,
                    CustomerId = order.CustomerId,
                    OrderDate = order.OrderDate,
                    Status = order.Status,
                    TotalAmount = order.TotalAmount,
                    DeliveryAddress = order.DeliveryAddress,
                    CustomerName = order.CustomerName,
                    CustomerEmail = order.CustomerEmail,
                    CustomerPhone = order.CustomerPhone,
                    EstimatedDeliveryDate = order.EstimatedDeliveryDate,
                    Notes = order.Notes,
                    Items = order.OrderItems.Select(oi => new OrderItemResponseDTO
                    {
                        OrderItemId = oi.Id,
                        GlobalId = oi.GlobalId,
                        ProductName = oi.ProductName,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice,
                        TotalPrice = oi.TotalPrice,
                        ProductDescription = oi.ProductDescription,
                        ProductImageUrl = oi.ImageUrl
                    }).ToList(),
                    Distributions = order.OrderDistributions.Select(od => new OrderDistributionResponseDTO
                    {
                        OrderDistributionId = od.Id,
                        DistributorName = od.DistributorName,
                        DistributorOrderId = od.DistributorOrderId,
                        DistributorTotalAmount = od.DistributorTotalAmount,
                        DistributorOrderDate = od.DistributorOrderDate,
                        DistributorNotes = od.DistributorNotes,
                        Status = od.Status
                    }).ToList()
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all orders");
                throw;
            }
        }

        public async Task<OrderResponseDTO> UpdateOrderStatusAsync(int orderId, OrderStatus status)
        {
            try
            {
                var order = await _context.Orders.FindAsync(orderId);
                if (order == null)
                    throw new ArgumentException($"Order {orderId} not found");

                order.Status = status;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Order {OrderId} status updated to {Status}", orderId, status);

                return await GetOrderByIdAsync(orderId) ?? throw new InvalidOperationException("Failed to retrieve updated order");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order {OrderId} status to {Status}", orderId, status);
                throw;
            }
        }

        public async Task<bool> ProcessOrderWithDistributorsAsync(int orderId)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .FirstOrDefaultAsync(o => o.Id == orderId);

                if (order == null)
                {
                    _logger.LogError("Order {OrderId} not found for processing", orderId);
                    return false;
                }

                // Create quotation request
                var quotationRequest = new QuotationRequestDTO
                {
                    Items = order.OrderItems.Select(oi => new QuotationItemDTO
                    {
                        GlobalId = oi.GlobalId,
                        Quantity = oi.Quantity
                    }).ToList()
                };

                // Get quotations from all distributors
                var quotations = await _quotationService.GetQuotationsAsync(quotationRequest);

                if (!quotations.Any())
                {
                    _logger.LogWarning("No quotations received for order {OrderId}", orderId);
                    await UpdateOrderStatusAsync(orderId, OrderStatus.Failed);
                    return false;
                }

                // Group quotations by distributor and select best options
                var distributorOrders = quotations
                    .GroupBy(q => q.Distributor)
                    .Select(g => new
                    {
                        Distributor = g.Key,
                        BestQuote = g.OrderBy(q => q.Price).First(),
                        TotalAmount = g.Sum(q => q.Price * q.AvailableQuantity)
                    })
                    .ToList();

                // Create order distributions
                foreach (var distributorOrder in distributorOrders)
                {
                    var orderDistribution = new OrderDistribution
                    {
                        OrderId = orderId,
                        DistributorName = distributorOrder.Distributor,
                        DistributorOrderId = $"ORD-{orderId}-{distributorOrder.Distributor}-{DateTime.UtcNow:yyyyMMddHHmmss}",
                        DistributorTotalAmount = distributorOrder.TotalAmount,
                        DistributorOrderDate = DateTime.UtcNow,
                        Status = OrderDistributionStatus.Pending
                    };

                    _context.OrderDistributions.Add(orderDistribution);
                }

                // Update order status
                order.Status = OrderStatus.Processing;
                order.EstimatedDeliveryDate = distributorOrders.Max(d => DateTime.UtcNow.AddDays(d.BestQuote.EstimatedDeliveryDays));

                await _context.SaveChangesAsync();

                _logger.LogInformation("Order {OrderId} processed with {Count} distributors", orderId, distributorOrders.Count);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing order {OrderId} with distributors", orderId);
                await UpdateOrderStatusAsync(orderId, OrderStatus.Failed);
                return false;
            }
        }

        public async Task<bool> DeleteOrderAsync(int orderId)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .Include(o => o.OrderDistributions)
                    .FirstOrDefaultAsync(o => o.Id == orderId);

                if (order == null)
                {
                    _logger.LogWarning("Order {OrderId} not found for deletion", orderId);
                    return false;
                }

                // Remove related entities first
                _context.OrderItems.RemoveRange(order.OrderItems);
                _context.OrderDistributions.RemoveRange(order.OrderDistributions);
                
                // Remove the order
                _context.Orders.Remove(order);
                
                await _context.SaveChangesAsync();

                _logger.LogInformation("Order {OrderId} deleted successfully", orderId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting order {OrderId}", orderId);
                return false;
            }
        }
    }
}