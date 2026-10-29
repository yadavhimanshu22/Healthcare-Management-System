namespace HealthcareManagementSystem.ViewModels
{
    public class AdminUserViewModel
    {
        public string Id { get; set; } = "";

        public string FullName { get; set; } = "";

        public string Email { get; set; } = "";

        public string PhoneNumber { get; set; } = "";

        public string Role { get; set; } = "User";

        public bool IsActive { get; set; }
    }
}