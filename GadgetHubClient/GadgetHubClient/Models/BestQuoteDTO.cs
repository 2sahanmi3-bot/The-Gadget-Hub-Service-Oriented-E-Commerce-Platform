namespace GadgetHubClient.Models
{
    public class BestQuoteDTO
    {
        public string GlobalId { get; set; } = string.Empty;
        public string ChosenDistributor { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int EstimatedDeliveryDays { get; set; }
        public List<QuotationResponseDTO> AllQuotes { get; set; } = new();
    }
}
