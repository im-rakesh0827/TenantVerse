CREATE INDEX IX_tbl_Unit_PropertyId
ON dbo.tbl_Unit(PropertyId);
GO

CREATE INDEX IX_tbl_Unit_Status
ON dbo.tbl_Unit(Status);
GO

CREATE INDEX IX_tbl_Unit_PropertyId_Status
ON dbo.tbl_Unit(PropertyId, Status);
GO


CREATE UNIQUE INDEX UX_tbl_Unit_PropertyId_UnitNumber
ON dbo.tbl_Unit(PropertyId, UnitNumber)
WHERE IsDeleted = 0;
GO


CREATE UNIQUE INDEX UX_tbl_Tenant_Active_Unit
ON dbo.tbl_Tenant(UnitId)
WHERE IsActive = 1;
GO





ALTER TABLE dbo.tbl_Invoice
ADD CONSTRAINT FK_tbl_Invoice_Property
FOREIGN KEY (PropertyId)
REFERENCES dbo.tbl_Property(PropertyId);
GO

ALTER TABLE dbo.tbl_Invoice
ADD CONSTRAINT FK_tbl_Invoice_Unit
FOREIGN KEY (UnitId)
REFERENCES dbo.tbl_Unit(UnitId);
GO

ALTER TABLE dbo.tbl_Invoice
ADD CONSTRAINT FK_tbl_Invoice_Tenant
FOREIGN KEY (TenantId)
REFERENCES dbo.tbl_Tenant(TenantId);
GO


CREATE UNIQUE INDEX UX_tbl_Invoice_Tenant_BillingMonth
ON dbo.tbl_Invoice
(
    TenantId,
    BillingMonth
)
WHERE IsActive = 1;
GO


CREATE INDEX IX_tbl_InvoiceCharge_InvoiceId
ON dbo.tbl_InvoiceCharge
(
    InvoiceId
)
WHERE IsActive = 1;
GO

ALTER TABLE dbo.tbl_InvoiceCharge
ADD CONSTRAINT FK_tbl_InvoiceCharge_Invoice
FOREIGN KEY (InvoiceId)
REFERENCES dbo.tbl_Invoice(InvoiceId);
GO