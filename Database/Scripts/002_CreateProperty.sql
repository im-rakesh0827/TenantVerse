DECLARE @i INT = 1;

WHILE @i <= 10000
BEGIN
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
        CONCAT('PROP-', @i),
        CONCAT('Sample Property ', @i),
        CONCAT('Owner ', @i),
        CONCAT('owner', @i, '@example.com'),
        CONCAT('99999', RIGHT('0000' + CAST(@i AS VARCHAR(4)), 4)),
        CONCAT('Street ', @i),
        NULL,
        'Bihārīganj',
        'Bihar',
        '852215',
        'India',
        5,
        20,
        CONCAT('Sample description for property ', @i),
        GETUTCDATE(),
        'SeedScript',
        1
    );

    SET @i = @i + 1;
END;