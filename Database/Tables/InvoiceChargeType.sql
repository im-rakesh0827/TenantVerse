-- DROP TYPE IF EXISTS dbo.InvoiceChargeType;
-- GO

CREATE TYPE dbo.InvoiceChargeType AS TABLE
(
    ChargeType          NVARCHAR(50) NOT NULL,
    Description         NVARCHAR(300) NULL,
    Amount              DECIMAL(18,2) NOT NULL,
    PreviousReading     DECIMAL(18,2) NULL,
    CurrentReading      DECIMAL(18,2) NULL,
    Units               DECIMAL(18,2) NULL,
    Rate                DECIMAL(18,2) NULL
);
GO