CREATE OR ALTER PROCEDURE dbo.IT_SP_GetAllUnit
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        U.UnitId,
        U.PropertyId,
        P.PropertyName,
        U.UnitNumber,
        U.UnitType,
        U.FloorNumber,
        U.Bedrooms,
        U.Bathrooms,
        U.Area,
        U.MonthlyRent,
        U.SecurityDeposit,
        U.Status,
        U.IsActive,
        U.CreatedDate,
        U.ModifiedDate
    FROM dbo.tbl_Unit U
    INNER JOIN dbo.tbl_Property P
        ON P.PropertyId = U.PropertyId
    WHERE
        U.IsActive = 1
        AND P.IsActive = 1
    ORDER BY
        -- P.PropertyName,
        -- U.UnitNumber;
        U.CreatedDate DESC
END;
GO