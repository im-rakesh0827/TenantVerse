CREATE OR ALTER  FUNCTION dbo.IT_FN_HasOutstandingTenantInvoices 
( 
    @TenantId INT 
) 
RETURNS BIT 
AS 
BEGIN 
    -- DECLARE @TenantId INT =74
    DECLARE @HasOutstandingInvoice BIT = 0; 
 
    DECLARE @TotalPayable DECIMAL(18, 2) = 0; 
    DECLARE @TotalPaid DECIMAL(18, 2) = 0; 
 
    -- ========================================== 
    -- Total Payable from Active Invoices 
    -- ========================================== 
 
    SELECT 
        @TotalPayable = ISNULL(SUM(i.TotalPayable), 0) 
    FROM dbo.tbl_Invoice i 
    WHERE 
        i.TenantId = @TenantId 
        AND i.IsActive = 1; 
 
 
    -- ========================================== 
    -- Total Paid for Active Invoices 
    -- ========================================== 
 
    SELECT 
        @TotalPaid = ISNULL(SUM(ip.PaymentAmount), 0) 
    FROM dbo.tbl_InvoicePayment ip 
 
    INNER JOIN dbo.tbl_Invoice i 
        ON i.InvoiceId = ip.InvoiceId 
 
    WHERE 
        i.TenantId = @TenantId 
        AND i.IsActive = 1 
        AND ip.IsActive = 1
        AND i.PaymentStatus ='Paid'
        AND ip.PaymentStatus='Completed'
 
 
    -- ========================================== 
    -- Check Outstanding Amount 
    -- ========================================== 
 
    IF @TotalPayable > @TotalPaid 
    BEGIN 
        SET @HasOutstandingInvoice = 1; 
    END 

    -- SELECT  @TotalPayable  as TotalPayable, @TotalPaid TotalPaid;

 
    RETURN @HasOutstandingInvoice; 
 
END; 