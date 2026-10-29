CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

START TRANSACTION;

ALTER DATABASE CHARACTER SET utf8mb4;

CREATE TABLE `AspNetRoles` (
    `Id` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `Name` varchar(256) CHARACTER SET utf8mb4 NULL,
    `NormalizedName` varchar(256) CHARACTER SET utf8mb4 NULL,
    `ConcurrencyStamp` longtext CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_AspNetRoles` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `AspNetUsers` (
    `Id` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `FullName` longtext CHARACTER SET utf8mb4 NOT NULL,
    `CreatedAt` datetime(6) NOT NULL,
    `UserName` varchar(256) CHARACTER SET utf8mb4 NULL,
    `NormalizedUserName` varchar(256) CHARACTER SET utf8mb4 NULL,
    `Email` varchar(256) CHARACTER SET utf8mb4 NULL,
    `NormalizedEmail` varchar(256) CHARACTER SET utf8mb4 NULL,
    `EmailConfirmed` tinyint(1) NOT NULL,
    `PasswordHash` longtext CHARACTER SET utf8mb4 NULL,
    `SecurityStamp` longtext CHARACTER SET utf8mb4 NULL,
    `ConcurrencyStamp` longtext CHARACTER SET utf8mb4 NULL,
    `PhoneNumber` longtext CHARACTER SET utf8mb4 NULL,
    `PhoneNumberConfirmed` tinyint(1) NOT NULL,
    `TwoFactorEnabled` tinyint(1) NOT NULL,
    `LockoutEnd` datetime(6) NULL,
    `LockoutEnabled` tinyint(1) NOT NULL,
    `AccessFailedCount` int NOT NULL,
    CONSTRAINT `PK_AspNetUsers` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `Departments` (
    `DepartmentId` int NOT NULL AUTO_INCREMENT,
    `Name` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
    `Description` varchar(500) CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_Departments` PRIMARY KEY (`DepartmentId`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `Patients` (
    `PatientId` int NOT NULL AUTO_INCREMENT,
    `UserId` longtext CHARACTER SET utf8mb4 NOT NULL,
    `FullName` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `Email` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Phone` longtext CHARACTER SET utf8mb4 NULL,
    `DateOfBirth` datetime(6) NULL,
    `Gender` longtext CHARACTER SET utf8mb4 NULL,
    `Address` longtext CHARACTER SET utf8mb4 NULL,
    `BloodGroup` longtext CHARACTER SET utf8mb4 NULL,
    `EmergencyContact` longtext CHARACTER SET utf8mb4 NULL,
    `CreatedAt` datetime(6) NOT NULL,
    CONSTRAINT `PK_Patients` PRIMARY KEY (`PatientId`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `Receptionists` (
    `ReceptionistId` int NOT NULL AUTO_INCREMENT,
    `UserId` longtext CHARACTER SET utf8mb4 NOT NULL,
    `FullName` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `Email` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Phone` longtext CHARACTER SET utf8mb4 NULL,
    `CreatedAt` datetime(6) NOT NULL,
    CONSTRAINT `PK_Receptionists` PRIMARY KEY (`ReceptionistId`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `AspNetRoleClaims` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `RoleId` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `ClaimType` longtext CHARACTER SET utf8mb4 NULL,
    `ClaimValue` longtext CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_AspNetRoleClaims` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_AspNetRoleClaims_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `AspNetRoles` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE TABLE `AspNetUserClaims` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `UserId` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `ClaimType` longtext CHARACTER SET utf8mb4 NULL,
    `ClaimValue` longtext CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_AspNetUserClaims` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_AspNetUserClaims_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE TABLE `AspNetUserLogins` (
    `LoginProvider` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `ProviderKey` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `ProviderDisplayName` longtext CHARACTER SET utf8mb4 NULL,
    `UserId` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_AspNetUserLogins` PRIMARY KEY (`LoginProvider`, `ProviderKey`),
    CONSTRAINT `FK_AspNetUserLogins_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE TABLE `AspNetUserRoles` (
    `UserId` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `RoleId` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_AspNetUserRoles` PRIMARY KEY (`UserId`, `RoleId`),
    CONSTRAINT `FK_AspNetUserRoles_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `AspNetRoles` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_AspNetUserRoles_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE TABLE `AspNetUserTokens` (
    `UserId` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `LoginProvider` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `Name` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `Value` longtext CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_AspNetUserTokens` PRIMARY KEY (`UserId`, `LoginProvider`, `Name`),
    CONSTRAINT `FK_AspNetUserTokens_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE TABLE `Notifications` (
    `NotificationId` int NOT NULL AUTO_INCREMENT,
    `UserId` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `Message` longtext CHARACTER SET utf8mb4 NOT NULL,
    `IsRead` tinyint(1) NOT NULL,
    `CreatedAt` datetime(6) NOT NULL,
    CONSTRAINT `PK_Notifications` PRIMARY KEY (`NotificationId`),
    CONSTRAINT `FK_Notifications_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE TABLE `Doctors` (
    `DoctorId` int NOT NULL AUTO_INCREMENT,
    `UserId` longtext CHARACTER SET utf8mb4 NOT NULL,
    `FullName` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `Email` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Phone` longtext CHARACTER SET utf8mb4 NULL,
    `Specialization` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
    `Qualification` varchar(150) CHARACTER SET utf8mb4 NULL,
    `Experience` int NOT NULL,
    `LicenseNumber` varchar(100) CHARACTER SET utf8mb4 NULL,
    `ConsultationFee` decimal(10,2) NOT NULL,
    `ProfileImage` longtext CHARACTER SET utf8mb4 NULL,
    `AvailableDays` longtext CHARACTER SET utf8mb4 NULL,
    `AvailableTime` longtext CHARACTER SET utf8mb4 NULL,
    `DepartmentId` int NULL,
    `CreatedAt` datetime(6) NOT NULL,
    `IsActive` tinyint(1) NOT NULL,
    CONSTRAINT `PK_Doctors` PRIMARY KEY (`DoctorId`),
    CONSTRAINT `FK_Doctors_Departments_DepartmentId` FOREIGN KEY (`DepartmentId`) REFERENCES `Departments` (`DepartmentId`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `Appointments` (
    `AppointmentId` int NOT NULL AUTO_INCREMENT,
    `PatientId` int NOT NULL,
    `DoctorId` int NOT NULL,
    `AppointmentDate` datetime(6) NOT NULL,
    `AppointmentTime` time(6) NOT NULL,
    `Reason` varchar(500) CHARACTER SET utf8mb4 NULL,
    `Status` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Notes` varchar(1000) CHARACTER SET utf8mb4 NULL,
    `CreatedAt` datetime(6) NOT NULL,
    CONSTRAINT `PK_Appointments` PRIMARY KEY (`AppointmentId`),
    CONSTRAINT `FK_Appointments_Doctors_DoctorId` FOREIGN KEY (`DoctorId`) REFERENCES `Doctors` (`DoctorId`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Appointments_Patients_PatientId` FOREIGN KEY (`PatientId`) REFERENCES `Patients` (`PatientId`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;

CREATE TABLE `LabReports` (
    `LabReportId` int NOT NULL AUTO_INCREMENT,
    `PatientId` int NOT NULL,
    `DoctorId` int NULL,
    `TestName` longtext CHARACTER SET utf8mb4 NOT NULL,
    `TestDate` datetime(6) NOT NULL,
    `Result` longtext CHARACTER SET utf8mb4 NULL,
    `ReferenceRange` longtext CHARACTER SET utf8mb4 NULL,
    `Remarks` longtext CHARACTER SET utf8mb4 NULL,
    `ReportFile` longtext CHARACTER SET utf8mb4 NULL,
    `CreatedAt` datetime(6) NOT NULL,
    CONSTRAINT `PK_LabReports` PRIMARY KEY (`LabReportId`),
    CONSTRAINT `FK_LabReports_Doctors_DoctorId` FOREIGN KEY (`DoctorId`) REFERENCES `Doctors` (`DoctorId`),
    CONSTRAINT `FK_LabReports_Patients_PatientId` FOREIGN KEY (`PatientId`) REFERENCES `Patients` (`PatientId`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;

CREATE TABLE `Bills` (
    `BillId` int NOT NULL AUTO_INCREMENT,
    `PatientId` int NOT NULL,
    `AppointmentId` int NULL,
    `ConsultationFee` decimal(10,2) NOT NULL,
    `LabCharges` decimal(10,2) NOT NULL,
    `MedicineCharges` decimal(10,2) NOT NULL,
    `OtherCharges` decimal(10,2) NOT NULL,
    `Discount` decimal(10,2) NOT NULL,
    `Tax` decimal(10,2) NOT NULL,
    `TotalAmount` decimal(10,2) NOT NULL,
    `PaymentStatus` longtext CHARACTER SET utf8mb4 NOT NULL,
    `PaymentDate` datetime(6) NULL,
    `CreatedAt` datetime(6) NOT NULL,
    CONSTRAINT `PK_Bills` PRIMARY KEY (`BillId`),
    CONSTRAINT `FK_Bills_Appointments_AppointmentId` FOREIGN KEY (`AppointmentId`) REFERENCES `Appointments` (`AppointmentId`),
    CONSTRAINT `FK_Bills_Patients_PatientId` FOREIGN KEY (`PatientId`) REFERENCES `Patients` (`PatientId`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;

CREATE TABLE `MedicalRecords` (
    `MedicalRecordId` int NOT NULL AUTO_INCREMENT,
    `PatientId` int NOT NULL,
    `DoctorId` int NOT NULL,
    `AppointmentId` int NULL,
    `Symptoms` longtext CHARACTER SET utf8mb4 NULL,
    `Diagnosis` longtext CHARACTER SET utf8mb4 NULL,
    `Treatment` longtext CHARACTER SET utf8mb4 NULL,
    `MedicalNotes` longtext CHARACTER SET utf8mb4 NULL,
    `FollowUpDate` datetime(6) NULL,
    `CreatedAt` datetime(6) NOT NULL,
    CONSTRAINT `PK_MedicalRecords` PRIMARY KEY (`MedicalRecordId`),
    CONSTRAINT `FK_MedicalRecords_Appointments_AppointmentId` FOREIGN KEY (`AppointmentId`) REFERENCES `Appointments` (`AppointmentId`),
    CONSTRAINT `FK_MedicalRecords_Doctors_DoctorId` FOREIGN KEY (`DoctorId`) REFERENCES `Doctors` (`DoctorId`) ON DELETE CASCADE,
    CONSTRAINT `FK_MedicalRecords_Patients_PatientId` FOREIGN KEY (`PatientId`) REFERENCES `Patients` (`PatientId`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;

CREATE TABLE `Prescriptions` (
    `PrescriptionId` int NOT NULL AUTO_INCREMENT,
    `PatientId` int NOT NULL,
    `DoctorId` int NOT NULL,
    `AppointmentId` int NULL,
    `Diagnosis` longtext CHARACTER SET utf8mb4 NULL,
    `Instructions` longtext CHARACTER SET utf8mb4 NULL,
    `PrescriptionDate` datetime(6) NOT NULL,
    CONSTRAINT `PK_Prescriptions` PRIMARY KEY (`PrescriptionId`),
    CONSTRAINT `FK_Prescriptions_Appointments_AppointmentId` FOREIGN KEY (`AppointmentId`) REFERENCES `Appointments` (`AppointmentId`),
    CONSTRAINT `FK_Prescriptions_Doctors_DoctorId` FOREIGN KEY (`DoctorId`) REFERENCES `Doctors` (`DoctorId`) ON DELETE CASCADE,
    CONSTRAINT `FK_Prescriptions_Patients_PatientId` FOREIGN KEY (`PatientId`) REFERENCES `Patients` (`PatientId`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;

CREATE TABLE `PrescriptionItems` (
    `PrescriptionItemId` int NOT NULL AUTO_INCREMENT,
    `PrescriptionId` int NOT NULL,
    `MedicineName` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Dosage` longtext CHARACTER SET utf8mb4 NULL,
    `Frequency` longtext CHARACTER SET utf8mb4 NULL,
    `Duration` longtext CHARACTER SET utf8mb4 NULL,
    `Instructions` longtext CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_PrescriptionItems` PRIMARY KEY (`PrescriptionItemId`),
    CONSTRAINT `FK_PrescriptionItems_Prescriptions_PrescriptionId` FOREIGN KEY (`PrescriptionId`) REFERENCES `Prescriptions` (`PrescriptionId`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE UNIQUE INDEX `IX_Appointments_DoctorId_AppointmentDate_AppointmentTime` ON `Appointments` (`DoctorId`, `AppointmentDate`, `AppointmentTime`);

CREATE INDEX `IX_Appointments_PatientId` ON `Appointments` (`PatientId`);

CREATE INDEX `IX_AspNetRoleClaims_RoleId` ON `AspNetRoleClaims` (`RoleId`);

CREATE UNIQUE INDEX `RoleNameIndex` ON `AspNetRoles` (`NormalizedName`);

CREATE INDEX `IX_AspNetUserClaims_UserId` ON `AspNetUserClaims` (`UserId`);

CREATE INDEX `IX_AspNetUserLogins_UserId` ON `AspNetUserLogins` (`UserId`);

CREATE INDEX `IX_AspNetUserRoles_RoleId` ON `AspNetUserRoles` (`RoleId`);

CREATE INDEX `EmailIndex` ON `AspNetUsers` (`NormalizedEmail`);

CREATE UNIQUE INDEX `UserNameIndex` ON `AspNetUsers` (`NormalizedUserName`);

CREATE INDEX `IX_Bills_AppointmentId` ON `Bills` (`AppointmentId`);

CREATE INDEX `IX_Bills_PatientId` ON `Bills` (`PatientId`);

CREATE INDEX `IX_Doctors_DepartmentId` ON `Doctors` (`DepartmentId`);

CREATE INDEX `IX_LabReports_DoctorId` ON `LabReports` (`DoctorId`);

CREATE INDEX `IX_LabReports_PatientId` ON `LabReports` (`PatientId`);

CREATE UNIQUE INDEX `IX_MedicalRecords_AppointmentId` ON `MedicalRecords` (`AppointmentId`);

CREATE INDEX `IX_MedicalRecords_DoctorId` ON `MedicalRecords` (`DoctorId`);

CREATE INDEX `IX_MedicalRecords_PatientId` ON `MedicalRecords` (`PatientId`);

CREATE INDEX `IX_Notifications_UserId` ON `Notifications` (`UserId`);

CREATE INDEX `IX_PrescriptionItems_PrescriptionId` ON `PrescriptionItems` (`PrescriptionId`);

CREATE UNIQUE INDEX `IX_Prescriptions_AppointmentId` ON `Prescriptions` (`AppointmentId`);

CREATE INDEX `IX_Prescriptions_DoctorId` ON `Prescriptions` (`DoctorId`);

CREATE INDEX `IX_Prescriptions_PatientId` ON `Prescriptions` (`PatientId`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260928163852_InitialCreate', '8.0.20');

COMMIT;

