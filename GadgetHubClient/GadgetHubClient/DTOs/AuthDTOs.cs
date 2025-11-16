using System.ComponentModel.DataAnnotations;

namespace GadgetHubClient.DTOs
{
    public class LoginRequestDTO
    {
        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(255, ErrorMessage = "Password cannot exceed 255 characters")]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; } = false;
    }

    public class LoginResponseDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public int UserId { get; set; }
        public int? CustomerId { get; set; }
        public string RedirectUrl { get; set; } = string.Empty;
    }

    public class RegisterRequestDTO
    {
        // User credentials
        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters")]
        [MinLength(3, ErrorMessage = "Username must be at least 3 characters")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(255, ErrorMessage = "Password cannot exceed 255 characters")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm Password is required")]
        [Compare("Password", ErrorMessage = "Password and Confirm Password do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;

        // Customer details
        [Required(ErrorMessage = "Full Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone Number is required")]
        [StringLength(15, ErrorMessage = "Phone Number cannot exceed 15 characters")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required")]
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "You must agree to the terms and conditions")]
        public bool AgreeToTerms { get; set; } = false;

        public string PreferredPaymentMethod { get; set; } = "Cash on Delivery";
    }

    public class RegisterResponseDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int UserId { get; set; }
        public int CustomerId { get; set; }
        public string RedirectUrl { get; set; } = string.Empty;
    }
}
