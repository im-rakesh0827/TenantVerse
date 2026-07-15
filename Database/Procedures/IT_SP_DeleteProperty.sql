CREATE OR ALTER PROCEDURE IT_SP_DeleteProperty
(
    @PropertyId INT,
    @UpdatedBy NVARCHAR(100)
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE tbl_Property
    SET
        IsActive = 0,
        UpdatedBy = @UpdatedBy,
        UpdatedOn = GETUTCDATE()
    WHERE PropertyId = @PropertyId
      AND IsActive = 1;

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO
