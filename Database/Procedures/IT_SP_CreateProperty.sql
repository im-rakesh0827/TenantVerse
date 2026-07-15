SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[IT_SP_CreateProperty]
(
    @PropertyCode    NVARCHAR(50),
    @PropertyName    NVARCHAR(150),
    @OwnerName       NVARCHAR(150),
    @Email           NVARCHAR(100),
    @PhoneNumber     NVARCHAR(20),
    @AddressLine1    NVARCHAR(250),
    @AddressLine2    NVARCHAR(250),
    @City            NVARCHAR(100),
    @State           NVARCHAR(100),
    @PostalCode      NVARCHAR(20),
    @Country         NVARCHAR(100),
    @TotalFloors     INT,
    @TotalFlats      INT,
    @Description     NVARCHAR(500),
    @CreatedBy       NVARCHAR(100)
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO tbl_Property
    (
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
        CreatedOn,
        CreatedBy,
        IsActive
    )
    VALUES
    (
        @PropertyCode,
        @PropertyName,
        @OwnerName,
        @Email,
        @PhoneNumber,
        @AddressLine1,
        @AddressLine2,
        @City,
        @State,
        @PostalCode,
        @Country,
        @TotalFloors,
        @TotalFlats,
        @Description,
        GETUTCDATE(),
        @CreatedBy,
        1
    );

    -- Return the new identity value (if PropertyId is an IDENTITY)
    SELECT SCOPE_IDENTITY() AS PropertyId;
END
GO
