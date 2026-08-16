CREATE OR ALTER PROCEDURE dbo.IT_SP_GetTenantByUnitId
(
    @UnitId INT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        T.TenantId,
        T.PropertyId,
        P.PropertyName,
        T.UnitId,
        U.UnitNumber,

        T.FirstName,
        T.LastName,
        T.Email,
        T.PhoneNumber,

        T.EmergencyContactName,
        T.EmergencyContactPhone,

        T.LeaseStartDate,
        T.LeaseEndDate,

        T.MonthlyRent,
        T.SecurityDeposit,

        T.Status,
        T.IsActive,

        T.CreatedDate,
        T.ModifiedDate

    FROM dbo.tbl_Tenant T

    INNER JOIN dbo.tbl_Property P
        ON P.PropertyId = T.PropertyId

    INNER JOIN dbo.tbl_Unit U
        ON U.UnitId = T.UnitId

    WHERE T.UnitId = @UnitId
      AND T.IsActive = 1

    ORDER BY T.TenantId DESC;
END;
GO