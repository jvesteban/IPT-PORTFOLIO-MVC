-- Run in SSMS on the target SQL Server instance.
-- EF Core uses __EFMigrationsHistory to apply only pending migrations.
USE [master];
GO

IF DB_ID(N'PortfolioDb') IS NULL
BEGIN
    CREATE DATABASE [PortfolioDb];
END;
GO

USE [PortfolioDb];
GO

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
    WHERE [MigrationId] = N'20261114120031_CreatePortfolioTables'
)
BEGIN
    CREATE TABLE [Users] (
        [UserId] int NOT NULL IDENTITY,
        [FullName] nvarchar(201) NOT NULL,
        [Username] nvarchar(100) NOT NULL,
        [Email] nvarchar(256) NULL,
        [PasswordHash] nvarchar(500) NOT NULL,
        [UserRole] nvarchar(20) NOT NULL DEFAULT N'Student',
        [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
        [CreatedAt] datetime2 NOT NULL DEFAULT (SYSUTCDATETIME()),
        [LastLoginAt] datetime2 NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([UserId]),
        CONSTRAINT [CK_Users_UserRole] CHECK ([UserRole] IN (N'Admin', N'Student'))
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261114120031_CreatePortfolioTables'
)
BEGIN
    CREATE TABLE [StudentProfiles] (
        [StudentProfileId] int NOT NULL IDENTITY,
        [UserId] int NOT NULL,
        [FirstName] nvarchar(100) NOT NULL,
        [MiddleName] nvarchar(100) NULL,
        [LastName] nvarchar(100) NOT NULL,
        [BirthDate] date NULL,
        [Introduction] nvarchar(max) NULL,
        [CareerGoal] nvarchar(max) NULL,
        [BusinessName] nvarchar(100) NULL,
        [AboutBusiness] nvarchar(2000) NULL,
        [Offering] nvarchar(2100) NULL,
        [Owner] nvarchar(2100) NULL,
        [ProfilePhotoUrl] nvarchar(500) NULL,
        [UpdatedAt] datetime2 NOT NULL DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT [PK_StudentProfiles] PRIMARY KEY ([StudentProfileId]),
        CONSTRAINT [FK_StudentProfiles_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261114120031_CreatePortfolioTables'
)
BEGIN
    CREATE TABLE [ContactInformation] (
        [ContactInformationId] int NOT NULL IDENTITY,
        [StudentProfileId] int NOT NULL,
        [Email] nvarchar(256) NULL,
        [Phone] nvarchar(50) NULL,
        [Address] nvarchar(500) NULL,
        [Website] nvarchar(500) NULL,
        [UpdatedAt] datetime2 NOT NULL DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT [PK_ContactInformation] PRIMARY KEY ([ContactInformationId]),
        CONSTRAINT [FK_ContactInformation_StudentProfiles_StudentProfileId] FOREIGN KEY ([StudentProfileId]) REFERENCES [StudentProfiles] ([StudentProfileId]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261114120031_CreatePortfolioTables'
)
BEGIN
    CREATE TABLE [EducationHistory] (
        [EducationHistoryId] int NOT NULL IDENTITY,
        [StudentProfileId] int NOT NULL,
        [EducationLevel] nvarchar(30) NOT NULL,
        [Grade] nvarchar(50) NOT NULL,
        [StrandTrack] nvarchar(200) NULL,
        [CollegeCourse] nvarchar(200) NULL,
        [SchoolName] nvarchar(200) NOT NULL,
        [StartYear] smallint NULL,
        [EndYear] smallint NULL,
        [DisplayOrder] int NOT NULL DEFAULT 0,
        CONSTRAINT [PK_EducationHistory] PRIMARY KEY ([EducationHistoryId]),
        CONSTRAINT [CK_EducationHistory_Level] CHECK ([EducationLevel] IN (N'Elementary', N'Junior High', N'Senior High', N'College')),
        CONSTRAINT [FK_EducationHistory_StudentProfiles_StudentProfileId] FOREIGN KEY ([StudentProfileId]) REFERENCES [StudentProfiles] ([StudentProfileId]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261114120031_CreatePortfolioTables'
)
BEGIN
    CREATE TABLE [Hobbies] (
        [HobbyId] int NOT NULL IDENTITY,
        [StudentProfileId] int NOT NULL,
        [Name] nvarchar(100) NOT NULL,
        [DisplayOrder] int NOT NULL DEFAULT 0,
        CONSTRAINT [PK_Hobbies] PRIMARY KEY ([HobbyId]),
        CONSTRAINT [FK_Hobbies_StudentProfiles_StudentProfileId] FOREIGN KEY ([StudentProfileId]) REFERENCES [StudentProfiles] ([StudentProfileId]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261114120031_CreatePortfolioTables'
)
BEGIN
    CREATE TABLE [Projects] (
        [ProjectId] int NOT NULL IDENTITY,
        [StudentProfileId] int NOT NULL,
        [Title] nvarchar(200) NOT NULL,
        [Description] nvarchar(max) NULL,
        [ProjectUrl] nvarchar(500) NULL,
        [ImageId] nvarchar(100) NULL,
        [DisplayOrder] int NOT NULL DEFAULT 0,
        CONSTRAINT [PK_Projects] PRIMARY KEY ([ProjectId]),
        CONSTRAINT [FK_Projects_StudentProfiles_StudentProfileId] FOREIGN KEY ([StudentProfileId]) REFERENCES [StudentProfiles] ([StudentProfileId]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261114120031_CreatePortfolioTables'
)
BEGIN
    CREATE TABLE [Skills] (
        [SkillId] int NOT NULL IDENTITY,
        [StudentProfileId] int NOT NULL,
        [Name] nvarchar(100) NOT NULL,
        [DisplayOrder] int NOT NULL DEFAULT 0,
        CONSTRAINT [PK_Skills] PRIMARY KEY ([SkillId]),
        CONSTRAINT [FK_Skills_StudentProfiles_StudentProfileId] FOREIGN KEY ([StudentProfileId]) REFERENCES [StudentProfiles] ([StudentProfileId]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261114120031_CreatePortfolioTables'
)
BEGIN
    CREATE TABLE [SocialLinks] (
        [SocialLinkId] int NOT NULL IDENTITY,
        [StudentProfileId] int NOT NULL,
        [Label] nvarchar(100) NOT NULL,
        [Url] nvarchar(500) NOT NULL,
        [IsVisible] bit NOT NULL DEFAULT CAST(1 AS bit),
        [DisplayOrder] int NOT NULL DEFAULT 0,
        CONSTRAINT [PK_SocialLinks] PRIMARY KEY ([SocialLinkId]),
        CONSTRAINT [FK_SocialLinks_StudentProfiles_StudentProfileId] FOREIGN KEY ([StudentProfileId]) REFERENCES [StudentProfiles] ([StudentProfileId]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261114120031_CreatePortfolioTables'
)
BEGIN
    CREATE UNIQUE INDEX [IX_ContactInformation_StudentProfileId] ON [ContactInformation] ([StudentProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261114120031_CreatePortfolioTables'
)
BEGIN
    CREATE UNIQUE INDEX [IX_EducationHistory_StudentProfileId_EducationLevel_Grade] ON [EducationHistory] ([StudentProfileId], [EducationLevel], [Grade]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261114120031_CreatePortfolioTables'
)
BEGIN
    CREATE INDEX [IX_Hobbies_StudentProfileId] ON [Hobbies] ([StudentProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261114120031_CreatePortfolioTables'
)
BEGIN
    CREATE INDEX [IX_Projects_StudentProfileId] ON [Projects] ([StudentProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261114120031_CreatePortfolioTables'
)
BEGIN
    CREATE INDEX [IX_Skills_StudentProfileId] ON [Skills] ([StudentProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261114120031_CreatePortfolioTables'
)
BEGIN
    CREATE INDEX [IX_SocialLinks_StudentProfileId] ON [SocialLinks] ([StudentProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261114120031_CreatePortfolioTables'
)
BEGIN
    CREATE UNIQUE INDEX [IX_StudentProfiles_UserId] ON [StudentProfiles] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261114120031_CreatePortfolioTables'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Users_Username] ON [Users] ([Username]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261114120031_CreatePortfolioTables'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261114120031_CreatePortfolioTables', N'10.0.14');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261115084245_AllowPortfolioParagraphs'
)
BEGIN
    DECLARE @var nvarchar(max);
    SELECT @var = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[StudentProfiles]') AND [c].[name] = N'AboutBusiness');
    IF @var IS NOT NULL EXEC(N'ALTER TABLE [StudentProfiles] DROP CONSTRAINT ' + @var + ';');
    ALTER TABLE [StudentProfiles] ALTER COLUMN [AboutBusiness] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261115084245_AllowPortfolioParagraphs'
)
BEGIN
    DECLARE @var1 nvarchar(max);
    SELECT @var1 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Skills]') AND [c].[name] = N'Name');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Skills] DROP CONSTRAINT ' + @var1 + ';');
    ALTER TABLE [Skills] ALTER COLUMN [Name] nvarchar(max) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261115084245_AllowPortfolioParagraphs'
)
BEGIN
    DECLARE @var2 nvarchar(max);
    SELECT @var2 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Hobbies]') AND [c].[name] = N'Name');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [Hobbies] DROP CONSTRAINT ' + @var2 + ';');
    ALTER TABLE [Hobbies] ALTER COLUMN [Name] nvarchar(max) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261115084245_AllowPortfolioParagraphs'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261115084245_AllowPortfolioParagraphs', N'10.0.14');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261115090629_ExpandOfferingDescriptions'
)
BEGIN
    DECLARE @var3 nvarchar(max);
    SELECT @var3 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[StudentProfiles]') AND [c].[name] = N'Offering');
    IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [StudentProfiles] DROP CONSTRAINT ' + @var3 + ';');
    ALTER TABLE [StudentProfiles] ALTER COLUMN [Offering] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261115090629_ExpandOfferingDescriptions'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261115090629_ExpandOfferingDescriptions', N'10.0.14');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261116181405_QueueDeactivationEmails'
)
BEGIN
    CREATE TABLE [PendingDeactivationEmails] (
        [UserId] int NOT NULL,
        [Recipient] nvarchar(256) NOT NULL,
        [Reason] nvarchar(500) NOT NULL,
        [NextAttemptAt] datetime2 NOT NULL,
        CONSTRAINT [PK_PendingDeactivationEmails] PRIMARY KEY ([UserId]),
        CONSTRAINT [FK_PendingDeactivationEmails_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261116181405_QueueDeactivationEmails'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261116181405_QueueDeactivationEmails', N'10.0.14');
END;

COMMIT;
GO

/* Example admin statistics query: */
-- SELECT COUNT(*) AS TotalUsers,
--        SUM(CASE WHEN IsActive = 1 THEN 1 ELSE 0 END) AS ActiveUsers,
--        SUM(CASE WHEN IsActive = 0 THEN 1 ELSE 0 END) AS InactiveUsers
-- FROM dbo.Users;
