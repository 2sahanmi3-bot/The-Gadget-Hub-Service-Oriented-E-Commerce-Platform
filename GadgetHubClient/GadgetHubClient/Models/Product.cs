using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GadgetHubClient.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        
        [Required]
        public string GlobalId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
    }
}
