CREATE OR ALTER  PROCEDURE dbo.IT_SP_GetAllInvoicePayments
AS 
BEGIN 
    SET NOCOUNT ON; 
 
    SELECT 
        InvoicePaymentId, 
        InvoiceId, 
        PaymentAmount, 
        PaymentDate, 
        PaymentMethod, 
        PaymentStatus, 
        TransactionReference, 
        Notes, 
        IsActive, 
        CreatedOn, 
        CreatedBy, 
        UpdatedOn, 
        UpdatedBy 
    FROM dbo.tbl_InvoicePayment 
    WHERE IsActive = 1 
    ORDER BY PaymentDate DESC, 
             InvoicePaymentId DESC; 
END; 