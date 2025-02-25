IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250124043441_Initial'
)
BEGIN
    CREATE TABLE [Vehicules] (
        [Plate] nvarchar(8) NOT NULL,
        [OwnerName] nvarchar(100) NOT NULL,
        [Brand] nvarchar(50) NULL,
        [Model] nvarchar(50) NULL,
        [Year] int NULL,
        CONSTRAINT [PK_Vehicules] PRIMARY KEY ([Plate])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250124043441_Initial'
)
BEGIN
    CREATE TABLE [InstallationHistories] (
        [HistoryId] int NOT NULL IDENTITY,
        [InvoiceNumber] nvarchar(17) NOT NULL,
        [TechnicalFileNumber] nvarchar(15) NULL,
        [TechnicianName] nvarchar(100) NOT NULL,
        [InstallationCompleted] nvarchar(255) NULL,
        [Date] datetime2 NULL,
        [PhotoUrl] nvarchar(255) NULL,
        [PlateId] nvarchar(8) NULL,
        CONSTRAINT [PK_InstallationHistories] PRIMARY KEY ([HistoryId]),
        CONSTRAINT [FK_InstallationHistories_Vehicules_PlateId] FOREIGN KEY ([PlateId]) REFERENCES [Vehicules] ([Plate]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250124043441_Initial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_InstallationHistories_InvoiceNumber] ON [InstallationHistories] ([InvoiceNumber]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250124043441_Initial'
)
BEGIN
    CREATE INDEX [IX_InstallationHistories_PlateId] ON [InstallationHistories] ([PlateId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250124043441_Initial'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250124043441_Initial', N'8.0.12');
END;
GO

COMMIT;
GO

