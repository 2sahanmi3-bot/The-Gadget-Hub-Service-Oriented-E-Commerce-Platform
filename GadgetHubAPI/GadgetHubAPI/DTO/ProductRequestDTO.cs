namespace GadgetHubAPI.DTO
{
    public class ProductRequestDTO
    {
        public string GlobalId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Inventory { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
    }
}
