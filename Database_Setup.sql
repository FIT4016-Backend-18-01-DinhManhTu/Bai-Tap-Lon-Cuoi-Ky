-- SQL Server Database Setup Script for Blood Bank Manager
-- Run this script in SQL Server Management Studio (SSMS) or SQL Server Express

-- 1. Create Database
CREATE DATABASE [BloodBankDB];
GO

-- 2. Use the database
USE [BloodBankDB];
GO

-- Note: Entity Framework Core will create all tables automatically 
-- when you run: dotnet ef database update

-- Optional: Manual table creation (if not using Entity Framework migrations)
-- Uncomment the following code if needed

/*
-- Create BloodTypes table
CREATE TABLE [BloodTypes] (
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [TypeName] NVARCHAR(50) NOT NULL UNIQUE,
    [Description] NVARCHAR(500)
);

-- Create Bloods table
CREATE TABLE [Bloods] (
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [UnitNumber] NVARCHAR(50) NOT NULL UNIQUE,
    [BloodTypeId] INT NOT NULL,
    [CollectionDate] DATETIME2 NOT NULL,
    [ExpiryDate] DATETIME2 NOT NULL,
    [Volume] FLOAT NOT NULL,
    [Status] INT NOT NULL DEFAULT 0,
    [DonorName] NVARCHAR(200),
    [Location] NVARCHAR(200),
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UsedAt] DATETIME2,
    [DiscardedAt] DATETIME2,
    [DiscardReason] NVARCHAR(500),
    FOREIGN KEY ([BloodTypeId]) REFERENCES [BloodTypes]([Id])
);

-- Create Patients table
CREATE TABLE [Patients] (
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [Name] NVARCHAR(200) NOT NULL,
    [PatientCode] NVARCHAR(50) NOT NULL UNIQUE,
    [BloodTypeId] INT NOT NULL,
    [DateOfBirth] DATETIME2 NOT NULL,
    [Gender] NVARCHAR(20),
    [Hospital] NVARCHAR(200),
    [Ward] NVARCHAR(100),
    [AdmissionDate] DATETIME2 NOT NULL,
    [PhoneNumber] NVARCHAR(20),
    [Email] NVARCHAR(200),
    [MedicalCondition] NVARCHAR(500),
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY ([BloodTypeId]) REFERENCES [BloodTypes]([Id])
);

-- Create BloodTransfusions table
CREATE TABLE [BloodTransfusions] (
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [PatientId] INT NOT NULL,
    [BloodId] INT NOT NULL,
    [QuantityTransfused] FLOAT NOT NULL,
    [TransfusionDate] DATETIME2 NOT NULL,
    [PerformedBy] NVARCHAR(200),
    [Notes] NVARCHAR(500),
    [Status] INT NOT NULL DEFAULT 2,
    FOREIGN KEY ([PatientId]) REFERENCES [Patients]([Id]),
    FOREIGN KEY ([BloodId]) REFERENCES [Bloods]([Id])
);

-- Create BloodInventories table
CREATE TABLE [BloodInventories] (
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [BloodTypeId] INT NOT NULL UNIQUE,
    [TotalUnits] INT NOT NULL DEFAULT 0,
    [TotalVolume] FLOAT NOT NULL DEFAULT 0,
    [AvailableUnits] INT NOT NULL DEFAULT 0,
    [ReservedUnits] INT NOT NULL DEFAULT 0,
    [LowStockThreshold] FLOAT NOT NULL DEFAULT 1000,
    [LastUpdated] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY ([BloodTypeId]) REFERENCES [BloodTypes]([Id]) ON DELETE CASCADE
);

-- Create BloodExpiryAlerts table
CREATE TABLE [BloodExpiryAlerts] (
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [BloodId] INT NOT NULL,
    [AlertDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [DaysUntilExpiry] INT NOT NULL,
    [IsAcknowledged] BIT NOT NULL DEFAULT 0,
    [AcknowledgedAt] DATETIME2,
    [AcknowledgedBy] NVARCHAR(200),
    [Status] INT NOT NULL DEFAULT 0,
    FOREIGN KEY ([BloodId]) REFERENCES [Bloods]([Id]) ON DELETE CASCADE
);

-- Create indexes for performance
CREATE INDEX [IX_Blood_ExpiryDate] ON [Bloods]([ExpiryDate]);
CREATE INDEX [IX_Blood_Status] ON [Bloods]([Status]);
CREATE INDEX [IX_BloodTransfusion_TransfusionDate] ON [BloodTransfusions]([TransfusionDate]);
CREATE INDEX [IX_BloodExpiryAlert_AlertDate] ON [BloodExpiryAlerts]([AlertDate]);
CREATE INDEX [IX_BloodExpiryAlert_Status] ON [BloodExpiryAlerts]([Status]);

-- Insert initial blood types
INSERT INTO [BloodTypes] ([TypeName], [Description]) VALUES
(N'O_NEGATIVE', N'O âm tính - Nhóm máu phổ biến'),
(N'O_POSITIVE', N'O dương tính'),
(N'A_NEGATIVE', N'A âm tính'),
(N'A_POSITIVE', N'A dương tính - Nhóm máu phổ biến'),
(N'B_NEGATIVE', N'B âm tính'),
(N'B_POSITIVE', N'B dương tính'),
(N'AB_NEGATIVE', N'AB âm tính - Nhóm máu hiếm'),
(N'AB_POSITIVE', N'AB dương tính');
*/

-- ============================================
-- CONNECTION STRING FOR APPLICATION
-- ============================================
-- Windows Authentication:
-- Server=.;Database=BloodBankDB;Trusted_Connection=true;Encrypt=false;

-- SQL Authentication (replace USERNAME and PASSWORD):
-- Server=.;Database=BloodBankDB;User Id=USERNAME;Password=PASSWORD;Encrypt=false;
