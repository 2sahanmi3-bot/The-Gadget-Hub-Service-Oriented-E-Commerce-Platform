namespace GadgetHubAPI.DTO
{
    public class QuotationResponseDTO
    {
        public string Distributor { get; set; } = string.Empty;
        public string GlobalId { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public decimal Price { get; set; }
        public int AvailableQuantity { get; set; }
        public int EstimatedDeliveryDays { get; set; }
        
        // Additional properties needed by OrderService
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Thumbnail { get; set; } = string.Empty;
    }
}
