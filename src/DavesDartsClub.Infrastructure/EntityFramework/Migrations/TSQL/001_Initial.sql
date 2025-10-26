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
CREATE TABLE [Leagues] (
    [LeagueId] uniqueidentifier NOT NULL,
    [LeagueName] nvarchar(50) NOT NULL,
    CONSTRAINT [PK_Leagues] PRIMARY KEY ([LeagueId])
);

CREATE TABLE [Members] (
    [MemberId] uniqueidentifier NOT NULL,
    [MemberName] nvarchar(50) NOT NULL,
    CONSTRAINT [PK_Members] PRIMARY KEY ([MemberId])
);

CREATE TABLE [Tournaments] (
    [TournamentId] uniqueidentifier NOT NULL,
    [TournamentName] nvarchar(50) NOT NULL,
    CONSTRAINT [PK_Tournaments] PRIMARY KEY ([TournamentId])
);

CREATE TABLE [PlayerProfileEntity] (
    [MemberId] uniqueidentifier NOT NULL,
    [PlayerId] uniqueidentifier NOT NULL,
    [Nickname] nvarchar(50) NOT NULL,
    CONSTRAINT [PK_PlayerProfileEntity] PRIMARY KEY ([MemberId]),
    CONSTRAINT [FK_PlayerProfileEntity_Members_MemberId] FOREIGN KEY ([MemberId]) REFERENCES [Members] ([MemberId]) ON DELETE CASCADE
);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251026222239_001_Initial', N'9.0.10');

COMMIT;
GO

