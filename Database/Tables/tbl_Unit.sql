CREATE TABLE dbo.tbl_Unit
(
    UnitId              INT IDENTITY(1,1) NOT NULL,
    PropertyId          INT NOT NULL,

    UnitNumber          NVARCHAR(50) NOT NULL,
    UnitType            NVARCHAR(50) NULL,
    FloorNumber         INT NULL,

    Bedrooms            INT NULL,
    Bathrooms           INT NULL,
    Area                DECIMAL(12,2) NULL,

    MonthlyRent         DECIMAL(18,2) NULL,
    SecurityDeposit     DECIMAL(18,2) NULL,

    Status              NVARCHAR(30) NOT NULL
        CONSTRAINT DF_tbl_Unit_Status DEFAULT ('Available'),

    IsActive            BIT NOT NULL
        CONSTRAINT DF_tbl_Unit_IsActive DEFAULT (1),

    CreatedDate         DATETIME2(0) NOT NULL
        CONSTRAINT DF_tbl_Unit_CreatedDate DEFAULT (GETUTCDATE()),

    ModifiedDate        DATETIME2(0) NULL,

    CONSTRAINT PK_tbl_Unit
        PRIMARY KEY (UnitId),

    CONSTRAINT FK_tbl_Unit_Property
        FOREIGN KEY (PropertyId)
        REFERENCES dbo.tbl_Property(PropertyId),

    CONSTRAINT CK_tbl_Unit_Status
        CHECK
        (
            Status IN
            (
                'Available',
                'Occupied',
                'Reserved',
                'Maintenance',
                'Inactive'
            )
        )
);
GO