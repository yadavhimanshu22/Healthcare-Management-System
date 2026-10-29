# Healthcare Management System

A B.Tech-friendly Healthcare Management System built with ASP.NET Core MVC, C#, Entity Framework Core, MySQL, Razor Views and Bootstrap 5.

## Requirements
- Visual Studio 2022
- .NET 8 SDK
- MySQL 8.x
- EF Core CLI

## Setup

1. Open `HealthcareManagementSystem.csproj` in Visual Studio 2022.
2. Install/start MySQL.
3. Open `appsettings.json`.
4. Replace `YOUR_MYSQL_PASSWORD` with your local MySQL password.
5. Open Package Manager Console or terminal.
6. Run:

```bash
dotnet restore
dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet run
```

The application also runs migrations and demo seed data on startup.

## Demo accounts

Admin: admin@healthcare.com / Admin@123  
Doctor: doctor@healthcare.com / Doctor@123  
Patient: patient@healthcare.com / Patient@123  
Receptionist: receptionist@healthcare.com / Receptionist@123

These credentials are for local development only.

## Main features
- ASP.NET Core Identity authentication
- Four roles
- Patient/doctor/receptionist/admin dashboards
- Appointment booking
- Doctor double-booking prevention
- Medical records
- Prescriptions
- Lab reports model
- Billing model
- Notifications model
- MySQL database
- EF Core migrations
- Responsive Bootstrap UI

## Important
This is an educational/project starter and should not be deployed for real clinical use without a security, privacy, compliance, auditing, backup and infrastructure review.
