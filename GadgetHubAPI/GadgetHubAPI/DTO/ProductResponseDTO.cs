namespace GadgetHubAPI.DTO
{
    public class ProductResponseDTO
    {
        public int ProductId { get; set; }
        public string GlobalId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Inventory { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
