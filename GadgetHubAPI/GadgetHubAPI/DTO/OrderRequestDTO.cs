using System.ComponentModel.DataAnnotations;

namespace GadgetHubAPI.DTO
{
    public class OrderRequestDTO
    {
        [Required]
        public int CustomerId { get; set; }
        
        [Required]
        public List<OrderItemRequestDTO> OrderItems { get; set; } = new List<OrderItemRequestDTO>();
        
        public string PaymentMethod { get; set; } = "Cash on Delivery";
        
        [Required]
        public string DeliveryAddress { get; set; } = string.Empty;
        
        public string Notes { get; set; } = string.Empty;
    }

    public class OrderItemRequestDTO
    {
        [Required]
        public string GlobalId { get; set; } = string.Empty;
        
        [Required]
        public string ProductName { get; set; } = string.Empty;
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than 0")]
        public decimal UnitPrice { get; set; }
        
        public string ImageUrl { get; set; } = string.Empty;
    }
}
