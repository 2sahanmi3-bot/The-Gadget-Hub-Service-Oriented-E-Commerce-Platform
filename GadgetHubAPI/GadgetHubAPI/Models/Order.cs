using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GadgetHubAPI.Models
{
    public class Order
    {
        public int Id { get; set; }
        
        [Required]
        public string OrderNumber { get; set; } = string.Empty;
        
        [Required]
        public int CustomerId { get; set; }
        
        public Customer Customer { get; set; } = default!;
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }
        
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        
        public string PaymentMethod { get; set; } = "Cash on Delivery";
        
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        
        public DateTime? EstimatedDeliveryDate { get; set; }
        
        public DateTime? ActualDeliveryDate { get; set; }
        
        public string DeliveryAddress { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string CustomerName { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string CustomerEmail { get; set; } = string.Empty;
        
        [StringLength(20)]
        public string CustomerPhone { get; set; } = string.Empty;
        
        public string Notes { get; set; } = string.Empty;
        
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        
        public List<OrderDistribution> OrderDistributions { get; set; } = new List<OrderDistribution>();
    }

    public class OrderItem
    {
        public int Id { get; set; }
        
        public int OrderId { get; set; }
        
        public Order Order { get; set; } = default!;
        
        [Required]
        public string GlobalId { get; set; } = string.Empty;
        
        [Required]
        public string ProductName { get; set; } = string.Empty;
        
        public int Quantity { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice => Quantity * UnitPrice;
        
        [StringLength(500)]
        public string ProductDescription { get; set; } = string.Empty;
        
        public string ImageUrl { get; set; } = string.Empty;
    }

    public class OrderDistribution
    {
        public int Id { get; set; }
        
        public int OrderId { get; set; }
        
        public Order Order { get; set; } = default!;
        
        [Required]
        public string DistributorName { get; set; } = string.Empty; // TechWorld, ElectroCom, GadgetCentral
        
        [Required]
        public string DistributorOrderId { get; set; } = string.Empty;
        
        public OrderDistributionStatus Status { get; set; } = OrderDistributionStatus.Pending;
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal DistributorTotalAmount { get; set; }
        
        public DateTime? DistributorOrderDate { get; set; }
        
        public DateTime? ConfirmedAt { get; set; }
        
        public DateTime? ShippedAt { get; set; }
        
        public DateTime? DeliveredAt { get; set; }
        
        public string TrackingNumber { get; set; } = string.Empty;
        
        [StringLength(1000)]
        public string DistributorNotes { get; set; } = string.Empty;
        
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }

    public class ShoppingCart
    {
        [Key]
        public int CartId { get; set; }
        
        [Required]
        public int CustomerId { get; set; }
        
        [Required]
        public DateTime CreatedDate { get; set; }
        
        public DateTime? LastModifiedDate { get; set; }
        
        // Navigation property
        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }

    public class CartItem
    {
        [Key]
        public int CartItemId { get; set; }
        
        [Required]
        public int CartId { get; set; }
        
        [Required]
        [StringLength(50)]
        public string GlobalId { get; set; } = string.Empty;
        
        [Required]
        [StringLength(200)]
        public string ProductName { get; set; } = string.Empty;
        
        [Required]
        public int Quantity { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }
        
        [StringLength(500)]
        public string ProductDescription { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string ProductImageUrl { get; set; } = string.Empty;
        
        public DateTime AddedDate { get; set; }
        
        // Navigation property
        public virtual ShoppingCart ShoppingCart { get; set; } = null!;
    }

    public enum OrderStatus
    {
        Pending = 0,
        Processing = 1,
        Confirmed = 2,
        Shipped = 3,
        Delivered = 4,
        Cancelled = 5,
        Failed = 6
    }

    public enum OrderDistributionStatus
    {
        Pending = 0,
        Confirmed = 1,
        Shipped = 2,
        Delivered = 3,
        Failed = 4
    }
}
