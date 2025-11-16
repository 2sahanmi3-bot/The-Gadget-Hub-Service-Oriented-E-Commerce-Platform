namespace GadgetHubClient.Models
{
    public class QuotationRequestDTO
    {
        public List<string> GlobalIds { get; set; } = new();
        public List<int> Quantities { get; set; } = new();
    }
}
