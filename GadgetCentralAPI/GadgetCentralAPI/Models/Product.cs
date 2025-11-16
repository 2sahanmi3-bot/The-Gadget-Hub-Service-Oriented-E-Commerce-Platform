using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GadgetCentralAPI.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        public string GlobalId { get; set; } = string.Empty;

        [Required]
        public string ItemName { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        public int Inventory { get; set; }

        [Required]
        public string ProductDetails { get; set; } = string.Empty;

        [Required]
        public string ProductCategory { get; set; } = string.Empty;

        public string Thumbnail { get; set; } = string.Empty;
    }
}