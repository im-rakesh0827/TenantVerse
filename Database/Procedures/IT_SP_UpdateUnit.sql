CREATE OR ALTER PROCEDURE dbo.IT_SP_UpdateUnit
(
    @UnitId             INT,
    @PropertyId         INT,
    @UnitNumber         NVARCHAR(50),
    @UnitType           NVARCHAR(50) = NULL,
    @FloorNumber        INT = NULL,
    @Bedrooms           INT = NULL,
    @Bathrooms          INT = NULL,
    @Area               DECIMAL(12,2) = NULL,
    @MonthlyRent        DECIMAL(18,2) = NULL,
    @SecurityDeposit    DECIMAL(18,2) = NULL,
    @Status             NVARCHAR(30)
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY

        BEGIN TRANSACTION;

        -- =========================================================
        -- 1. Validate Unit
        -- =========================================================

        IF NOT EXISTS
        (
            SELECT 1
            FROM dbo.tbl_Unit WITH (UPDLOCK, HOLDLOCK)
            WHERE UnitId = @UnitId
              AND IsActive = 1
        )
        BEGIN
            THROW 50003,
                  'Unit does not exist or is inactive.',
                  1;
        END;


        -- =========================================================
        -- 2. Validate Property
        -- =========================================================

        IF NOT EXISTS
        (
            SELECT 1
            FROM dbo.tbl_Property
            WHERE PropertyId = @PropertyId
              AND IsActive = 1
        )
        BEGIN
            THROW 50004,
                  'Property does not exist or is inactive.',
                  1;
        END;


        -- =========================================================
        -- 3. Validate Unit Number
        -- =========================================================

        IF NULLIF(LTRIM(RTRIM(@UnitNumber)), '') IS NULL
        BEGIN
            THROW 50009,
                  'Unit number is required.',
                  1;
        END;


        -- =========================================================
        -- 4. Check Duplicate Unit Number
        -- =========================================================

        IF EXISTS
        (
            SELECT 1
            FROM dbo.tbl_Unit
            WHERE PropertyId = @PropertyId
              AND UnitNumber = LTRIM(RTRIM(@UnitNumber))
              AND UnitId <> @UnitId
              AND IsActive = 1
        )
        BEGIN
            THROW 50005,
                  'Unit number already exists for this property.',
                  1;
        END;


        -- =========================================================
        -- 5. Validate Status
        -- =========================================================

        IF @Status NOT IN
        (
            'Available',
            'Occupied',
            'Maintenance'
        )
        BEGIN
            THROW 50006,
                  'Invalid unit status.',
                  1;
        END;


        -- =========================================================
        -- 6. Check Active Tenant
        -- =========================================================

        DECLARE @HasActiveTenant BIT = 0;

        IF EXISTS
        (
            SELECT 1
            FROM dbo.tbl_Tenant WITH (UPDLOCK, HOLDLOCK)
            WHERE UnitId = @UnitId
              AND IsActive = 1
        )
        BEGIN
            SET @HasActiveTenant = 1;
        END;


        -- =========================================================
        -- 7. Occupancy Rules
        -- =========================================================

        -- Active tenant exists.
        -- Flat must remain Occupied.

        IF @HasActiveTenant = 1
           AND @Status <> 'Occupied'
        BEGIN
            THROW 50007,
                  'Cannot change flat status because it has an active tenant. Deactivate the tenant first.',
                  1;
        END;


        -- No active tenant.
        -- Flat cannot be manually marked Occupied.

        IF @HasActiveTenant = 0
           AND @Status = 'Occupied'
        BEGIN
            THROW 50008,
                  'Cannot mark flat as Occupied because it has no active tenant.',
                  1;
        END;


        -- =========================================================
        -- 8. Validate Numeric Values
        -- =========================================================

        IF @FloorNumber IS NOT NULL
           AND @FloorNumber < 0
        BEGIN
            THROW 50010,
                  'Floor number cannot be negative.',
                  1;
        END;


        IF @Bedrooms IS NOT NULL
           AND @Bedrooms < 0
        BEGIN
            THROW 50011,
                  'Bedrooms cannot be negative.',
                  1;
        END;


        IF @Bathrooms IS NOT NULL
           AND @Bathrooms < 0
        BEGIN
            THROW 50012,
                  'Bathrooms cannot be negative.',
                  1;
        END;


        IF @Area IS NOT NULL
           AND @Area < 0
        BEGIN
            THROW 50013,
                  'Area cannot be negative.',
                  1;
        END;


        IF @MonthlyRent IS NOT NULL
           AND @MonthlyRent < 0
        BEGIN
            THROW 50014,
                  'Monthly rent cannot be negative.',
                  1;
        END;


        IF @SecurityDeposit IS NOT NULL
           AND @SecurityDeposit < 0
        BEGIN
            THROW 50015,
                  'Security deposit cannot be negative.',
                  1;
        END;


        -- =========================================================
        -- 9. Update Unit
        -- =========================================================

        UPDATE dbo.tbl_Unit
        SET
            PropertyId = @PropertyId,
            UnitNumber = LTRIM(RTRIM(@UnitNumber)),
            UnitType = NULLIF(LTRIM(RTRIM(@UnitType)), ''),
            FloorNumber = @FloorNumber,
            Bedrooms = @Bedrooms,
            Bathrooms = @Bathrooms,
            Area = @Area,
            MonthlyRent = @MonthlyRent,
            SecurityDeposit = @SecurityDeposit,
            Status = @Status,
            ModifiedDate = GETUTCDATE()
        WHERE UnitId = @UnitId
          AND IsActive = 1;


        -- =========================================================
        -- 10. Verify Update
        -- =========================================================

        IF @@ROWCOUNT = 0
        BEGIN
            THROW 50016,
                  'Flat could not be updated.',
                  1;
        END;


        -- =========================================================
        -- 11. Commit
        -- =========================================================

        COMMIT TRANSACTION;


        -- =========================================================
        -- 12. Return UnitId
        -- =========================================================

        SELECT CAST(@UnitId AS INT) AS UnitId;

    END TRY

    BEGIN CATCH

        IF @@TRANCOUNT > 0
        BEGIN
            ROLLBACK TRANSACTION;
        END;

        THROW;

    END CATCH
END;
GO