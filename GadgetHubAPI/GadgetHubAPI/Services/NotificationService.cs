using GadgetHubAPI.Models;
using Microsoft.Extensions.Logging;

namespace GadgetHubAPI.Services
{
    public interface INotificationService
    {
        Task NotifyOrderConfirmedAsync(Order order);
        Task NotifyOrderShippedAsync(Order order, OrderDistribution distribution);
        Task NotifyOrderDeliveredAsync(Order order, OrderDistribution distribution);
        Task NotifyOrderStatusChangedAsync(Order order, string newStatus);
    }

    public class NotificationService : INotificationService
    {
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(ILogger<NotificationService> logger)
        {
            _logger = logger;
        }

        public async Task NotifyOrderConfirmedAsync(Order order)
        {
            try
            {
                _logger.LogInformation("Sending order confirmation notification for order {OrderNumber}", order.OrderNumber);
                
                // In a real system, you would:
                // 1. Send email to customer
                // 2. Send SMS notification
                // 3. Push notification to mobile app
                // 4. Update customer dashboard
                
                var message = $"Order {order.OrderNumber} has been confirmed! Total: Rs. {order.TotalAmount:N2}. " +
                             $"Estimated delivery: {order.EstimatedDeliveryDate?.ToString("MMM dd, yyyy") ?? "TBD"}";
                
                _logger.LogInformation("Order confirmation notification: {Message}", message);
                
                // Simulate async operation
                await Task.Delay(100);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending order confirmation notification for order {OrderNumber}", order.OrderNumber);
            }
        }

        public async Task NotifyOrderShippedAsync(Order order, OrderDistribution distribution)
        {
            try
            {
                _logger.LogInformation("Sending order shipped notification for order {OrderNumber}, distribution {DistributorOrderId}", 
                    order.OrderNumber, distribution.DistributorOrderId);
                
                var message = $"Your order {order.OrderNumber} has been shipped by {distribution.DistributorName}! " +
                             $"Tracking number: {distribution.TrackingNumber}";
                
                _logger.LogInformation("Order shipped notification: {Message}", message);
                
                await Task.Delay(100);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending order shipped notification for order {OrderNumber}", order.OrderNumber);
            }
        }

        public async Task NotifyOrderDeliveredAsync(Order order, OrderDistribution distribution)
        {
            try
            {
                _logger.LogInformation("Sending order delivered notification for order {OrderNumber}, distribution {DistributorOrderId}", 
                    order.OrderNumber, distribution.DistributorOrderId);
                
                var message = $"Your order {order.OrderNumber} has been delivered by {distribution.DistributorName}! " +
                             $"Thank you for shopping with The Gadget Hub.";
                
                _logger.LogInformation("Order delivered notification: {Message}", message);
                
                await Task.Delay(100);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending order delivered notification for order {OrderNumber}", order.OrderNumber);
            }
        }

        public async Task NotifyOrderStatusChangedAsync(Order order, string newStatus)
        {
            try
            {
                _logger.LogInformation("Sending order status change notification for order {OrderNumber}, new status: {Status}", 
                    order.OrderNumber, newStatus);
                
                var message = $"Your order {order.OrderNumber} status has been updated to: {newStatus}";
                
                _logger.LogInformation("Order status change notification: {Message}", message);
                
                await Task.Delay(100);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending order status change notification for order {OrderNumber}", order.OrderNumber);
            }
        }
    }
}
