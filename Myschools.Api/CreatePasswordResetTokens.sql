USE [MyschoolsSMS];
GO

IF OBJECT_ID(N'dbo.PasswordResetTokens', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PasswordResetTokens
    (
        PasswordResetTokenId int IDENTITY(1,1) NOT NULL,
        UserId int NOT NULL,
        TokenHash varchar(64) NOT NULL,
        ExpiresAt datetime2 NOT NULL,
        CreatedAt datetime2 NOT NULL,

        CONSTRAINT PK_PasswordResetTokens
            PRIMARY KEY (PasswordResetTokenId),
        CONSTRAINT FK_PasswordResetTokens_Users
            FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId)
    );
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = N'UX_PasswordResetTokens_UserId'
      AND object_id = OBJECT_ID(N'dbo.PasswordResetTokens')
)
BEGIN
    CREATE UNIQUE INDEX UX_PasswordResetTokens_UserId
        ON dbo.PasswordResetTokens(UserId);
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_PasswordResetTokens_TokenHash'
      AND object_id = OBJECT_ID(N'dbo.PasswordResetTokens')
)
BEGIN
    CREATE INDEX IX_PasswordResetTokens_TokenHash
        ON dbo.PasswordResetTokens(TokenHash);
END;
GO
