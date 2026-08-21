CREATE OR ALTER PROCEDURE [dbo].[IT_SP_GetChargesByInvoiceId] 
(
    @InvoiceId INT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT c.*
    FROM dbo.tbl_InvoiceCharge c
    WHERE c.InvoiceId = @InvoiceId
    AND c.IsActive=1
    ORDER BY c.InvoiceChargeId;
END
GO