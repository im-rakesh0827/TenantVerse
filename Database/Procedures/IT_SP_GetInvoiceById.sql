CREATE OR ALTER PROCEDURE dbo.IT_SP_GetInvoiceById
(
    @InvoiceId INT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        i.InvoiceId,
        i.InvoiceNumber,
        i.PropertyId,
        p.PropertyName,
        i.UnitId,
        u.UnitNumber,
        i.TenantId,
        CONCAT(t.FirstName, ' ', t.LastName) AS TenantName,
        i.BillingMonth,
        i.InvoiceDate,
        i.DueDate,
        i.SubTotal,
        i.DiscountAmount,
        i.LateFee,
        i.TotalPayable,
        i.PaymentStatus,
        i.Notes,
        i.IsActive,
        i.CreatedOn,
        i.CreatedBy,
        i.UpdatedOn,
        i.UpdatedBy
    FROM dbo.tbl_Invoice i
    INNER JOIN dbo.tbl_Property p
        ON p.PropertyId = i.PropertyId
    INNER JOIN dbo.tbl_Unit u
        ON u.UnitId = i.UnitId
    INNER JOIN dbo.tbl_Tenant t
        ON t.TenantId = i.TenantId
    WHERE i.InvoiceId = @InvoiceId
      AND i.IsActive = 1;


    SELECT
        InvoiceChargeId,
        InvoiceId,
        ChargeType,
        Description,
        Amount,
        PreviousReading,
        CurrentReading,
        Units,
        Rate,
        IsActive,
        CreatedOn,
        CreatedBy,
        UpdatedOn,
        UpdatedBy
    FROM dbo.tbl_InvoiceCharge
    WHERE InvoiceId = @InvoiceId
      AND IsActive = 1
    ORDER BY InvoiceChargeId;
END;
GO