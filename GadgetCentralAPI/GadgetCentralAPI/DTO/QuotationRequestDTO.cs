namespace TechWorldAPI.DTO
{
    public class QuotationRequestDTO
    {
        public List<string> GlobalId { get; set; } = new();
        public List<int> Quantities { get; set; } = new();
    }
}
