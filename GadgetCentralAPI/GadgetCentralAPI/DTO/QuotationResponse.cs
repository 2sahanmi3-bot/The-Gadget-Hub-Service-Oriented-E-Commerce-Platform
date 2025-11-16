namespace GadgetCentralAPI.DTO
{
    public class QuotationResponse
    {
        public int ProductId { get; set; }
        public string GlobalId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string ProductDetails { get; set; } = string.Empty;
        public string Thumbnail { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Inventory { get; set; }
        public string Supplier { get; set; } = string.Empty;
        public string SupplierAddress { get; set; } = string.Empty;
        public DateTime IssuedAt { get; set; } = DateTime.Now;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}