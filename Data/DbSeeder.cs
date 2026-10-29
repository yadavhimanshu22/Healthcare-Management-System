using HealthcareManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HealthcareManagementSystem.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        await context.Database.MigrateAsync();

        foreach (var role in new[] { "Admin", "Doctor", "Patient", "Receptionist" })
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        var admin = await CreateUser(userManager, "admin@healthcare.com", "Admin User", "Admin@123");
        if (!await userManager.IsInRoleAsync(admin, "Admin"))
            await userManager.AddToRoleAsync(admin, "Admin");

        if (!await context.Departments.AnyAsync())
        {
            context.Departments.AddRange(
                new Department { Name = "General Medicine", Description = "General healthcare services" },
                new Department { Name = "Cardiology", Description = "Heart and cardiovascular care" },
                new Department { Name = "Pediatrics", Description = "Healthcare for children" }
            );
            await context.SaveChangesAsync();
        }

        var doctorUser = await CreateUser(userManager, "doctor@healthcare.com", "Dr. Demo", "Doctor@123");
        if (!await userManager.IsInRoleAsync(doctorUser, "Doctor"))
            await userManager.AddToRoleAsync(doctorUser, "Doctor");

        if (!await context.Doctors.AnyAsync(d => d.UserId == doctorUser.Id))
        {
            var dept = await context.Departments.FirstAsync();
            context.Doctors.Add(new Doctor
            {
                UserId = doctorUser.Id, FullName = "Dr. Demo", Email = doctorUser.Email!,
                Specialization = "General Physician", Qualification = "MBBS",
                Experience = 5, LicenseNumber = "DEMO-001", ConsultationFee = 500,
                AvailableDays = "Mon,Tue,Wed,Thu,Fri", AvailableTime = "10:00-17:00",
                DepartmentId = dept.DepartmentId
            });
            await context.SaveChangesAsync();
        }

        var patientUser = await CreateUser(userManager, "patient@healthcare.com", "Demo Patient", "Patient@123");
        if (!await userManager.IsInRoleAsync(patientUser, "Patient"))
            await userManager.AddToRoleAsync(patientUser, "Patient");

        if (!await context.Patients.AnyAsync(p => p.UserId == patientUser.Id))
        {
            context.Patients.Add(new Patient
            {
                UserId = patientUser.Id, FullName = "Demo Patient",
                Email = patientUser.Email!, Phone = "9999999999", Gender = "Other",
                BloodGroup = "O+"
            });
            await context.SaveChangesAsync();
        }

        var receptionistUser = await CreateUser(userManager, "receptionist@healthcare.com", "Demo Receptionist", "Receptionist@123");
        if (!await userManager.IsInRoleAsync(receptionistUser, "Receptionist"))
            await userManager.AddToRoleAsync(receptionistUser, "Receptionist");

        if (!await context.Receptionists.AnyAsync(r => r.UserId == receptionistUser.Id))
        {
            context.Receptionists.Add(new Receptionist
            {
                UserId = receptionistUser.Id, FullName = "Demo Receptionist",
                Email = receptionistUser.Email!, Phone = "8888888888"
            });
            await context.SaveChangesAsync();
        }
    }

    private static async Task<ApplicationUser> CreateUser(
        UserManager<ApplicationUser> manager, string email, string name, string password)
    {
        var user = await manager.FindByEmailAsync(email);
        if (user != null) return user;

        user = new ApplicationUser { UserName = email, Email = email, FullName = name };
        var result = await manager.CreateAsync(user, password);
        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join("; ", result.Errors.Select(e => e.Description)));
        return user;
    }
}
