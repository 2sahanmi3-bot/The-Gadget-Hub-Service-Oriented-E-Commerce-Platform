using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectroComAPI.Models
{
    public class Quotation
    {
        [Key]
        public int QuotationId { get; set; }

        [Required]
        public string GlobalId { get; set; } = string.Empty;

        [Required]
        public string QuotedProduct { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }

        public DateTime IssuedAt { get; set; } = DateTime.Now;
    }
}