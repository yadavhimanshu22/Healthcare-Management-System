using System.ComponentModel.DataAnnotations;

namespace HealthcareManagementSystem.Models;

public class Department
{
    public int DepartmentId { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = "";

    [StringLength(500)]
    public string? Description { get; set; }

    public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
}
