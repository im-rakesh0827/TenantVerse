CREATE OR ALTER PROCEDURE dbo.IT_SP_GetAllInvoice
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

        ISNULL(
            SUM(
                CASE
                    WHEN c.ChargeType = 'MonthlyRent'
                    THEN c.Amount
                    ELSE 0
                END
            ), 0
        ) AS MonthlyRent,

        ISNULL(
            SUM(
                CASE
                    WHEN c.ChargeType = 'Electricity'
                    THEN c.Amount
                    ELSE 0
                END
            ), 0
        ) AS ElectricityCharge,

        ISNULL(
            SUM(
                CASE
                    WHEN c.ChargeType = 'Maintenance'
                    THEN c.Amount
                    ELSE 0
                END
            ), 0
        ) AS MaintenanceCharge,

        ISNULL(
            SUM(
                CASE
                    WHEN c.ChargeType = 'Water'
                    THEN c.Amount
                    ELSE 0
                END
            ), 0
        ) AS WaterCharge,

        ISNULL(
            SUM(
                CASE
                    WHEN c.ChargeType = 'LateFee'
                    THEN c.Amount
                    ELSE 0
                END
            ), 0
        ) AS LateFee,

        ISNULL(
            SUM(
                CASE
                    WHEN c.ChargeType = 'Discount'
                    THEN c.Amount
                    ELSE 0
                END
            ), 0
        ) AS Discount,

        i.SubTotal,
        i.TotalPayable,
        i.PaymentStatus,

        i.IsActive,
        i.CreatedOn,
        i.CreatedBy,
        i.UpdatedOn,
        i.UpdatedBy

    FROM dbo.tbl_Invoice i

    INNER JOIN dbo.tbl_Property p
        ON p.PropertyId = i.PropertyId
       AND p.IsActive = 1

    INNER JOIN dbo.tbl_Unit u
        ON u.UnitId = i.UnitId
       AND u.IsActive = 1

    INNER JOIN dbo.tbl_Tenant t
        ON t.TenantId = i.TenantId

    LEFT JOIN dbo.tbl_InvoiceCharge c
        ON c.InvoiceId = i.InvoiceId
       AND c.IsActive = 1

    WHERE i.IsActive = 1

    GROUP BY
        i.InvoiceId,
        i.InvoiceNumber,

        i.PropertyId,
        p.PropertyName,

        i.UnitId,
        u.UnitNumber,

        i.TenantId,
        t.FirstName,
        t.LastName,

        i.BillingMonth,
        i.InvoiceDate,
        i.DueDate,

        i.SubTotal,
        i.TotalPayable,
        i.PaymentStatus,

        i.IsActive,
        i.CreatedOn,
        i.CreatedBy,
        i.UpdatedOn,
        i.UpdatedBy

    ORDER BY
        i.InvoiceId DESC;
END;
GO

