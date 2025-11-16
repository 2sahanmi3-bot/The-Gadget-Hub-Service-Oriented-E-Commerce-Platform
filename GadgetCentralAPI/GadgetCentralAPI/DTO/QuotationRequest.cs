namespace GadgetCentralAPI.DTO
{
    public class QuotationRequest
    {
        public string GlobalId { get; set; } = string.Empty;
        public int RequestedQuantity { get; set; }
    }
}