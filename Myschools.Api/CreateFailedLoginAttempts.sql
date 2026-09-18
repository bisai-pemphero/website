USE [MyschoolsSMS];
GO

IF OBJECT_ID(N'dbo.FailedLoginAttempts', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.FailedLoginAttempts
    (
        Username nvarchar(256) NOT NULL,
        FailedAttempts int NOT NULL,
        LockoutUntil datetime2 NULL,
        LastFailedAt datetime2 NOT NULL,

        CONSTRAINT PK_FailedLoginAttempts
            PRIMARY KEY (Username)
    );
END;
GO