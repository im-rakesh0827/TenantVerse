CREATE OR ALTER PROCEDURE dbo.IT_SP_ReverseInvoicePayment
(
    @InvoicePaymentId INT,
    @UpdatedBy NVARCHAR(100) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY

        BEGIN TRANSACTION;

        DECLARE
            @InvoiceId INT,
            @PaymentStatus NVARCHAR(50),
            @TotalPayable DECIMAL(18,2),
            @TotalPaid DECIMAL(18,2),
            @BalanceDue DECIMAL(18,2),
            @NewInvoiceStatus NVARCHAR(50);

        -- ==========================================
        -- GET PAYMENT
        -- ==========================================

        SELECT
            @InvoiceId = InvoiceId,
            @PaymentStatus = PaymentStatus
        FROM dbo.tbl_invoicePayment
        WHERE InvoicePaymentId = @InvoicePaymentId
          AND IsActive = 1;


        -- ==========================================
        -- PAYMENT NOT FOUND
        -- ==========================================

        IF @InvoiceId IS NULL
        BEGIN
            THROW 50001, 'Payment not found.', 1;
        END;


        -- ==========================================
        -- ALREADY REVERSED
        -- ==========================================

        IF @PaymentStatus = 'Reversed'
        BEGIN
            THROW 50002, 'Payment has already been reversed.', 1;
        END;


        -- ==========================================
        -- GET INVOICE
        -- ==========================================

        SELECT
            @TotalPayable = TotalPayable
        FROM dbo.tbl_invoice
        WHERE InvoiceId = @InvoiceId
          AND IsActive = 1;


        IF @TotalPayable IS NULL
        BEGIN
            THROW 50003, 'Invoice not found.', 1;
        END;


        -- ==========================================
        -- REVERSE PAYMENT
        -- ==========================================

        UPDATE dbo.tbl_invoicePayment
        SET
            PaymentStatus = 'Reversed',
            UpdatedOn = GETDATE(),
            UpdatedBy = @UpdatedBy
        WHERE InvoicePaymentId = @InvoicePaymentId;


        -- ==========================================
        -- RECALCULATE COMPLETED PAYMENTS
        -- ==========================================

        SELECT
            @TotalPaid =
                ISNULL(SUM(PaymentAmount), 0)
        FROM dbo.tbl_invoicePayment
        WHERE InvoiceId = @InvoiceId
          AND IsActive = 1
          AND PaymentStatus = 'Completed';


        -- ==========================================
        -- CALCULATE BALANCE
        -- ==========================================

        SET @BalanceDue =
            @TotalPayable - @TotalPaid;


        -- ==========================================
        -- CALCULATE INVOICE STATUS
        -- ==========================================

        IF @TotalPaid <= 0
        BEGIN
            SET @NewInvoiceStatus = 'Pending';
        END
        ELSE IF @TotalPaid >= @TotalPayable
        BEGIN
            SET @NewInvoiceStatus = 'Paid';
        END
        ELSE
        BEGIN
            SET @NewInvoiceStatus = 'Partially Paid';
        END;


        -- ==========================================
        -- UPDATE INVOICE
        -- ==========================================

        UPDATE dbo.tbl_invoice
        SET
            PaymentStatus = @NewInvoiceStatus,
            UpdatedOn = GETDATE(),
            UpdatedBy = @UpdatedBy
        WHERE InvoiceId = @InvoiceId;


        COMMIT TRANSACTION;


        -- ==========================================
        -- RETURN RESULT
        -- ==========================================

        SELECT
            CAST(1 AS BIT) AS IsSuccess,
            'Payment reversed successfully.' AS Message,
            @InvoiceId AS InvoiceId,
            @NewInvoiceStatus AS PaymentStatus,
            @TotalPaid AS TotalPaid,
            @BalanceDue AS BalanceDue;


    END TRY
    BEGIN CATCH

        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;

    END CATCH
END;