-- CREATE OR ALTER PROCEDURE dbo.IT_SP_UpdateTenant
-- (
--     @TenantId                INT,
--     @PropertyId              INT,
--     @UnitId                  INT,
--     @FirstName               NVARCHAR(100),
--     @LastName                NVARCHAR(100) = NULL,
--     @Email                   NVARCHAR(200) = NULL,
--     @PhoneNumber             NVARCHAR(20) = NULL,
--     @EmergencyContactName    NVARCHAR(150) = NULL,
--     @EmergencyContactPhone   NVARCHAR(20) = NULL,
--     @LeaseStartDate          DATE = NULL,
--     @LeaseEndDate            DATE = NULL,
--     @MonthlyRent             DECIMAL(18,2) = NULL,
--     @SecurityDeposit         DECIMAL(18,2) = NULL,
--     @Status                  NVARCHAR(30)
-- )
-- AS
-- BEGIN
--     SET NOCOUNT ON;

--     IF NOT EXISTS
--     (
--         SELECT 1
--         FROM dbo.tbl_Tenant
--         WHERE TenantId = @TenantId
--           AND IsActive = 1
--     )
--     BEGIN
--         THROW 50010, 'Tenant does not exist or is inactive.', 1;
--     END;

--     IF NOT EXISTS
--     (
--         SELECT 1
--         FROM dbo.tbl_Property
--         WHERE PropertyId = @PropertyId
--           AND IsActive = 1
--     )
--     BEGIN
--         THROW 50011, 'Property does not exist or is inactive.', 1;
--     END;

--     IF NOT EXISTS
--     (
--         SELECT 1
--         FROM dbo.tbl_Unit
--         WHERE UnitId = @UnitId
--           AND PropertyId = @PropertyId
--           AND IsActive = 1
--     )
--     BEGIN
--         THROW 50012, 'Flat does not exist or does not belong to the selected property.', 1;
--     END;

--     IF NULLIF(LTRIM(RTRIM(@FirstName)), '') IS NULL
--     BEGIN
--         THROW 50013, 'First name is required.', 1;
--     END;

--     IF @LeaseStartDate IS NOT NULL
--        AND @LeaseEndDate IS NOT NULL
--        AND @LeaseEndDate < @LeaseStartDate
--     BEGIN
--         THROW 50014, 'Lease end date cannot be earlier than lease start date.', 1;
--     END;

--     IF @MonthlyRent IS NOT NULL
--        AND @MonthlyRent < 0
--     BEGIN
--         THROW 50015, 'Monthly rent cannot be negative.', 1;
--     END;

--     IF @SecurityDeposit IS NOT NULL
--        AND @SecurityDeposit < 0
--     BEGIN
--         THROW 50016, 'Security deposit cannot be negative.', 1;
--     END;

--     IF @Status NOT IN ('Active', 'Inactive', 'Pending')
--     BEGIN
--         THROW 50017, 'Invalid tenant status.', 1;
--     END;

--     UPDATE dbo.tbl_Tenant
--     SET
--         PropertyId = @PropertyId,
--         UnitId = @UnitId,

--         FirstName = LTRIM(RTRIM(@FirstName)),
--         LastName = NULLIF(LTRIM(RTRIM(@LastName)), ''),

--         Email = NULLIF(LTRIM(RTRIM(@Email)), ''),
--         PhoneNumber = NULLIF(LTRIM(RTRIM(@PhoneNumber)), ''),

--         EmergencyContactName =
--             NULLIF(LTRIM(RTRIM(@EmergencyContactName)), ''),

--         EmergencyContactPhone =
--             NULLIF(LTRIM(RTRIM(@EmergencyContactPhone)), ''),

--         LeaseStartDate = @LeaseStartDate,
--         LeaseEndDate = @LeaseEndDate,

--         MonthlyRent = @MonthlyRent,
--         SecurityDeposit = @SecurityDeposit,

--         Status = @Status,

--         ModifiedDate = GETDATE()

--     WHERE TenantId = @TenantId
--       AND IsActive = 1;

--     SELECT @TenantId AS TenantId;
-- END;
-- GO
















--New Proc, previous was not updating the unit or flat status properly
CREATE OR ALTER PROCEDURE dbo.IT_SP_UpdateTenant
(
    @TenantId                INT,
    @PropertyId              INT,
    @UnitId                  INT,
    @FirstName               NVARCHAR(100),
    @LastName                NVARCHAR(100) = NULL,
    @Email                   NVARCHAR(200) = NULL,
    @PhoneNumber             NVARCHAR(20) = NULL,
    @EmergencyContactName    NVARCHAR(150) = NULL,
    @EmergencyContactPhone   NVARCHAR(20) = NULL,
    @LeaseStartDate          DATE = NULL,
    @LeaseEndDate            DATE = NULL,
    @MonthlyRent             DECIMAL(18,2) = NULL,
    @SecurityDeposit         DECIMAL(18,2) = NULL,
    @Status                  NVARCHAR(30)
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY

        BEGIN TRANSACTION;

        DECLARE @OldUnitId INT;

        -- ==========================================
        -- VALIDATE TENANT
        -- ==========================================

        IF NOT EXISTS
        (
            SELECT 1
            FROM dbo.tbl_Tenant
            WHERE TenantId = @TenantId
              AND IsActive = 1
        )
        BEGIN
            THROW 50010,
                'Tenant does not exist or is inactive.',
                1;
        END;


        -- ==========================================
        -- GET CURRENT UNIT
        -- ==========================================

        SELECT
            @OldUnitId = UnitId
        FROM dbo.tbl_Tenant
        WHERE TenantId = @TenantId
          AND IsActive = 1;


        -- ==========================================
        -- VALIDATE PROPERTY
        -- ==========================================

        IF NOT EXISTS
        (
            SELECT 1
            FROM dbo.tbl_Property
            WHERE PropertyId = @PropertyId
              AND IsActive = 1
        )
        BEGIN
            THROW 50011,
                'Property does not exist or is inactive.',
                1;
        END;


        -- ==========================================
        -- VALIDATE NEW UNIT
        -- ==========================================

        IF NOT EXISTS
        (
            SELECT 1
            FROM dbo.tbl_Unit
            WHERE UnitId = @UnitId
              AND PropertyId = @PropertyId
              AND IsActive = 1
        )
        BEGIN
            THROW 50012,
                'Flat does not exist or does not belong to the selected property.',
                1;
        END;


        -- ==========================================
        -- CHECK NEW UNIT AVAILABILITY
        -- ==========================================

        IF @OldUnitId <> @UnitId
           AND EXISTS
           (
               SELECT 1
               FROM dbo.tbl_Unit
               WHERE UnitId = @UnitId
                 AND Status = 'Occupied'
                 AND IsActive = 1
           )
        BEGIN
            THROW 50018,
                'The selected flat is already occupied.',
                1;
        END;


        -- ==========================================
        -- VALIDATE FIRST NAME
        -- ==========================================

        IF NULLIF(
            LTRIM(RTRIM(@FirstName)),
            ''
        ) IS NULL
        BEGIN
            THROW 50013,
                'First name is required.',
                1;
        END;


        -- ==========================================
        -- VALIDATE LEASE DATES
        -- ==========================================

        IF @LeaseStartDate IS NOT NULL
           AND @LeaseEndDate IS NOT NULL
           AND @LeaseEndDate < @LeaseStartDate
        BEGIN
            THROW 50014,
                'Lease end date cannot be earlier than lease start date.',
                1;
        END;


        -- ==========================================
        -- VALIDATE MONTHLY RENT
        -- ==========================================

        IF @MonthlyRent IS NOT NULL
           AND @MonthlyRent < 0
        BEGIN
            THROW 50015,
                'Monthly rent cannot be negative.',
                1;
        END;


        -- ==========================================
        -- VALIDATE SECURITY DEPOSIT
        -- ==========================================

        IF @SecurityDeposit IS NOT NULL
           AND @SecurityDeposit < 0
        BEGIN
            THROW 50016,
                'Security deposit cannot be negative.',
                1;
        END;


        -- ==========================================
        -- VALIDATE TENANT STATUS
        -- ==========================================

        IF @Status NOT IN
        (
            'Active',
            'Inactive',
            'Pending'
        )
        BEGIN
            THROW 50017,
                'Invalid tenant status.',
                1;
        END;


        -- ==========================================
        -- RELEASE OLD UNIT
        -- ==========================================

        IF @OldUnitId IS NOT NULL
           AND @OldUnitId <> @UnitId
        BEGIN

            UPDATE dbo.tbl_Unit
            SET
                Status = 'Available',
                ModifiedDate = GETDATE()

            WHERE UnitId = @OldUnitId
              AND IsActive = 1;

        END;


        -- ==========================================
        -- OCCUPY NEW UNIT
        -- ==========================================

        IF @OldUnitId <> @UnitId
        BEGIN

            UPDATE dbo.tbl_Unit
            SET
                Status = 'Occupied',
                ModifiedDate = GETDATE()

            WHERE UnitId = @UnitId
              AND IsActive = 1;

        END;


        -- ==========================================
        -- UPDATE TENANT
        -- ==========================================

        UPDATE dbo.tbl_Tenant
        SET
            PropertyId = @PropertyId,
            UnitId = @UnitId,

            FirstName =
                LTRIM(RTRIM(@FirstName)),

            LastName =
                NULLIF(
                    LTRIM(RTRIM(@LastName)),
                    ''
                ),

            Email =
                NULLIF(
                    LTRIM(RTRIM(@Email)),
                    ''
                ),

            PhoneNumber =
                NULLIF(
                    LTRIM(RTRIM(@PhoneNumber)),
                    ''
                ),

            EmergencyContactName =
                NULLIF(
                    LTRIM(RTRIM(@EmergencyContactName)),
                    ''
                ),

            EmergencyContactPhone =
                NULLIF(
                    LTRIM(RTRIM(@EmergencyContactPhone)),
                    ''
                ),

            LeaseStartDate = @LeaseStartDate,
            LeaseEndDate = @LeaseEndDate,

            MonthlyRent = @MonthlyRent,
            SecurityDeposit = @SecurityDeposit,

            Status = @Status,

            ModifiedDate = GETDATE()

        WHERE TenantId = @TenantId
          AND IsActive = 1;


        -- ==========================================
        -- COMMIT
        -- ==========================================

        COMMIT TRANSACTION;


        -- ==========================================
        -- RETURN RESULT
        -- ==========================================

        SELECT
            @TenantId AS TenantId;


    END TRY

    BEGIN CATCH

        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;

    END CATCH
END;
GO