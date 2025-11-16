using GadgetHubAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace GadgetHubAPI.Data
{
    public class OrderRepo
    {
        private readonly AppDbContext _context;

        public OrderRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .Include(o => o.OrderDistributions)
                .ThenInclude(od => od.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Order?> GetByOrderNumberAsync(string orderNumber)
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .Include(o => o.OrderDistributions)
                .ThenInclude(od => od.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);
        }

        public async Task<List<Order>> GetByCustomerIdAsync(int customerId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.OrderDistributions)
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<List<Order>> GetAllAsync()
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .Include(o => o.OrderDistributions)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<Order> CreateAsync(Order order)
        {
            // Generate unique order number
            order.OrderNumber = GenerateOrderNumber();
            
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<bool> UpdateAsync(Order order)
        {
            try
            {
                _context.Orders.Update(order);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateStatusAsync(int orderId, OrderStatus status)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return false;

            order.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateDistributionStatusAsync(int distributionId, OrderDistributionStatus status)
        {
            var distribution = await _context.OrderDistributions.FindAsync(distributionId);
            if (distribution == null) return false;

            distribution.Status = status;
            
            // Update timestamps based on status
            switch (status)
            {
                case OrderDistributionStatus.Confirmed:
                    distribution.ConfirmedAt = DateTime.UtcNow;
                    break;
                case OrderDistributionStatus.Shipped:
                    distribution.ShippedAt = DateTime.UtcNow;
                    break;
                case OrderDistributionStatus.Delivered:
                    distribution.DeliveredAt = DateTime.UtcNow;
                    break;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        private string GenerateOrderNumber()
        {
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var random = new Random().Next(1000, 9999);
            return $"GH{timestamp}{random}";
        }
    }
}
