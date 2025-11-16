namespace TechWorldAPI.DTO
{
    public class QuotationResponseDTO
    {
        public string GlobalId { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int AvailableQuantity { get; set; }
        public int EstimatedDeliveryDays { get; set; }
    }
}
