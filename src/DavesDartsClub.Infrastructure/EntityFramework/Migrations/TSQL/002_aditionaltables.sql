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
    WHERE [MigrationId] = N'20250518202233_1_Initial'
)
BEGIN
    CREATE TABLE [Members] (
        [MemberId] uniqueidentifier NOT NULL,
        [MemberName] nvarchar(50) NOT NULL,
        CONSTRAINT [PK_Members] PRIMARY KEY ([MemberId])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250518202233_1_Initial'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250518202233_1_Initial', N'9.0.10');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251019172444_002_additionaltables'
)
BEGIN
    CREATE TABLE [Leagues] (
        [LeagueId] uniqueidentifier NOT NULL,
        [LeagueName] nvarchar(50) NOT NULL,
        CONSTRAINT [PK_Leagues] PRIMARY KEY ([LeagueId])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251019172444_002_additionaltables'
)
BEGIN
    CREATE TABLE [PlayerProfileEntity] (
        [MemberId] uniqueidentifier NOT NULL,
        [PlayerId] uniqueidentifier NOT NULL,
        [Nickname] nvarchar(50) NOT NULL,
        CONSTRAINT [PK_PlayerProfileEntity] PRIMARY KEY ([MemberId]),
        CONSTRAINT [FK_PlayerProfileEntity_Members_MemberId] FOREIGN KEY ([MemberId]) REFERENCES [Members] ([MemberId]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251019172444_002_additionaltables'
)
BEGIN
    CREATE TABLE [Tournaments] (
        [TournamentId] uniqueidentifier NOT NULL,
        [TournamentName] nvarchar(50) NOT NULL,
        CONSTRAINT [PK_Tournaments] PRIMARY KEY ([TournamentId])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251019172444_002_additionaltables'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251019172444_002_additionaltables', N'9.0.10');
END;

COMMIT;
GO

