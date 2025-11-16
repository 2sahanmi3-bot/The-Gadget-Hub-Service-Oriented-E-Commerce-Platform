using System.ComponentModel.DataAnnotations;

namespace GadgetHubAPI.Models
{
    public class Customer
    {
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string PhoneNumber { get; set; } = string.Empty;
        
        public string Address { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Foreign key to User table
        public int UserId { get; set; }
        
        // Navigation properties
        public User User { get; set; } = null!;
        public List<Order> Orders { get; set; } = new List<Order>();
    }
}
