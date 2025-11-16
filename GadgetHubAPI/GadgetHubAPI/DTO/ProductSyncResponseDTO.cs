namespace GadgetHubAPI.DTO
{
    public class ProductSyncResponseDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string DistributorUsed { get; set; } = string.Empty;
    }
}
