using System.ComponentModel.DataAnnotations.Schema;

namespace HealthcareManagementSystem.Models;

public class Bill
{
    public int BillId { get; set; }
    public int PatientId { get; set; }
    public int? AppointmentId { get; set; }
    [Column(TypeName="decimal(10,2)")] public decimal ConsultationFee { get; set; }
    [Column(TypeName="decimal(10,2)")] public decimal LabCharges { get; set; }
    [Column(TypeName="decimal(10,2)")] public decimal MedicineCharges { get; set; }
    [Column(TypeName="decimal(10,2)")] public decimal OtherCharges { get; set; }
    [Column(TypeName="decimal(10,2)")] public decimal Discount { get; set; }
    [Column(TypeName="decimal(10,2)")] public decimal Tax { get; set; }
    [Column(TypeName="decimal(10,2)")] public decimal TotalAmount { get; set; }
    public string PaymentStatus { get; set; } = "Pending";
    public DateTime? PaymentDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Patient? Patient { get; set; }
    public Appointment? Appointment { get; set; }
}
