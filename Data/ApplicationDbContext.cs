using HealthcareManagementSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HealthcareManagementSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // =====================================================
        // DATABASE TABLES
        // =====================================================

        public DbSet<Patient> Patients => Set<Patient>();

        public DbSet<Doctor> Doctors => Set<Doctor>();

        public DbSet<Receptionist> Receptionists => Set<Receptionist>();

        public DbSet<Department> Departments => Set<Department>();

        public DbSet<Appointment> Appointments => Set<Appointment>();

        public DbSet<MedicalRecord> MedicalRecords => Set<MedicalRecord>();

        public DbSet<Prescription> Prescriptions => Set<Prescription>();

        public DbSet<PrescriptionItem> PrescriptionItems => Set<PrescriptionItem>();

        public DbSet<LabReport> LabReports => Set<LabReport>();

        public DbSet<Bill> Bills => Set<Bill>();

        public DbSet<Notification> Notifications => Set<Notification>();


        // =====================================================
        // DATABASE RELATIONSHIPS & CONFIGURATION
        // =====================================================

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // IMPORTANT:
            // This creates all ASP.NET Core Identity tables
            base.OnModelCreating(builder);


            // =================================================
            // APPOINTMENT
            // =================================================

            // Same doctor cannot have duplicate appointment
            // at the same date and time.
            builder.Entity<Appointment>()
                .HasIndex(a => new
                {
                    a.DoctorId,
                    a.AppointmentDate,
                    a.AppointmentTime
                })
                .IsUnique();


            // Appointment -> Patient
            builder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);


            // Appointment -> Doctor
            builder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);


            // =================================================
            // MEDICAL RECORD
            // =================================================

            builder.Entity<MedicalRecord>()
                .HasOne(x => x.Patient)
                .WithMany(x => x.MedicalRecords)
                .HasForeignKey(x => x.PatientId)
                .OnDelete(DeleteBehavior.Restrict);


            // =================================================
            // PRESCRIPTION
            // =================================================

            builder.Entity<Prescription>()
                .HasOne(x => x.Patient)
                .WithMany(x => x.Prescriptions)
                .HasForeignKey(x => x.PatientId)
                .OnDelete(DeleteBehavior.Restrict);


            // =================================================
            // LAB REPORT
            // =================================================

            builder.Entity<LabReport>()
                .HasOne(x => x.Patient)
                .WithMany(x => x.LabReports)
                .HasForeignKey(x => x.PatientId)
                .OnDelete(DeleteBehavior.Restrict);


            // =================================================
            // BILL
            // =================================================

            builder.Entity<Bill>()
                .HasOne(x => x.Patient)
                .WithMany(x => x.Bills)
                .HasForeignKey(x => x.PatientId)
                .OnDelete(DeleteBehavior.Restrict);


            // =================================================
            // NOTIFICATION
            // =================================================

            builder.Entity<Notification>()
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            // =================================================
            // DECIMAL PRECISION
            // =================================================

            builder.Entity<Doctor>()
                .Property(x => x.ConsultationFee)
                .HasPrecision(10, 2);


            builder.Entity<Bill>()
                .Property(x => x.ConsultationFee)
                .HasPrecision(10, 2);

            builder.Entity<Bill>()
                .Property(x => x.LabCharges)
                .HasPrecision(10, 2);

            builder.Entity<Bill>()
                .Property(x => x.MedicineCharges)
                .HasPrecision(10, 2);

            builder.Entity<Bill>()
                .Property(x => x.OtherCharges)
                .HasPrecision(10, 2);

            builder.Entity<Bill>()
                .Property(x => x.Discount)
                .HasPrecision(10, 2);

            builder.Entity<Bill>()
                .Property(x => x.Tax)
                .HasPrecision(10, 2);

            builder.Entity<Bill>()
                .Property(x => x.TotalAmount)
                .HasPrecision(10, 2);
        }
    }
}