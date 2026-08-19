CREATE TABLE dbo.tbl_InvoicePayment
(
    InvoicePaymentId INT IDENTITY(1,1) NOT NULL,
    InvoiceId INT NOT NULL,

    PaymentAmount DECIMAL(18,2) NOT NULL,
    PaymentDate DATE NOT NULL,

    PaymentMethod NVARCHAR(50) NOT NULL,
    PaymentStatus NVARCHAR(30) NOT NULL,

    TransactionReference NVARCHAR(200) NULL,
    Notes NVARCHAR(1000) NULL,

    IsActive BIT NOT NULL,

    CreatedOn DATETIME2(7) NOT NULL,
    CreatedBy NVARCHAR(200) NULL,

    UpdatedOn DATETIME2(7) NULL,
    UpdatedBy NVARCHAR(200) NULL,

    CONSTRAINT PK_tbl_InvoicePayment
        PRIMARY KEY CLUSTERED (InvoicePaymentId),

    CONSTRAINT FK_tbl_InvoicePayment_Invoice
        FOREIGN KEY (InvoiceId)
        REFERENCES dbo.tbl_Invoice(InvoiceId)
);
GO



