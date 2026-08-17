CREATE TABLE dbo.tbl_Invoice
(
    InvoiceId           INT IDENTITY(1,1) PRIMARY KEY,

    InvoiceNumber       NVARCHAR(50) NOT NULL,

    PropertyId          INT NOT NULL,
    UnitId              INT NOT NULL,
    TenantId            INT NOT NULL,

    BillingMonth        DATE NOT NULL,
    InvoiceDate         DATE NOT NULL,
    DueDate             DATE NOT NULL,

    PaymentStatus       NVARCHAR(30) NOT NULL
        CONSTRAINT DF_tbl_Invoice_PaymentStatus
        DEFAULT ('Pending'),

    SubTotal            DECIMAL(18,2) NOT NULL
        CONSTRAINT DF_tbl_Invoice_SubTotal
        DEFAULT (0),

    DiscountAmount      DECIMAL(18,2) NOT NULL
        CONSTRAINT DF_tbl_Invoice_DiscountAmount
        DEFAULT (0),

    LateFee             DECIMAL(18,2) NOT NULL
        CONSTRAINT DF_tbl_Invoice_LateFee
        DEFAULT (0),

    TotalPayable        DECIMAL(18,2) NOT NULL
        CONSTRAINT DF_tbl_Invoice_TotalPayable
        DEFAULT (0),

    Notes               NVARCHAR(1000) NULL,

    IsActive            BIT NOT NULL
        CONSTRAINT DF_tbl_Invoice_IsActive
        DEFAULT (1),

    CreatedOn           DATETIME2 NOT NULL
        CONSTRAINT DF_tbl_Invoice_CreatedOn
        DEFAULT (GETUTCDATE()),

    CreatedBy           NVARCHAR(200) NULL,

    UpdatedOn           DATETIME2 NULL,

    UpdatedBy           NVARCHAR(200) NULL
);
GO