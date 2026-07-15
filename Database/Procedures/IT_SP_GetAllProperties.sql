SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER     PROCEDURE [dbo].[IT_SP_GetAllProperties]
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
    WHERE IsActive = 1
    ORDER BY CreatedOn DESC
    -- ORDER BY PropertyName ASC;
END
GO
