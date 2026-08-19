CREATE OR ALTER PROCEDURE dbo.IT_SP_CreateInvoicePayment
(
    @InvoiceId       INT,
    @PaymentAmount   DECIMAL(18,2),
    @PaymentDate     DATE,
    @PaymentMethod   NVARCHAR(50),
    @TransactionReference NVARCHAR(200) = NULL,
    @Notes           NVARCHAR(500) = NULL,
    @CreatedBy       NVARCHAR(100) = NULL
)

AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY

        BEGIN TRANSACTION;

        DECLARE
            @TotalPayable DECIMAL(18,2),
            @TotalPaid DECIMAL(18,2),
            @BalanceDue DECIMAL(18,2),
            @PaymentStatus NVARCHAR(50);

        -- ==========================================
        -- GET INVOICE TOTAL
        -- ==========================================

        SELECT
            @TotalPayable = TotalPayable
        FROM dbo.tbl_invoice
        WHERE InvoiceId = @InvoiceId
          AND IsActive = 1;


        IF @TotalPayable IS NULL
        BEGIN
            THROW 50001, 'Invoice not found.', 1;
        END;


        -- ==========================================
        -- GET ALREADY PAID AMOUNT
        -- ==========================================

        SELECT
            @TotalPaid =
                ISNULL(SUM(PaymentAmount), 0)
        FROM dbo.tbl_invoicePayment
        WHERE InvoiceId = @InvoiceId
          AND IsActive = 1
          AND PaymentStatus = 'Completed';


        -- ==========================================
        -- VALIDATE PAYMENT
        -- ==========================================

        IF @PaymentAmount <= 0
        BEGIN
            THROW 50002, 'Payment amount must be greater than zero.', 1;
        END;


        SET @BalanceDue =
            @TotalPayable - @TotalPaid;


        -- IF @PaymentAmount > @BalanceDue
        -- BEGIN
        --     THROW 50003, 'Payment amount cannot exceed the balance due.', 1;
        -- END;

        IF @PaymentAmount > @BalanceDue
BEGIN
    DECLARE @ErrorMessage NVARCHAR(500);

    SET @ErrorMessage =
        CONCAT(
            'Payment amount cannot exceed the balance due of ₹',
            FORMAT(@BalanceDue, 'N2'),
            '.'
        );

    THROW 50003, @ErrorMessage, 1;
END;


        -- ==========================================
        -- INSERT PAYMENT
        -- ==========================================

        INSERT INTO dbo.tbl_invoicePayment
        (
            InvoiceId,
            PaymentAmount,
            PaymentDate,
            PaymentMethod,
            PaymentStatus,
            Notes,
            CreatedBy,
            CreatedOn,
            IsActive
        )
        VALUES
        (
            @InvoiceId,
            @PaymentAmount,
            @PaymentDate,
            @PaymentMethod,
            'Completed',
            @Notes,
            @CreatedBy,
            GETDATE(),
            1
        );


        -- ==========================================
        -- RECALCULATE TOTAL PAID
        -- ==========================================

        SET @TotalPaid =
            @TotalPaid + @PaymentAmount;


        -- ==========================================
        -- CALCULATE STATUS
        -- ==========================================

        IF @TotalPaid <= 0
        BEGIN
            SET @PaymentStatus = 'Pending';
        END
        ELSE IF @TotalPaid >= @TotalPayable
        BEGIN
            SET @PaymentStatus = 'Paid';
        END
        ELSE
        BEGIN
            SET @PaymentStatus = 'Partially Paid';
        END;


        -- ==========================================
        -- UPDATE INVOICE
        -- ==========================================

        UPDATE dbo.tbl_invoice
        SET
            PaymentStatus = @PaymentStatus,
            UpdatedOn = GETDATE(),
            UpdatedBy = @CreatedBy
        WHERE InvoiceId = @InvoiceId;


        COMMIT TRANSACTION;


        -- ==========================================
        -- RETURN RESULT
        -- ==========================================

        SELECT
            CAST(1 AS BIT) AS IsSuccess,
            'Payment recorded successfully.' AS Message,
            @PaymentStatus AS PaymentStatus,
            @TotalPaid AS TotalPaid,
            (@TotalPayable - @TotalPaid) AS BalanceDue;


    END TRY
    BEGIN CATCH

        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;

    END CATCH
END;