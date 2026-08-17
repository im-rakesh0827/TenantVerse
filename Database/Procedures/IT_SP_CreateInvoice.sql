CREATE OR ALTER PROCEDURE dbo.IT_SP_CreateInvoice
(
    @PropertyId          INT,
    @UnitId              INT,
    @TenantId            INT,
    @BillingMonth        DATE,
    @InvoiceDate         DATE,
    @DueDate             DATE,

    @MonthlyRent         DECIMAL(18,2),

    @PreviousReading     DECIMAL(18,2),
    @CurrentReading      DECIMAL(18,2),
    @ElectricityRate     DECIMAL(18,2),

    @MaintenanceCharge   DECIMAL(18,2),
    @WaterCharge         DECIMAL(18,2),

    @LateFee             DECIMAL(18,2),
    @Discount            DECIMAL(18,2),

    @Notes               NVARCHAR(1000) = NULL,
    @CreatedBy           NVARCHAR(200) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY

        BEGIN TRANSACTION;


        /* =====================================================
           1. VALIDATE PROPERTY
           ===================================================== */

        IF NOT EXISTS
        (
            SELECT 1
            FROM dbo.tbl_Property
            WHERE PropertyId = @PropertyId
              AND IsActive = 1
        )
        BEGIN
            THROW 50030,
                  'Property does not exist or is inactive.',
                  1;
        END;


        /* =====================================================
           2. VALIDATE FLAT
           ===================================================== */

        IF NOT EXISTS
        (
            SELECT 1
            FROM dbo.tbl_Unit
            WHERE UnitId = @UnitId
              AND PropertyId = @PropertyId
              AND IsActive = 1
        )
        BEGIN
            THROW 50031,
                  'Flat does not exist, is inactive, or does not belong to the selected property.',
                  1;
        END;


        /* =====================================================
           3. VALIDATE TENANT
           ===================================================== */

        IF NOT EXISTS
        (
            SELECT 1
            FROM dbo.tbl_Tenant
            WHERE TenantId = @TenantId
              AND UnitId = @UnitId
              AND PropertyId = @PropertyId
              AND IsActive = 1
        )
        BEGIN
            THROW 50032,
                  'Tenant does not exist, is inactive, or does not belong to the selected flat.',
                  1;
        END;


        /* =====================================================
           4. VALIDATE FLAT IS OCCUPIED
           ===================================================== */

        IF NOT EXISTS
        (
            SELECT 1
            FROM dbo.tbl_Unit
            WHERE UnitId = @UnitId
              AND PropertyId = @PropertyId
              AND IsActive = 1
              AND Status = 'Occupied'
        )
        BEGIN
            THROW 50033,
                  'Cannot create invoice because the selected flat is not occupied.',
                  1;
        END;


        /* =====================================================
           5. VALIDATE BILLING DATA
           ===================================================== */

        IF @MonthlyRent < 0
        BEGIN
            THROW 50034,
                  'Monthly rent cannot be negative.',
                  1;
        END;


        IF @PreviousReading < 0
        BEGIN
            THROW 50035,
                  'Previous electricity reading cannot be negative.',
                  1;
        END;


        IF @CurrentReading < @PreviousReading
        BEGIN
            THROW 50036,
                  'Current electricity reading cannot be less than previous reading.',
                  1;
        END;


        IF @ElectricityRate < 0
        BEGIN
            THROW 50037,
                  'Electricity rate cannot be negative.',
                  1;
        END;


        IF @MaintenanceCharge < 0
        BEGIN
            THROW 50038,
                  'Maintenance charge cannot be negative.',
                  1;
        END;


        IF @WaterCharge < 0
        BEGIN
            THROW 50039,
                  'Water charge cannot be negative.',
                  1;
        END;


        IF @LateFee < 0
        BEGIN
            THROW 50040,
                  'Late fee cannot be negative.',
                  1;
        END;


        /* =====================================================
           6. NORMALIZE DISCOUNT
           
           User can send either:
               100
           or
               -100

           We always store discount as negative.
           ===================================================== */

        IF @Discount > 0
        BEGIN
            SET @Discount = -@Discount;
        END;


        /* =====================================================
           7. PREVENT DUPLICATE INVOICE
           ===================================================== */

        IF EXISTS
        (
            SELECT 1
            FROM dbo.tbl_Invoice
            WHERE TenantId = @TenantId
              AND BillingMonth = DATEFROMPARTS
              (
                  YEAR(@BillingMonth),
                  MONTH(@BillingMonth),
                  1
              )
              AND IsActive = 1
        )
        BEGIN
            THROW 50041,
                  'An active invoice already exists for this tenant and billing month.',
                  1;
        END;


        /* =====================================================
           8. CALCULATE ELECTRICITY
           ===================================================== */

        DECLARE @Units DECIMAL(18,2);

        DECLARE @ElectricityAmount DECIMAL(18,2);

        SET @Units =
            @CurrentReading - @PreviousReading;

        SET @ElectricityAmount =
            @Units * @ElectricityRate;


        /* =====================================================
           9. CALCULATE SUBTOTAL
           ===================================================== */

        DECLARE @SubTotal DECIMAL(18,2);

        SET @SubTotal =
              @MonthlyRent
            + @ElectricityAmount
            + @MaintenanceCharge
            + @WaterCharge;


        /* =====================================================
           10. CALCULATE TOTAL
           ===================================================== */

        DECLARE @TotalPayable DECIMAL(18,2);

        SET @TotalPayable =
              @SubTotal
            + @LateFee
            + @Discount;


        /* =====================================================
           11. GENERATE INVOICE NUMBER
           ===================================================== */

        DECLARE @InvoiceId INT;

        DECLARE @InvoiceNumber NVARCHAR(50);

        SET @InvoiceNumber =
            CONCAT
            (
                'INV-',
                FORMAT(@BillingMonth, 'yyyyMM'),
                '-',
                RIGHT
                (
                    '000000' +
                    CAST
                    (
                        ISNULL
                        (
                            (
                                SELECT MAX(InvoiceId) + 1
                                FROM dbo.tbl_Invoice
                            ),
                            1
                        )
                        AS VARCHAR(10)
                    ),
                    6
                )
            );


        /* =====================================================
           12. CREATE INVOICE HEADER
           ===================================================== */

        INSERT INTO dbo.tbl_Invoice
        (
            InvoiceNumber,
            PropertyId,
            UnitId,
            TenantId,
            BillingMonth,
            InvoiceDate,
            DueDate,
            PaymentStatus,
            SubTotal,
            DiscountAmount,
            LateFee,
            TotalPayable,
            Notes,
            IsActive,
            CreatedOn,
            CreatedBy
        )
        VALUES
        (
            @InvoiceNumber,
            @PropertyId,
            @UnitId,
            @TenantId,

            DATEFROMPARTS
            (
                YEAR(@BillingMonth),
                MONTH(@BillingMonth),
                1
            ),

            @InvoiceDate,
            @DueDate,

            'Pending',

            @SubTotal,
            @Discount,
            @LateFee,
            @TotalPayable,

            @Notes,

            1,

            GETUTCDATE(),
            @CreatedBy
        );


        SET @InvoiceId = SCOPE_IDENTITY();


        /* =====================================================
           13. MONTHLY RENT CHARGE
           ===================================================== */

        INSERT INTO dbo.tbl_InvoiceCharge
        (
            InvoiceId,
            ChargeType,
            Description,
            Amount,
            IsActive,
            CreatedOn,
            CreatedBy
        )
        VALUES
        (
            @InvoiceId,
            'MonthlyRent',
            'Monthly Flat Rent',
            @MonthlyRent,
            1,
            GETUTCDATE(),
            @CreatedBy
        );


        /* =====================================================
           14. ELECTRICITY CHARGE
           ===================================================== */

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
        VALUES
        (
            @InvoiceId,
            'Electricity',
            'Electricity Bill',
            @ElectricityAmount,

            @PreviousReading,
            @CurrentReading,
            @Units,
            @ElectricityRate,

            1,
            GETUTCDATE(),
            @CreatedBy
        );


        /* =====================================================
           15. MAINTENANCE CHARGE
           ===================================================== */

        IF @MaintenanceCharge > 0
        BEGIN
            INSERT INTO dbo.tbl_InvoiceCharge
            (
                InvoiceId,
                ChargeType,
                Description,
                Amount,
                IsActive,
                CreatedOn,
                CreatedBy
            )
            VALUES
            (
                @InvoiceId,
                'Maintenance',
                'Maintenance Charge',
                @MaintenanceCharge,
                1,
                GETUTCDATE(),
                @CreatedBy
            );
        END;


        /* =====================================================
           16. WATER CHARGE
           ===================================================== */

        IF @WaterCharge > 0
        BEGIN
            INSERT INTO dbo.tbl_InvoiceCharge
            (
                InvoiceId,
                ChargeType,
                Description,
                Amount,
                IsActive,
                CreatedOn,
                CreatedBy
            )
            VALUES
            (
                @InvoiceId,
                'Water',
                'Water Charge',
                @WaterCharge,
                1,
                GETUTCDATE(),
                @CreatedBy
            );
        END;


        /* =====================================================
           17. LATE FEE
           ===================================================== */

        IF @LateFee > 0
        BEGIN
            INSERT INTO dbo.tbl_InvoiceCharge
            (
                InvoiceId,
                ChargeType,
                Description,
                Amount,
                IsActive,
                CreatedOn,
                CreatedBy
            )
            VALUES
            (
                @InvoiceId,
                'LateFee',
                'Late Fee',
                @LateFee,
                1,
                GETUTCDATE(),
                @CreatedBy
            );
        END;


        /* =====================================================
           18. DISCOUNT
           ===================================================== */

        IF @Discount <> 0
        BEGIN
            INSERT INTO dbo.tbl_InvoiceCharge
            (
                InvoiceId,
                ChargeType,
                Description,
                Amount,
                IsActive,
                CreatedOn,
                CreatedBy
            )
            VALUES
            (
                @InvoiceId,
                'Discount',
                'Discount',
                @Discount,
                1,
                GETUTCDATE(),
                @CreatedBy
            );
        END;


        /* =====================================================
           19. COMMIT
           ===================================================== */

        COMMIT TRANSACTION;


        /* =====================================================
           20. RETURN
           ===================================================== */

        SELECT
            @InvoiceId AS InvoiceId,
            @InvoiceNumber AS InvoiceNumber,
            @TotalPayable AS TotalPayable;

    END TRY

    BEGIN CATCH

        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;

    END CATCH
END;
GO