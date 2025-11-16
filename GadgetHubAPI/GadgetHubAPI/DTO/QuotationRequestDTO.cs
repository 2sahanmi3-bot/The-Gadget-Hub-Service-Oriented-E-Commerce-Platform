using System.ComponentModel.DataAnnotations;

namespace GadgetHubAPI.DTO
{
    public class QuotationRequestDTO
    {
        [Required]
        public List<QuotationItemDTO> Items { get; set; } = new List<QuotationItemDTO>();
    }

    public class QuotationItemDTO
    {
        [Required]
        public string GlobalId { get; set; } = string.Empty;
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
    }
}
