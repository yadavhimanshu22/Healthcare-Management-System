using System.ComponentModel.DataAnnotations;

namespace HealthcareManagementSystem.Models;

public class Receptionist
{
    public int ReceptionistId { get; set; }
    [Required] public string UserId { get; set; } = "";
    [Required, StringLength(150)] public string FullName { get; set; } = "";
    [Required, EmailAddress] public string Email { get; set; } = "";
    [Phone] public string? Phone { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
