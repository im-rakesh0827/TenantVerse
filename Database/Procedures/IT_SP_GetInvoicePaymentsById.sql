CREATE OR ALTER PROCEDURE dbo.IT_SP_GetInvoicePaymentsById
(
    @InvoiceId INT
)
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
    WHERE InvoiceId = @InvoiceId
      AND IsActive = 1
    ORDER BY PaymentDate DESC,
             InvoicePaymentId DESC;
END;
GO