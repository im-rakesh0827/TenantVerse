CREATE OR ALTER PROCEDURE dbo.IT_SP_CreateUnit
(
    @PropertyId          INT,
    @UnitNumber          NVARCHAR(50),
    @UnitType            NVARCHAR(50) = NULL,
    @FloorNumber         INT = NULL,
    @Bedrooms            INT = NULL,
    @Bathrooms           INT = NULL,
    @Area                DECIMAL(12,2) = NULL,
    @MonthlyRent         DECIMAL(18,2) = NULL,
    @SecurityDeposit     DECIMAL(18,2) = NULL,
    @Status              NVARCHAR(30) = 'Available'
)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.tbl_Property
        WHERE PropertyId = @PropertyId
          AND IsActive = 1
    )
    BEGIN
        THROW 50001, 'Property does not exist or is inactive.', 1;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM dbo.tbl_Unit
        WHERE PropertyId = @PropertyId
          AND UnitNumber = @UnitNumber
          AND IsActive = 1
    )
    BEGIN
        THROW 50002, 'Unit number already exists for this property.', 1;
    END;

    INSERT INTO dbo.tbl_Unit
    (
        PropertyId,
        UnitNumber,
        UnitType,
        FloorNumber,
        Bedrooms,
        Bathrooms,
        Area,
        MonthlyRent,
        SecurityDeposit,
        Status
    )
    VALUES
    (
        @PropertyId,
        @UnitNumber,
        @UnitType,
        @FloorNumber,
        @Bedrooms,
        @Bathrooms,
        @Area,
        @MonthlyRent,
        @SecurityDeposit,
        @Status
    );

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS UnitId;
END;
GO