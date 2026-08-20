SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER  PROCEDURE [dbo].[IT_SP_DeleteTenant]
(
    @TenantId INT
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY

        BEGIN TRANSACTION;

        DECLARE @UnitId INT;


        -- =========================================================
        -- 1. Get active tenant and its Flat
        -- =========================================================

        SELECT
            @UnitId = UnitId
        FROM dbo.tbl_Tenant WITH (UPDLOCK, HOLDLOCK)
        WHERE TenantId = @TenantId
          AND IsActive = 1;


        -- =========================================================
        -- 2. Validate tenant
        -- =========================================================

        IF @UnitId IS NULL
        BEGIN
            THROW 50020,
                  'Tenant does not exist or is already inactive.',
                  1;
        END;


        -- Checking if there are invoices which are not yet paid
        -- IF dbo.IT_FN_HasOutstandingTenantInvoices(@TenantId) = 1
        -- BEGIN
        --     THROW 50001,
        --         'Tenant cannot be deleted because there are outstanding invoice dues.',
        --         1;
        -- END;



        DECLARE @TotalPayable DECIMAL(18, 2) = 0;
DECLARE @TotalPaid DECIMAL(18, 2) = 0;
DECLARE @TotalDue DECIMAL(18, 2) = 0;
DECLARE @ErrorMessage NVARCHAR(2048);


IF dbo.IT_FN_HasOutstandingTenantInvoices(@TenantId) = 1
BEGIN

    -- Total Payable
    SELECT
        @TotalPayable = ISNULL(SUM(i.TotalPayable), 0)
    FROM dbo.tbl_Invoice i
    WHERE
        i.TenantId = @TenantId
        AND i.IsActive = 1;


    -- Total Paid
    SELECT
        @TotalPaid = ISNULL(SUM(ip.PaymentAmount), 0)
    FROM dbo.tbl_InvoicePayment ip

    INNER JOIN dbo.tbl_Invoice i
        ON i.InvoiceId = ip.InvoiceId

    WHERE
        i.TenantId = @TenantId
        AND i.IsActive = 1
        AND ip.IsActive = 1
        AND i.PaymentStatus='Paid'
        AND ip.PaymentStatus='Completed';


    -- Total Due
    SET @TotalDue = @TotalPayable - @TotalPaid;


    -- Build error message
    SET @ErrorMessage =
        'Tenant cannot be deleted. ' +
        'Total Payable: ' + CAST(@TotalPayable AS VARCHAR(20)) +
        ', Total Paid: ' + CAST(@TotalPaid AS VARCHAR(20)) +
        ', Total Due: ' + CAST(@TotalDue AS VARCHAR(20)) +
        '. Please settle the outstanding amount before deleting the tenant.';


    THROW 50001, @ErrorMessage, 1;

END;



        -- =========================================================
        -- 3. Deactivate Tenant
        -- =========================================================

        UPDATE dbo.tbl_Tenant
        SET
            IsActive = 0,
            Status = 'Inactive',
            ModifiedDate = GETDATE()
        WHERE TenantId = @TenantId
          AND IsActive = 1;


        -- =========================================================
        -- 4. Make Flat Available
        -- =========================================================

        UPDATE dbo.tbl_Unit
        SET
            Status = 'Available',
            ModifiedDate = GETDATE()
        WHERE UnitId = @UnitId
          AND IsActive = 1;


        -- =========================================================
        -- 5. Commit
        -- =========================================================

        COMMIT TRANSACTION;


        -- =========================================================
        -- 6. Return TenantId
        -- =========================================================

        SELECT @TenantId AS TenantId;

    END TRY

    BEGIN CATCH

        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;

    END CATCH
END;
GO
