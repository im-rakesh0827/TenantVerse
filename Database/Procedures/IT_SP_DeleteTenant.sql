CREATE OR ALTER PROCEDURE dbo.IT_SP_DeleteTenant
(
    @TenantId INT
)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.tbl_Tenant
        WHERE TenantId = @TenantId
          AND IsActive = 1
    )
    BEGIN
        THROW 50020, 'Tenant does not exist or is already inactive.', 1;
    END;

    UPDATE dbo.tbl_Tenant
    SET
        IsActive = 0,
        Status = 'Inactive',
        ModifiedDate = GETDATE()

    WHERE TenantId = @TenantId
      AND IsActive = 1;

    SELECT @TenantId AS TenantId;
END;
GO