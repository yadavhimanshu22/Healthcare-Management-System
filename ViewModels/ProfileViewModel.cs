using System.ComponentModel.DataAnnotations;

namespace HealthcareManagementSystem.ViewModels
{
    public class ProfileViewModel
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100)]
        public string FullName { get; set; } = "";

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = "";

        [Phone]
        [StringLength(10)]
        public string? PhoneNumber { get; set; }

        public string Role { get; set; } = "User";

        public bool IsAdmin { get; set; }
    }
}