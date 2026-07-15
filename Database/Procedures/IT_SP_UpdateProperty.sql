CREATE OR ALTER PROCEDURE IT_SP_UpdateProperty
(
    @PropertyId INT,
    @PropertyCode NVARCHAR(50),
    @PropertyName NVARCHAR(150),
    @OwnerName NVARCHAR(150),
    @Email NVARCHAR(100),
    @PhoneNumber NVARCHAR(20),
    @AddressLine1 NVARCHAR(250),
    @AddressLine2 NVARCHAR(250),
    @City NVARCHAR(100),
    @State NVARCHAR(100),
    @PostalCode NVARCHAR(20),
    @Country NVARCHAR(100),
    @TotalFloors INT,
    @TotalFlats INT,
    @Description NVARCHAR(500),
    @UpdatedBy NVARCHAR(100)
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE tbl_Property
    SET
        PropertyCode = @PropertyCode,
        PropertyName = @PropertyName,
        OwnerName = @OwnerName,
        Email = @Email,
        PhoneNumber = @PhoneNumber,
        AddressLine1 = @AddressLine1,
        AddressLine2 = @AddressLine2,
        City = @City,
        State = @State,
        PostalCode = @PostalCode,
        Country = @Country,
        TotalFloors = @TotalFloors,
        TotalFlats = @TotalFlats,
        Description = @Description,
        UpdatedOn = GETUTCDATE(),
        UpdatedBy = @UpdatedBy
    WHERE PropertyId = @PropertyId
      AND IsActive = 1;

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO