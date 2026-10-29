using HealthcareManagementSystem.Data;
using HealthcareManagementSystem.Models;
using HealthcareManagementSystem.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// =====================================================
// DATABASE CONNECTION
// =====================================================

var connectionString =
    builder.Configuration.GetConnectionString("HealthcareDb")
    ?? throw new InvalidOperationException(
        "HealthcareDb connection string is missing."
    );

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)
    );
});


// =====================================================
// IDENTITY
// =====================================================

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        // Password settings
        options.Password.RequiredLength = 6;
        options.Password.RequireDigit = true;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;

        // User settings
        options.User.RequireUniqueEmail = true;

        // Email confirmation not required
        options.SignIn.RequireConfirmedAccount = false;
        options.SignIn.RequireConfirmedEmail = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


// =====================================================
// COOKIE SETTINGS
// =====================================================

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";

    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
});


// =====================================================
// MVC
// =====================================================

builder.Services.AddControllersWithViews();


// =====================================================
// APPLICATION SERVICES
// =====================================================

builder.Services.AddScoped<IHealthcareService, HealthcareService>();
builder.Services.AddHttpContextAccessor();


// =====================================================
// BUILD APPLICATION
// =====================================================

var app = builder.Build();


// =====================================================
// DATABASE SEEDING + SINGLE ADMIN ACCOUNT
// =====================================================

using (var scope = app.Services.CreateScope())
{
    try
    {
        // =================================================
        // EXISTING DATABASE SEEDER
        // =================================================

        await DbSeeder.SeedAsync(scope.ServiceProvider);


        // =================================================
        // IDENTITY SERVICES
        // =================================================

        var roleManager =
            scope.ServiceProvider
                .GetRequiredService<RoleManager<IdentityRole>>();

        var userManager =
            scope.ServiceProvider
                .GetRequiredService<UserManager<ApplicationUser>>();


        // =================================================
        // ADMIN DETAILS
        // ONLY THIS USER WILL BE ADMIN
        // =================================================

        const string adminRole = "Admin";
        const string adminEmail = "Himanshu@2522.com";
        const string adminPassword = "Himanshu@2522";


        // =================================================
        // CREATE ADMIN ROLE IF NOT EXISTS
        // =================================================

        if (!await roleManager.RoleExistsAsync(adminRole))
        {
            var roleResult =
                await roleManager.CreateAsync(
                    new IdentityRole(adminRole)
                );

            if (!roleResult.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    roleResult.Errors.Select(e => e.Description)
                );

                throw new Exception(
                    "Failed to create Admin role: " + errors
                );
            }
        }


        // =================================================
        // FIND FIXED ADMIN USER
        // =================================================

        var adminUser =
            await userManager.FindByEmailAsync(adminEmail);


        // =================================================
        // CREATE ADMIN USER IF NOT EXISTS
        // =================================================

        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                FullName = "Himanshu Yadav"
            };

            var createResult =
                await userManager.CreateAsync(
                    adminUser,
                    adminPassword
                );

            if (!createResult.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    createResult.Errors.Select(e => e.Description)
                );

                throw new Exception(
                    "Failed to create admin user: " + errors
                );
            }
        }


        // =================================================
        // MAKE SURE HIMANSHU IS ADMIN
        // =================================================

        if (!await userManager.IsInRoleAsync(
                adminUser,
                adminRole))
        {
            var addRoleResult =
                await userManager.AddToRoleAsync(
                    adminUser,
                    adminRole
                );

            if (!addRoleResult.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    addRoleResult.Errors.Select(e => e.Description)
                );

                throw new Exception(
                    "Failed to assign Admin role: " + errors
                );
            }
        }


        // =================================================
        // REMOVE ADMIN ROLE FROM ALL OTHER USERS
        // =================================================

        var allUsers =
            await userManager.Users.ToListAsync();

        foreach (var user in allUsers)
        {
            // Skip the fixed Admin user
            if (user.Id == adminUser.Id)
            {
                continue;
            }

            // Check if this user is Admin
            if (await userManager.IsInRoleAsync(
                    user,
                    adminRole))
            {
                // Remove Admin role
                var removeRoleResult =
                    await userManager.RemoveFromRoleAsync(
                        user,
                        adminRole
                    );

                if (!removeRoleResult.Succeeded)
                {
                    var errors = string.Join(
                        ", ",
                        removeRoleResult.Errors.Select(
                            e => e.Description
                        )
                    );

                    throw new Exception(
                        $"Failed to remove Admin role from {user.Email}: {errors}"
                    );
                }
            }
        }


        // =================================================
        // FINAL ADMIN CHECK
        // =================================================

        Console.WriteLine(
            "================================================="
        );

        Console.WriteLine(
            "ADMIN ACCOUNT CONFIGURED"
        );

        Console.WriteLine(
            $"Admin Email: {adminEmail}"
        );

        Console.WriteLine(
            "Admin Role: Admin"
        );

        Console.WriteLine(
            "All other Admin roles have been removed."
        );

        Console.WriteLine(
            "================================================="
        );
    }
    catch (Exception ex)
    {
        var logger =
            scope.ServiceProvider
                .GetRequiredService<ILogger<Program>>();

        logger.LogError(
            ex,
            "An error occurred while seeding the database or configuring the Admin account."
        );
    }
}


// =====================================================
// HTTP PIPELINE
// =====================================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();


// =====================================================
// AUTHENTICATION & AUTHORIZATION
// =====================================================

app.UseAuthentication();

app.UseAuthorization();


// =====================================================
// MVC ROUTING
// =====================================================

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);


// =====================================================
// RUN APPLICATION
// =====================================================

app.Run();