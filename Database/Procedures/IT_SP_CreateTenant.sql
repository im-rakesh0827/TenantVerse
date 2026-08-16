CREATE OR ALTER PROCEDURE dbo.IT_SP_CreateTenant
(
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
    @Status                  NVARCHAR(30) = 'Active'
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY

        ------------------------------------------------------------
        -- BEGIN TRANSACTION
        ------------------------------------------------------------

        BEGIN TRANSACTION;


        ------------------------------------------------------------
        -- Validate Property
        ------------------------------------------------------------

        IF NOT EXISTS
        (
            SELECT 1
            FROM dbo.tbl_Property
            WHERE PropertyId = @PropertyId
              AND IsActive = 1
        )
        BEGIN
            THROW 50001,
                  'Property does not exist or is inactive.',
                  1;
        END;


        ------------------------------------------------------------
        -- Validate Flat
        -- Flat must exist and belong to selected property
        ------------------------------------------------------------

        IF NOT EXISTS
        (
            SELECT 1
            FROM dbo.tbl_Unit
            WHERE UnitId = @UnitId
              AND PropertyId = @PropertyId
              AND IsActive = 1
        )
        BEGIN
            THROW 50002,
                  'Flat does not exist or does not belong to the selected property.',
                  1;
        END;


        ------------------------------------------------------------
        -- Validate and Lock Flat
        --
        -- UPDLOCK  = lock the selected row for update
        -- HOLDLOCK = hold the lock until transaction completes
        --
        -- This prevents two users from assigning the same
        -- Available flat at the same time.
        ------------------------------------------------------------

        IF NOT EXISTS
        (
            SELECT 1
            FROM dbo.tbl_Unit WITH (UPDLOCK, HOLDLOCK)
            WHERE UnitId = @UnitId
              AND PropertyId = @PropertyId
              AND IsActive = 1
              AND Status = 'Available'
        )
        BEGIN
            THROW 50003,
                  'Selected flat is not available for tenant assignment.',
                  1;
        END;


        ------------------------------------------------------------
        -- Validate First Name
        ------------------------------------------------------------

        IF NULLIF(LTRIM(RTRIM(@FirstName)), '') IS NULL
        BEGIN
            THROW 50004,
                  'First name is required.',
                  1;
        END;


        ------------------------------------------------------------
        -- Validate Lease Dates
        ------------------------------------------------------------

        IF @LeaseStartDate IS NOT NULL
           AND @LeaseEndDate IS NOT NULL
           AND @LeaseEndDate < @LeaseStartDate
        BEGIN
            THROW 50005,
                  'Lease end date cannot be earlier than lease start date.',
                  1;
        END;


        ------------------------------------------------------------
        -- Validate Monthly Rent
        ------------------------------------------------------------

        IF @MonthlyRent IS NOT NULL
           AND @MonthlyRent < 0
        BEGIN
            THROW 50006,
                  'Monthly rent cannot be negative.',
                  1;
        END;


        ------------------------------------------------------------
        -- Validate Security Deposit
        ------------------------------------------------------------

        IF @SecurityDeposit IS NOT NULL
           AND @SecurityDeposit < 0
        BEGIN
            THROW 50007,
                  'Security deposit cannot be negative.',
                  1;
        END;


        ------------------------------------------------------------
        -- Validate Tenant Status
        ------------------------------------------------------------

        IF @Status NOT IN
        (
            'Active',
            'Inactive',
            'Pending'
        )
        BEGIN
            THROW 50008,
                  'Invalid tenant status.',
                  1;
        END;


        ------------------------------------------------------------
        -- Create Tenant
        ------------------------------------------------------------

        INSERT INTO dbo.tbl_Tenant
        (
            PropertyId,
            UnitId,
            FirstName,
            LastName,
            Email,
            PhoneNumber,
            EmergencyContactName,
            EmergencyContactPhone,
            LeaseStartDate,
            LeaseEndDate,
            MonthlyRent,
            SecurityDeposit,
            Status,
            IsActive,
            CreatedDate
        )
        VALUES
        (
            @PropertyId,
            @UnitId,
            LTRIM(RTRIM(@FirstName)),
            NULLIF(LTRIM(RTRIM(@LastName)), ''),
            NULLIF(LTRIM(RTRIM(@Email)), ''),
            NULLIF(LTRIM(RTRIM(@PhoneNumber)), ''),
            NULLIF(LTRIM(RTRIM(@EmergencyContactName)), ''),
            NULLIF(LTRIM(RTRIM(@EmergencyContactPhone)), ''),
            @LeaseStartDate,
            @LeaseEndDate,
            @MonthlyRent,
            @SecurityDeposit,
            @Status,
            1,
            GETDATE()
        );


        ------------------------------------------------------------
        -- Get newly created TenantId
        ------------------------------------------------------------

        DECLARE @TenantId INT;

        SET @TenantId = CAST(SCOPE_IDENTITY() AS INT);


        ------------------------------------------------------------
        -- Update Flat Status
        -- Available → Occupied
        ------------------------------------------------------------

        UPDATE dbo.tbl_Unit
        SET
            Status = 'Occupied',
            ModifiedDate = GETUTCDATE()
        WHERE UnitId = @UnitId
          AND PropertyId = @PropertyId
          AND IsActive = 1
          AND Status = 'Available';


        ------------------------------------------------------------
        -- Make sure Flat was updated
        ------------------------------------------------------------

        IF @@ROWCOUNT = 0
        BEGIN
            THROW 50009,
                  'Unable to mark the selected flat as occupied.',
                  1;
        END;


        ------------------------------------------------------------
        -- Commit Transaction
        ------------------------------------------------------------

        COMMIT TRANSACTION;


        ------------------------------------------------------------
        -- Return newly created TenantId
        ------------------------------------------------------------

        SELECT @TenantId AS TenantId;

    END TRY

    BEGIN CATCH

        ------------------------------------------------------------
        -- Rollback transaction if anything fails
        ------------------------------------------------------------

        IF @@TRANCOUNT > 0
        BEGIN
            ROLLBACK TRANSACTION;
        END;

        THROW;

    END CATCH;

END;
GO


