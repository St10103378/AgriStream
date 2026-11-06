-- AgriStream SQL Database Setup
-- Run against your Azure SQL Database

-- Users table for login + roles
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE Users (
        Id              INT IDENTITY(1,1) PRIMARY KEY,
        Username        NVARCHAR(100) NOT NULL UNIQUE,
        PasswordHash    NVARCHAR(256) NOT NULL,
        Role            NVARCHAR(50)  NOT NULL,   -- Farmer, Agronomist, Admin
        CreatedAt       DATETIME2     NOT NULL DEFAULT GETUTCDATE()
    );
END
GO

-- Device master registry (updated by Queue-triggered Function)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Devices')
BEGIN
    CREATE TABLE Devices (
        DeviceId        NVARCHAR(50)  PRIMARY KEY,
        Status          NVARCHAR(50)  NOT NULL DEFAULT 'Idle',  -- Idle, Irrigating, Offline, Error
        LastCommand     NVARCHAR(100) NULL,
        LastUpdated     DATETIME2     NOT NULL DEFAULT GETUTCDATE(),
        Location        NVARCHAR(100) NULL
    );
END
GO

-- Seed data (optional)
INSERT INTO Users (Username, PasswordHash, Role)
VALUES 
    ('farmer1',  'AQAAAAIAAYagAAAAEHashExampleReplaceWithRealBCryptOrIdentityHash', 'Farmer'),
    ('agro1',    'AQAAAAIAAYagAAAAEHashExampleReplaceWithRealBCryptOrIdentityHash', 'Agronomist'),
    ('admin',    'AQAAAAIAAYagAAAAEHashExampleReplaceWithRealBCryptOrIdentityHash', 'Admin');
GO

INSERT INTO Devices (DeviceId, Status, Location)
VALUES 
    ('SENSOR-001', 'Idle', 'Field A - North'),
    ('SENSOR-002', 'Idle', 'Field B - South'),
    ('IRRIG-101',  'Idle', 'Pivot 1');
GO
