namespace GadgetHubAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = "";  
        public string PasswordHash { get; set; } = "";
        public string Role { get; set; } = "User";   // "User" | "Admin"
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
