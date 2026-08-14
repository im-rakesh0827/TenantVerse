CREATE OR ALTER PROCEDURE dbo.IT_SP_UpdateUnit
(
    @UnitId             INT,
    @PropertyId         INT,
    @UnitNumber         NVARCHAR(50),
    @UnitType           NVARCHAR(50) = NULL,
    @FloorNumber        INT = NULL,
    @Bedrooms           INT = NULL,
    @Bathrooms          INT = NULL,
    @Area               DECIMAL(12,2) = NULL,
    @MonthlyRent        DECIMAL(18,2) = NULL,
    @SecurityDeposit    DECIMAL(18,2) = NULL,
    @Status             NVARCHAR(30)
)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.tbl_Unit
        WHERE UnitId = @UnitId
          AND IsActive = 1
    )
    BEGIN
        THROW 50003, 'Unit does not exist or is inactive.', 1;
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.tbl_Property
        WHERE PropertyId = @PropertyId
          AND IsActive = 1
    )
    BEGIN
        THROW 50004, 'Property does not exist or is inactive.', 1;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM dbo.tbl_Unit
        WHERE PropertyId = @PropertyId
          AND UnitNumber = @UnitNumber
          AND UnitId <> @UnitId
          AND IsActive = 1
    )
    BEGIN
        THROW 50005, 'Unit number already exists for this property.', 1;
    END;

    UPDATE dbo.tbl_Unit
    SET
        PropertyId = @PropertyId,
        UnitNumber = @UnitNumber,
        UnitType = @UnitType,
        FloorNumber = @FloorNumber,
        Bedrooms = @Bedrooms,
        Bathrooms = @Bathrooms,
        Area = @Area,
        MonthlyRent = @MonthlyRent,
        SecurityDeposit = @SecurityDeposit,
        Status = @Status,
        ModifiedDate = GETUTCDATE()

    WHERE
        UnitId = @UnitId
        AND IsActive = 1;

    SELECT CAST(@UnitId AS INT) AS UnitId;
END;
GO