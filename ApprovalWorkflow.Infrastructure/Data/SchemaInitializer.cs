using Dapper;

namespace ApprovalWorkflow.Infrastructure.Data;

public class SchemaInitializer
{
    private readonly DbConnectionFactory _connectionFactory;

    public SchemaInitializer(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task InitializeAsync()
    {
        using var conn = _connectionFactory.Create();

        await conn.ExecuteAsync(@"
            IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'UserRoles')
            BEGIN
                CREATE TABLE UserRoles (
                    RoleID          INT IDENTITY(1,1) PRIMARY KEY,
                    RoleName        NVARCHAR(50)  NOT NULL,
                    RoleDescription NVARCHAR(200) NOT NULL,
                    SortOrder       INT           NOT NULL
                )
            END");

        await conn.ExecuteAsync(@"
            IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'UserXRoles')
            BEGIN
                CREATE TABLE UserXRoles (
                    UXRID   INT IDENTITY(1,1) PRIMARY KEY,
                    UserID  INT NOT NULL UNIQUE REFERENCES Users(Id),
                    RoleID  INT NOT NULL       REFERENCES UserRoles(RoleID)
                )
            END");

        await conn.ExecuteAsync(@"
            IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Tier')
            BEGIN
                CREATE TABLE Tier (
                    TierID   INT IDENTITY(1,1) PRIMARY KEY,
                    TierName NVARCHAR(50) NOT NULL
                )
            END");

        await conn.ExecuteAsync(@"
            IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Status')
            BEGIN
                CREATE TABLE Status (
                    StatusID   INT IDENTITY(1,1) PRIMARY KEY,
                    StatusName NVARCHAR(100) NOT NULL
                )
            END");

        await conn.ExecuteAsync(@"
            IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'CardRequests')
            BEGIN
                CREATE TABLE CardRequests (
                    RequestID         INT IDENTITY(1,1) PRIMARY KEY,
                    CompanyName       NVARCHAR(200) NOT NULL,
                    JobTitle          NVARCHAR(100) NOT NULL,
                    FirstName         NVARCHAR(100) NOT NULL,
                    LastName          NVARCHAR(100) NOT NULL,
                    Address1          NVARCHAR(200) NOT NULL,
                    Address2          NVARCHAR(200) NULL,
                    City              NVARCHAR(100) NOT NULL,
                    StateProvince     NVARCHAR(100) NOT NULL,
                    Zip               NVARCHAR(20)  NOT NULL,
                    Country           NVARCHAR(100) NOT NULL DEFAULT 'UNITED STATES',
                    TierID            INT NOT NULL REFERENCES Tier(TierID),
                    ExpirationDate    DATE NOT NULL,
                    CardStatus        NVARCHAR(50)  NOT NULL DEFAULT 'Not Printed',
                    Location          NVARCHAR(200) NULL,
                    Manager           NVARCHAR(200) NULL,
                    ApproverUserID    INT NULL REFERENCES Users(Id),
                    Notes             NVARCHAR(MAX) NULL,
                    StatusID          INT NOT NULL REFERENCES Status(StatusID),
                    SubmittedByUserID INT NOT NULL REFERENCES Users(Id),
                    CreatedAt         DATETIME NOT NULL DEFAULT GETDATE()
                )
            END");

        await conn.ExecuteAsync(@"
            IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'RequestAuditLog')
            BEGIN
                CREATE TABLE RequestAuditLog (
                    LogID     INT IDENTITY(1,1) PRIMARY KEY,
                    RequestID INT NOT NULL REFERENCES CardRequests(RequestID),
                    UserID    INT NOT NULL REFERENCES Users(Id),
                    Action    NVARCHAR(500) NOT NULL,
                    LoggedAt  DATETIME NOT NULL DEFAULT GETDATE()
                )
            END");

        await conn.ExecuteAsync(@"
            IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Locations')
            BEGIN
                CREATE TABLE Locations (
                    LocationID   INT IDENTITY(1,1) PRIMARY KEY,
                    LocationName NVARCHAR(200) NOT NULL
                )
            END");

        // Add Email column to Users if not already present
        await conn.ExecuteAsync(@"
            IF NOT EXISTS (
                SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_NAME = 'Users' AND COLUMN_NAME = 'Email'
            )
            BEGIN
                ALTER TABLE Users ADD Email NVARCHAR(100) NOT NULL DEFAULT ''
            END");

        await conn.ExecuteAsync(@"
            IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'PasswordResetTokens')
            BEGIN
                CREATE TABLE PasswordResetTokens (
                    TokenId   INT IDENTITY(1,1) PRIMARY KEY,
                    UserId    INT          NOT NULL REFERENCES Users(Id),
                    Token     NVARCHAR(50) NOT NULL,
                    ExpiresAt DATETIME     NOT NULL,
                    IsUsed    BIT          NOT NULL DEFAULT 0
                )
            END");

        // Add IsActive column to Users if not already present
        await conn.ExecuteAsync(@"
            IF NOT EXISTS (
                SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_NAME = 'Users' AND COLUMN_NAME = 'IsActive'
            )
            BEGIN
                ALTER TABLE Users ADD IsActive BIT NOT NULL DEFAULT 1
            END");

        // Add MustChangePassword column to Users if not already present
        await conn.ExecuteAsync(@"
            IF NOT EXISTS (
                SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_NAME = 'Users' AND COLUMN_NAME = 'MustChangePassword'
            )
            BEGIN
                ALTER TABLE Users ADD MustChangePassword BIT NOT NULL DEFAULT 0
            END");
    }
}
