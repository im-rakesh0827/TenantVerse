-- CREATE OR ALTER PROCEDURE dbo.IT_SP_DeleteTenant
-- (
--     @TenantId INT
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
--         THROW 50020, 'Tenant does not exist or is already inactive.', 1;
--     END;

--     UPDATE dbo.tbl_Tenant
--     SET
--         IsActive = 0,
--         Status = 'Inactive',
--         ModifiedDate = GETDATE()

--     WHERE TenantId = @TenantId
--       AND IsActive = 1;

--     SELECT @TenantId AS TenantId;
-- END;
-- GO





CREATE OR ALTER PROCEDURE dbo.IT_SP_DeleteTenant
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