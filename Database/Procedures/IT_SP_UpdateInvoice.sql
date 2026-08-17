CREATE OR ALTER PROCEDURE dbo.IT_SP_UpdateInvoice
(
    @InvoiceId              INT,
    @BillingMonth           DATE,
    @InvoiceDate            DATE,
    @DueDate                DATE,
    @DiscountAmount         DECIMAL(18,2),
    @LateFee                DECIMAL(18,2),
    @Notes                  NVARCHAR(1000) = NULL,
    @UpdatedBy              NVARCHAR(200) = NULL,

    @Charges                dbo.InvoiceChargeType READONLY
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY

        BEGIN TRANSACTION;

        ------------------------------------------------------------
        -- 1. Validate invoice
        ------------------------------------------------------------

        IF NOT EXISTS
        (
            SELECT 1
            FROM dbo.tbl_Invoice
            WHERE InvoiceId = @InvoiceId
              AND IsActive = 1
        )
        BEGIN
            THROW 50001, 'Invoice not found or inactive.', 1;
        END;


        ------------------------------------------------------------
        -- 2. Validate charges
        ------------------------------------------------------------

        IF NOT EXISTS
        (
            SELECT 1
            FROM @Charges
        )
        BEGIN
            THROW 50002, 'At least one invoice charge is required.', 1;
        END;


        ------------------------------------------------------------
        -- 3. Validate electricity readings
        ------------------------------------------------------------

        IF EXISTS
        (
            SELECT 1
            FROM @Charges
            WHERE ChargeType = 'Electricity'
              AND
              (
                    PreviousReading IS NULL
                 OR CurrentReading IS NULL
                 OR Rate IS NULL
              )
        )
        BEGIN
            THROW 50003,
                  'Electricity charge requires previous reading, current reading and rate.',
                  1;
        END;


        ------------------------------------------------------------
        -- 4. Validate electricity reading
        ------------------------------------------------------------

        IF EXISTS
        (
            SELECT 1
            FROM @Charges
            WHERE ChargeType = 'Electricity'
              AND CurrentReading < PreviousReading
        )
        BEGIN
            THROW 50004,
                  'Current electricity reading cannot be less than previous reading.',
                  1;
        END;


        ------------------------------------------------------------
        -- 5. Update invoice header
        ------------------------------------------------------------

        UPDATE dbo.tbl_Invoice
        SET
            BillingMonth   = @BillingMonth,
            InvoiceDate    = @InvoiceDate,
            DueDate        = @DueDate,
            DiscountAmount = ISNULL(@DiscountAmount, 0),
            LateFee        = ISNULL(@LateFee, 0),
            Notes          = @Notes,
            UpdatedOn      = GETUTCDATE(),
            UpdatedBy      = @UpdatedBy
        WHERE InvoiceId = @InvoiceId;


        ------------------------------------------------------------
        -- 6. Deactivate existing charges
        ------------------------------------------------------------

        UPDATE dbo.tbl_InvoiceCharge
        SET
            IsActive = 0,
            UpdatedOn = GETUTCDATE(),
            UpdatedBy = @UpdatedBy
        WHERE InvoiceId = @InvoiceId
          AND IsActive = 1;


        ------------------------------------------------------------
        -- 7. Insert new charges
        ------------------------------------------------------------

        INSERT INTO dbo.tbl_InvoiceCharge
        (
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
            CreatedBy
        )
        SELECT
            @InvoiceId,

            ChargeType,

            Description,

            CASE
                WHEN ChargeType = 'Electricity'
                THEN
                    (
                        ISNULL(CurrentReading, 0)
                        -
                        ISNULL(PreviousReading, 0)
                    )
                    *
                    ISNULL(Rate, 0)

                ELSE
                    Amount
            END,

            PreviousReading,

            CurrentReading,

            CASE
                WHEN ChargeType = 'Electricity'
                THEN
                    (
                        ISNULL(CurrentReading, 0)
                        -
                        ISNULL(PreviousReading, 0)
                    )

                ELSE
                    Units
            END,

            Rate,

            1,

            GETUTCDATE(),

            @UpdatedBy

        FROM @Charges;


        ------------------------------------------------------------
        -- 8. Calculate subtotal
        ------------------------------------------------------------

        DECLARE @SubTotal DECIMAL(18,2);

        SELECT
            @SubTotal =
                ISNULL(SUM(Amount), 0)
        FROM dbo.tbl_InvoiceCharge
        WHERE InvoiceId = @InvoiceId
          AND IsActive = 1;


        ------------------------------------------------------------
        -- 9. Calculate total payable
        ------------------------------------------------------------

        DECLARE @TotalPayable DECIMAL(18,2);

        SET @TotalPayable =
              @SubTotal
            - ISNULL(@DiscountAmount, 0)
            + ISNULL(@LateFee, 0);


        ------------------------------------------------------------
        -- 10. Update invoice totals
        ------------------------------------------------------------

        UPDATE dbo.tbl_Invoice
        SET
            SubTotal =
                @SubTotal,

            TotalPayable =
                @TotalPayable,

            UpdatedOn =
                GETUTCDATE(),

            UpdatedBy =
                @UpdatedBy

        WHERE InvoiceId = @InvoiceId;


        ------------------------------------------------------------
        -- 11. Return InvoiceId
        ------------------------------------------------------------

        SELECT
            @InvoiceId AS InvoiceId;


        COMMIT TRANSACTION;

    END TRY

    BEGIN CATCH

        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;

    END CATCH
END;
GO