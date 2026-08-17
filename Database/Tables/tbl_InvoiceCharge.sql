CREATE TABLE dbo.tbl_InvoiceCharge
(
    InvoiceChargeId     INT IDENTITY(1,1) PRIMARY KEY,

    InvoiceId           INT NOT NULL,

    ChargeType          NVARCHAR(50) NOT NULL,

    Description         NVARCHAR(300) NULL,

    Amount              DECIMAL(18,2) NOT NULL
        CONSTRAINT DF_tbl_InvoiceCharge_Amount
        DEFAULT (0),

    -- Electricity-specific fields
    PreviousReading     DECIMAL(18,2) NULL,

    CurrentReading      DECIMAL(18,2) NULL,

    Units               DECIMAL(18,2) NULL,

    Rate                DECIMAL(18,2) NULL,

    IsActive            BIT NOT NULL
        CONSTRAINT DF_tbl_InvoiceCharge_IsActive
        DEFAULT (1),

    CreatedOn           DATETIME2 NOT NULL
        CONSTRAINT DF_tbl_InvoiceCharge_CreatedOn
        DEFAULT (GETUTCDATE()),

    CreatedBy           NVARCHAR(200) NULL,

    UpdatedOn           DATETIME2 NULL,

    UpdatedBy           NVARCHAR(200) NULL
);
GO