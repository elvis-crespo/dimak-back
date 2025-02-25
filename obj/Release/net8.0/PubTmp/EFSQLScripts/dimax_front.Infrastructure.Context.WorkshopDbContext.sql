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
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250206223026_Initial'
)
BEGIN
    CREATE TABLE [Users] (
        [Id] uniqueidentifier NOT NULL,
        [Username] nvarchar(50) NOT NULL,
        [PasswordHash] nvarchar(max) NOT NULL,
        [Role] nvarchar(50) NOT NULL,
        [Email] nvarchar(50) NOT NULL,
        [RefreshToken] nvarchar(max) NULL,
        [RefreshTokenExpiryTime] datetime2 NULL,
        [Token] nvarchar(max) NULL,
        [TokenExpiryTime] datetime2 NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250206223026_Initial'
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

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250206223026_Initial'
)
BEGIN
    CREATE TABLE [InstallationHistories] (
        [HistoryId] int NOT NULL IDENTITY,
        [InvoiceNumber] nvarchar(17) NOT NULL,
        [TechnicalFileNumber] nvarchar(15) NULL,
        [TechnicianName] nvarchar(100) NOT NULL,
        [InstallationCompleted] nvarchar(max) NULL,
        [Date] date NOT NULL,
        [PhotoUrl] nvarchar(255) NULL,
        [PlateId] nvarchar(8) NULL,
        CONSTRAINT [PK_InstallationHistories] PRIMARY KEY ([HistoryId]),
        CONSTRAINT [FK_InstallationHistories_Vehicules_PlateId] FOREIGN KEY ([PlateId]) REFERENCES [Vehicules] ([Plate]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250206223026_Initial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_InstallationHistories_InvoiceNumber] ON [InstallationHistories] ([InvoiceNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250206223026_Initial'
)
BEGIN
    CREATE INDEX [IX_InstallationHistories_PlateId] ON [InstallationHistories] ([PlateId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250206223026_Initial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Users_Id_Username_Email] ON [Users] ([Id], [Username], [Email]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250206223026_Initial'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250206223026_Initial', N'9.0.1');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250213052951_Update-table'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250213052951_Update-table', N'9.0.1');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250213062829_Another'
)
BEGIN
    DROP INDEX [IX_InstallationHistories_InvoiceNumber] ON [InstallationHistories];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250213062829_Another'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250213062829_Another', N'9.0.1');
END;

COMMIT;
GO

