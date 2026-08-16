-- CREATE   PROCEDURE dbo.IT_SP_DeleteUnit 
-- ( 
--     @UnitId INT, 
--     @UpdatedBy NVARCHAR(100) 
-- ) 
-- AS 
-- BEGIN 
--     SET NOCOUNT ON; 
 
--     UPDATE dbo.tbl_Unit 
--     SET 
--         IsActive = 0, 
--         ModifiedDate = GETDATE() 
--     WHERE UnitId = @UnitId 
--       AND IsActive = 1; 
 
--     SELECT CAST(@@ROWCOUNT AS INT) AS RowsAffected; 
-- END 



CREATE OR ALTER PROCEDURE dbo.IT_SP_DeleteUnit
(
    @UnitId INT,
    @UpdatedBy NVARCHAR(100)
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY

        BEGIN TRANSACTION;

        -- =========================================================
        -- 1. Check Flat exists and is active
        -- =========================================================

        IF NOT EXISTS
        (
            SELECT 1
            FROM dbo.tbl_Unit WITH (UPDLOCK, HOLDLOCK)
            WHERE UnitId = @UnitId
              AND IsActive = 1
        )
        BEGIN
            THROW 50020,
                  'Flat does not exist or is already inactive.',
                  1;
        END;


        -- =========================================================
        -- 2. Check active Tenant
        -- =========================================================

        IF EXISTS
        (
            SELECT 1
            FROM dbo.tbl_Tenant WITH (UPDLOCK, HOLDLOCK)
            WHERE UnitId = @UnitId
              AND IsActive = 1
        )
        BEGIN
            THROW 50021,
                  'Cannot deactivate this flat because it has an active tenant.',
                  1;
        END;


        -- =========================================================
        -- 3. Deactivate Flat
        -- =========================================================

        UPDATE dbo.tbl_Unit
        SET
            IsActive = 0,
            ModifiedDate = GETDATE()
        WHERE UnitId = @UnitId
          AND IsActive = 1;

        DECLARE @RowsAffected INT = @@ROWCOUNT;


        -- =========================================================
        -- 4. Commit
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