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
    WHERE [MigrationId] = N'20260728192335_1_Initial'
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
    WHERE [MigrationId] = N'20260728192335_1_Initial'
)
BEGIN
    CREATE TABLE [Members] (
        [MemberId] uniqueidentifier NOT NULL,
        [MemberName] nvarchar(50) NOT NULL,
        [FirstName] nvarchar(50) NOT NULL,
        [LastName] nvarchar(50) NOT NULL,
        CONSTRAINT [PK_Members] PRIMARY KEY ([MemberId])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728192335_1_Initial'
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
    WHERE [MigrationId] = N'20260728192335_1_Initial'
)
BEGIN
    CREATE TABLE [Venues] (
        [VenueId] uniqueidentifier NOT NULL,
        [VenueName] nvarchar(100) NOT NULL,
        [Address] nvarchar(200) NOT NULL,
        [City] nvarchar(100) NOT NULL,
        [Postcode] nvarchar(20) NOT NULL,
        [ContactPhone] nvarchar(20) NULL,
        [ContactEmail] nvarchar(100) NULL,
        [NumberOfBoards] int NOT NULL,
        [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
        CONSTRAINT [PK_Venues] PRIMARY KEY ([VenueId])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728192335_1_Initial'
)
BEGIN
    CREATE TABLE [Seasons] (
        [SeasonId] uniqueidentifier NOT NULL,
        [SeasonName] nvarchar(50) NOT NULL,
        [LeagueId] uniqueidentifier NOT NULL,
        [StartDate] datetime2 NOT NULL,
        [EndDate] datetime2 NOT NULL,
        [IsActive] bit NOT NULL DEFAULT CAST(0 AS bit),
        CONSTRAINT [PK_Seasons] PRIMARY KEY ([SeasonId]),
        CONSTRAINT [FK_Seasons_Leagues_LeagueId] FOREIGN KEY ([LeagueId]) REFERENCES [Leagues] ([LeagueId]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728192335_1_Initial'
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
    WHERE [MigrationId] = N'20260728192335_1_Initial'
)
BEGIN
    CREATE TABLE [Divisions] (
        [DivisionId] uniqueidentifier NOT NULL,
        [DivisionName] nvarchar(100) NOT NULL,
        [SeasonId] uniqueidentifier NOT NULL,
        [LeagueId] uniqueidentifier NOT NULL,
        [DivisionLevel] int NOT NULL,
        [IsActive] bit NOT NULL,
        [DisplayOrder] int NOT NULL,
        CONSTRAINT [PK_Divisions] PRIMARY KEY ([DivisionId]),
        CONSTRAINT [FK_Divisions_Leagues_LeagueId] FOREIGN KEY ([LeagueId]) REFERENCES [Leagues] ([LeagueId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Divisions_Seasons_SeasonId] FOREIGN KEY ([SeasonId]) REFERENCES [Seasons] ([SeasonId]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728192335_1_Initial'
)
BEGIN
    CREATE TABLE [Teams] (
        [TeamId] uniqueidentifier NOT NULL,
        [TeamName] nvarchar(50) NOT NULL,
        [LeagueId] uniqueidentifier NOT NULL,
        [CaptainId] uniqueidentifier NOT NULL,
        [HomeVenueId] uniqueidentifier NULL,
        [DivisionId] uniqueidentifier NOT NULL,
        [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
        CONSTRAINT [PK_Teams] PRIMARY KEY ([TeamId]),
        CONSTRAINT [FK_Teams_Divisions_DivisionId] FOREIGN KEY ([DivisionId]) REFERENCES [Divisions] ([DivisionId]) ON DELETE CASCADE,
        CONSTRAINT [FK_Teams_Leagues_LeagueId] FOREIGN KEY ([LeagueId]) REFERENCES [Leagues] ([LeagueId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Teams_Members_CaptainId] FOREIGN KEY ([CaptainId]) REFERENCES [Members] ([MemberId]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728192335_1_Initial'
)
BEGIN
    CREATE TABLE [Fixtures] (
        [FixtureId] uniqueidentifier NOT NULL,
        [DivisionId] uniqueidentifier NOT NULL,
        [SeasonId] uniqueidentifier NOT NULL,
        [HomeTeamId] uniqueidentifier NOT NULL,
        [AwayTeamId] uniqueidentifier NOT NULL,
        [VenueId] uniqueidentifier NOT NULL,
        [ScheduledDate] datetime2 NOT NULL,
        [RoundNumber] int NOT NULL,
        [Status] int NOT NULL DEFAULT 0,
        CONSTRAINT [PK_Fixtures] PRIMARY KEY ([FixtureId]),
        CONSTRAINT [FK_Fixtures_Divisions_DivisionId] FOREIGN KEY ([DivisionId]) REFERENCES [Divisions] ([DivisionId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Fixtures_Seasons_SeasonId] FOREIGN KEY ([SeasonId]) REFERENCES [Seasons] ([SeasonId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Fixtures_Teams_AwayTeamId] FOREIGN KEY ([AwayTeamId]) REFERENCES [Teams] ([TeamId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Fixtures_Teams_HomeTeamId] FOREIGN KEY ([HomeTeamId]) REFERENCES [Teams] ([TeamId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Fixtures_Venues_VenueId] FOREIGN KEY ([VenueId]) REFERENCES [Venues] ([VenueId]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728192335_1_Initial'
)
BEGIN
    CREATE TABLE [MatchResults] (
        [MatchResultId] uniqueidentifier NOT NULL,
        [FixtureId] uniqueidentifier NOT NULL,
        [HomeTeamScore] int NOT NULL,
        [AwayTeamScore] int NOT NULL,
        [SubmittedByMemberId] uniqueidentifier NOT NULL,
        [SubmittedDate] datetime2 NOT NULL,
        [ConfirmedDate] datetime2 NULL,
        [Status] int NOT NULL DEFAULT 0,
        CONSTRAINT [PK_MatchResults] PRIMARY KEY ([MatchResultId]),
        CONSTRAINT [FK_MatchResults_Fixtures_FixtureId] FOREIGN KEY ([FixtureId]) REFERENCES [Fixtures] ([FixtureId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_MatchResults_Members_SubmittedByMemberId] FOREIGN KEY ([SubmittedByMemberId]) REFERENCES [Members] ([MemberId]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728192335_1_Initial'
)
BEGIN
    CREATE INDEX [IX_Divisions_LeagueId] ON [Divisions] ([LeagueId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728192335_1_Initial'
)
BEGIN
    CREATE INDEX [IX_Divisions_SeasonId] ON [Divisions] ([SeasonId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728192335_1_Initial'
)
BEGIN
    CREATE INDEX [IX_Fixtures_AwayTeamId] ON [Fixtures] ([AwayTeamId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728192335_1_Initial'
)
BEGIN
    CREATE INDEX [IX_Fixtures_DivisionId] ON [Fixtures] ([DivisionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728192335_1_Initial'
)
BEGIN
    CREATE INDEX [IX_Fixtures_HomeTeamId] ON [Fixtures] ([HomeTeamId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728192335_1_Initial'
)
BEGIN
    CREATE INDEX [IX_Fixtures_SeasonId] ON [Fixtures] ([SeasonId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728192335_1_Initial'
)
BEGIN
    CREATE INDEX [IX_Fixtures_VenueId] ON [Fixtures] ([VenueId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728192335_1_Initial'
)
BEGIN
    CREATE INDEX [IX_MatchResults_FixtureId] ON [MatchResults] ([FixtureId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728192335_1_Initial'
)
BEGIN
    CREATE INDEX [IX_MatchResults_SubmittedByMemberId] ON [MatchResults] ([SubmittedByMemberId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728192335_1_Initial'
)
BEGIN
    CREATE INDEX [IX_Members_LastName_FirstName] ON [Members] ([LastName], [FirstName]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728192335_1_Initial'
)
BEGIN
    CREATE INDEX [IX_Seasons_LeagueId] ON [Seasons] ([LeagueId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728192335_1_Initial'
)
BEGIN
    CREATE INDEX [IX_Teams_CaptainId] ON [Teams] ([CaptainId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728192335_1_Initial'
)
BEGIN
    CREATE INDEX [IX_Teams_DivisionId] ON [Teams] ([DivisionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728192335_1_Initial'
)
BEGIN
    CREATE INDEX [IX_Teams_LeagueId] ON [Teams] ([LeagueId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728192335_1_Initial'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260728192335_1_Initial', N'10.0.10');
END;

COMMIT;
GO

