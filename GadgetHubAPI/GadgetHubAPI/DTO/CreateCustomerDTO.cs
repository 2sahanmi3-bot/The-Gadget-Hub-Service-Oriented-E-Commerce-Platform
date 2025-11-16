using System.ComponentModel.DataAnnotations;

namespace GadgetHubAPI.DTO
{
    public class CreateCustomerDTO
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;
        
        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string Address { get; set; } = string.Empty;
        
        [Required]
        public int UserId { get; set; }
    }
}


