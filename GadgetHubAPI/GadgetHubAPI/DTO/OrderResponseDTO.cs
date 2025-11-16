using System.ComponentModel.DataAnnotations;
using GadgetHubAPI.Models;

namespace GadgetHubAPI.DTO
{
    public class CreateOrderDTO
    {
        [Required]
        public int CustomerId { get; set; }
        
        [Required]
        [StringLength(500)]
        public string DeliveryAddress { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string CustomerName { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string CustomerEmail { get; set; } = string.Empty;
        
        [Required]
        [StringLength(20)]
        public string CustomerPhone { get; set; } = string.Empty;
        
        [StringLength(1000)]
        public string Notes { get; set; } = string.Empty;
        
        [Required]
        public List<OrderItemDTO> Items { get; set; } = new List<OrderItemDTO>();
    }

    public class OrderItemDTO
    {
        [Required]
        [StringLength(50)]
        public string GlobalId { get; set; } = string.Empty;
        
        [Required]
        [StringLength(200)]
        public string ProductName { get; set; } = string.Empty;
        
        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal UnitPrice { get; set; }
        
        [StringLength(500)]
        public string ProductDescription { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string ProductImageUrl { get; set; } = string.Empty;
    }

    public class OrderResponseDTO
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public string DeliveryAddress { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public DateTime? EstimatedDeliveryDate { get; set; }
        public string Notes { get; set; } = string.Empty;
        public List<OrderItemResponseDTO> Items { get; set; } = new List<OrderItemResponseDTO>();
        public List<OrderDistributionResponseDTO> Distributions { get; set; } = new List<OrderDistributionResponseDTO>();
    }

    public class OrderItemResponseDTO
    {
        public int OrderItemId { get; set; }
        public string GlobalId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string ProductDescription { get; set; } = string.Empty;
        public string ProductImageUrl { get; set; } = string.Empty;
    }

    public class OrderDistributionResponseDTO
    {
        public int OrderDistributionId { get; set; }
        public string DistributorName { get; set; } = string.Empty;
        public string DistributorOrderId { get; set; } = string.Empty;
        public decimal DistributorTotalAmount { get; set; }
        public DateTime? DistributorOrderDate { get; set; }
        public DateTime? EstimatedDeliveryDate { get; set; }
        public string DistributorNotes { get; set; } = string.Empty;
        public OrderDistributionStatus Status { get; set; }
    }

    public class CartItemDTO
    {
        [Required]
        [StringLength(50)]
        public string GlobalId { get; set; } = string.Empty;
        
        [Required]
        [StringLength(200)]
        public string ProductName { get; set; } = string.Empty;
        
        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal UnitPrice { get; set; }
        
        [StringLength(500)]
        public string ProductDescription { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string ProductImageUrl { get; set; } = string.Empty;
    }

    public class CartResponseDTO
    {
        public int CartId { get; set; }
        public int CustomerId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public decimal TotalAmount { get; set; }
        public int TotalItems { get; set; }
        public List<CartItemResponseDTO> Items { get; set; } = new List<CartItemResponseDTO>();
    }

    public class CartItemResponseDTO
    {
        public int CartItemId { get; set; }
        public string GlobalId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string ProductDescription { get; set; } = string.Empty;
        public string ProductImageUrl { get; set; } = string.Empty;
        public DateTime AddedDate { get; set; }
    }

    public class AddToCartDTO
    {
        [Required]
        public int CustomerId { get; set; }
        
        [Required]
        public CartItemDTO Item { get; set; } = new CartItemDTO();
    }

    public class UpdateCartItemDTO
    {
        [Required]
        public int CartItemId { get; set; }
        
        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }

    public class RemoveFromCartDTO
    {
        [Required]
        public int CartItemId { get; set; }
    }
}
