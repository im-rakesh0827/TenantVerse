CREATE TABLE dbo.tbl_Tenant
(
    TenantId            INT IDENTITY(1,1) PRIMARY KEY,
    PropertyId          INT NOT NULL,
    UnitId              INT NOT NULL,

    FirstName           NVARCHAR(100) NOT NULL,
    LastName            NVARCHAR(100) NULL,

    Email               NVARCHAR(200) NULL,
    PhoneNumber         NVARCHAR(20) NULL,

    EmergencyContactName
                        NVARCHAR(150) NULL,
    EmergencyContactPhone
                        NVARCHAR(20) NULL,

    LeaseStartDate      DATE NULL,
    LeaseEndDate        DATE NULL,

    MonthlyRent         DECIMAL(18,2) NULL,
    SecurityDeposit     DECIMAL(18,2) NULL,

    Status              NVARCHAR(30) NOT NULL
                        DEFAULT 'Active',

    IsActive            BIT NOT NULL
                        DEFAULT 1,

    CreatedDate         DATETIME2 NOT NULL
                        DEFAULT GETDATE(),

    ModifiedDate        DATETIME2 NULL
);