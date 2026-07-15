CREATE OR ALTER PROCEDURE IT_SP_GetPropertyById 
(
    @PropertyId INT
)
AS

BEGIN
    SET NOCOUNT ON;
    SELECT
            PropertyId,
            PropertyCode,
            PropertyName,
            OwnerName,
            Email,
            PhoneNumber,
            AddressLine1,
            AddressLine2,
            City,
            State,
            PostalCode,
            Country,
            TotalFloors,
            TotalFlats,
            Description,
            IsActive,
            CreatedOn,
            CreatedBy,
            UpdatedOn,
            UpdatedBy
        FROM tbl_Property
        WHERE PropertyId = @PropertyId
        AND IsActive = 1
        ORDER BY PropertyName ASC;
END
GO


