CREATE OR ALTER PROCEDURE dbo.IT_SP_DeleteUnit
(
    @UnitId INT,
    @UpdatedBy NVARCHAR(100)
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.tbl_Unit
    SET
        IsActive = 0,
        ModifiedDate = GETDATE()
    WHERE UnitId = @UnitId
      AND IsActive = 1;

    SELECT CAST(@@ROWCOUNT AS INT) AS RowsAffected;
END
GO