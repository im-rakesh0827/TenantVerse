-- CREATE OR ALTER PROCEDURE IT_SP_DeleteProperty
-- (
--     @PropertyId INT,
--     @UpdatedBy NVARCHAR(100)
-- )
-- AS
-- BEGIN
--     SET NOCOUNT ON;

--     UPDATE tbl_Property
--     SET
--         IsActive = 0,
--         UpdatedBy = @UpdatedBy,
--         UpdatedOn = GETUTCDATE()
--     WHERE PropertyId = @PropertyId
--       AND IsActive = 1;

--     SELECT @@ROWCOUNT AS RowsAffected;
-- END
-- GO



CREATE OR ALTER PROCEDURE dbo.IT_SP_DeleteProperty
(
    @PropertyId INT,
    @UpdatedBy NVARCHAR(100)
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY

        BEGIN TRANSACTION;

        -- =========================================================
        -- 1. Check Property exists and is active
        -- =========================================================

        IF NOT EXISTS
        (
            SELECT 1
            FROM dbo.tbl_Property WITH (UPDLOCK, HOLDLOCK)
            WHERE PropertyId = @PropertyId
              AND IsActive = 1
        )
        BEGIN
            THROW 50030,
                  'Property does not exist or is already inactive.',
                  1;
        END;


        -- =========================================================
        -- 2. Check whether Property has active Flats
        -- =========================================================

        IF EXISTS
        (
            SELECT 1
            FROM dbo.tbl_Unit WITH (UPDLOCK, HOLDLOCK)
            WHERE PropertyId = @PropertyId
              AND IsActive = 1
        )
        BEGIN
            THROW 50031,
                  'Cannot deactivate this property because it has active flats.',
                  1;
        END;


        -- =========================================================
        -- 3. Deactivate Property
        -- =========================================================

        UPDATE dbo.tbl_Property
        SET
            IsActive = 0,
            UpdatedBy = @UpdatedBy,
            UpdatedOn = GETUTCDATE()
        WHERE PropertyId = @PropertyId
          AND IsActive = 1;

        DECLARE @RowsAffected INT = @@ROWCOUNT;


        -- =========================================================
        -- 4. Commit transaction
        -- =========================================================

        COMMIT TRANSACTION;


        -- =========================================================
        -- 5. Return result
        -- =========================================================

        SELECT @RowsAffected AS RowsAffected;

    END TRY

    BEGIN CATCH

        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;

    END CATCH
END;
GO