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
CREATE TABLE [Members] (
    [MemberId] uniqueidentifier NOT NULL,
    [MemberName] nvarchar(50) NOT NULL,
    CONSTRAINT [PK_Members] PRIMARY KEY ([MemberId])
);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250518202233_1_Initial', N'9.0.5');

COMMIT;
GO

