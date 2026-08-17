SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY

    BEGIN TRANSACTION;

    PRINT '==============================================';
    PRINT 'TenantVerse Test Data Creation Started';
    PRINT '==============================================';


    /* =========================================================
       0. CLEAN EXISTING PROPERTY / FLAT / TENANT DATA
       ========================================================= */

    PRINT 'Cleaning existing data...';

    DELETE FROM dbo.tbl_Tenant;

    DELETE FROM dbo.tbl_Unit;

    DELETE FROM dbo.tbl_Property;

    PRINT 'Existing data deleted.';


    /* =========================================================
       TEMP TABLES
       ========================================================= */

    CREATE TABLE #SeedProperties
    (
        PropertyId INT NOT NULL,
        PropertyName NVARCHAR(300) NOT NULL
    );

    CREATE TABLE #SeedUnits
    (
        UnitId INT NOT NULL,
        PropertyId INT NOT NULL,
        UnitNumber NVARCHAR(50) NOT NULL
    );


    /* =========================================================
       1. CREATE 50 PROPERTIES
       ========================================================= */

    ;WITH Numbers AS
    (
        SELECT TOP (50)
            ROW_NUMBER() OVER
            (
                ORDER BY (SELECT NULL)
            ) AS Number
        FROM sys.all_objects a
        CROSS JOIN sys.all_objects b
    )
    INSERT INTO dbo.tbl_Property
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
        IsActive,
        CreatedOn,
        CreatedBy
    )
    OUTPUT
        inserted.PropertyId,
        inserted.PropertyName
    INTO #SeedProperties
    (
        PropertyId,
        PropertyName
    )
    SELECT
        CONCAT(
            'UT-PROP-',
            RIGHT(
                '000' + CAST(Number AS VARCHAR(3)),
                3
            )
        ),

        CONCAT(
            'UT Property ',
            RIGHT(
                '000' + CAST(Number AS VARCHAR(3)),
                3
            )
        ),

        CONCAT(
            'Test Owner ',
            RIGHT(
                '000' + CAST(Number AS VARCHAR(3)),
                3
            )
        ),

        CONCAT(
            'owner',
            RIGHT(
                '000' + CAST(Number AS VARCHAR(3)),
                3
            ),
            '@tenantverse.test'
        ),

        CONCAT(
            '90000',
            RIGHT(
                '000' + CAST(Number AS VARCHAR(3)),
                3
            )
        ),

        CONCAT(
            'Test Address ',
            Number,
            ', Main Road'
        ),

        NULL,

        'Darbhanga',

        'Bihar',

        CONCAT(
            '846',
            RIGHT(
                '000' + CAST(Number AS VARCHAR(3)),
                3
            )
        ),

        'India',

        2,

        2,

        CONCAT(
            'Test property created for TenantVerse unit testing - Property ',
            Number
        ),

        1,

        GETUTCDATE(),

        'TestData'


    FROM Numbers;


    PRINT '50 properties created.';


    /* =========================================================
       2. CREATE 100 FLATS
       
       2 flats for every property:
       
           Flat-01
           Flat-02
       ========================================================= */

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
        Status,
        IsActive,
        CreatedDate,
        ModifiedDate
    )
    OUTPUT
        inserted.UnitId,
        inserted.PropertyId,
        inserted.UnitNumber
    INTO #SeedUnits
    (
        UnitId,
        PropertyId,
        UnitNumber
    )
    SELECT
        p.PropertyId,

        CONCAT(
            'Flat-',
            RIGHT(
                '00' + CAST(n.Number AS VARCHAR(2)),
                2
            )
        ),

        CASE
            WHEN n.Number = 1 THEN '2BHK'
            ELSE '1BHK'
        END,

        CASE
            WHEN n.Number = 1 THEN 1
            ELSE 2
        END,

        CASE
            WHEN n.Number = 1 THEN 2
            ELSE 1
        END,

        CASE
            WHEN n.Number = 1 THEN 2
            ELSE 1
        END,

        CASE
            WHEN n.Number = 1 THEN 1100.00
            ELSE 750.00
        END,

        CASE
            WHEN n.Number = 1 THEN 15000.00
            ELSE 10000.00
        END,

        CASE
            WHEN n.Number = 1 THEN 30000.00
            ELSE 20000.00
        END,

        'Available',

        1,

        GETUTCDATE(),

        GETUTCDATE()

    FROM #SeedProperties p
    CROSS JOIN
    (
        SELECT 1 AS Number
        UNION ALL
        SELECT 2
    ) n;


    PRINT '100 flats created.';


    /* =========================================================
       3. CREATE 25 ACTIVE TENANTS
       
       Property 001 -> Flat-01 -> Tenant 01
       Property 002 -> Flat-01 -> Tenant 02
       ...
       Property 025 -> Flat-01 -> Tenant 25
       ========================================================= */

    ;WITH PropertyNumbers AS
    (
        SELECT
            PropertyId,
            PropertyName,
            TRY_CONVERT
            (
                INT,
                RIGHT(PropertyName, 3)
            ) AS PropertyNumber
        FROM #SeedProperties
    ),
    TenantData AS
    (
        SELECT
            p.PropertyId,
            u.UnitId,
            p.PropertyNumber,
            ROW_NUMBER() OVER
            (
                ORDER BY p.PropertyNumber
            ) AS TenantNumber
        FROM PropertyNumbers p
        INNER JOIN #SeedUnits u
            ON u.PropertyId = p.PropertyId
           AND u.UnitNumber = 'Flat-01'
        WHERE p.PropertyNumber BETWEEN 1 AND 25
    )
    INSERT INTO dbo.tbl_Tenant
    (
        PropertyId,
        UnitId,
        FirstName,
        LastName,
        Email,
        PhoneNumber,
        EmergencyContactName,
        EmergencyContactPhone,
        LeaseStartDate,
        LeaseEndDate,
        MonthlyRent,
        SecurityDeposit,
        Status,
        IsActive,
        CreatedDate
    )
    SELECT
        PropertyId,

        UnitId,

        CONCAT(
            'TestTenant',
            RIGHT(
                '00' + CAST(TenantNumber AS VARCHAR(2)),
                2
            )
        ),

        'Kumar',

        CONCAT(
            'tenant',
            RIGHT(
                '00' + CAST(TenantNumber AS VARCHAR(2)),
                2
            ),
            '@tenantverse.test'
        ),

        CONCAT(
            '900000',
            RIGHT(
                '0000' + CAST(TenantNumber AS VARCHAR(4)),
                4
            )
        ),

        CONCAT(
            'Emergency Contact ',
            TenantNumber
        ),

        CONCAT(
            '910000',
            RIGHT(
                '0000' + CAST(TenantNumber AS VARCHAR(4)),
                4
            )
        ),

        DATEADD(
            MONTH,
            -TenantNumber,
            CAST(GETDATE() AS DATE)
        ),

        DATEADD(
            YEAR,
            1,
            CAST(GETDATE() AS DATE)
        ),

        15000.00,

        30000.00,

        'Active',

        1,

        GETUTCDATE()

    FROM TenantData;


    PRINT '25 tenants created.';


    /* =========================================================
       4. MARK TENANT-ASSIGNED FLATS AS OCCUPIED
       ========================================================= */

    UPDATE u
    SET
        u.Status = 'Occupied',
        u.ModifiedDate = GETUTCDATE()
    FROM dbo.tbl_Unit u
    INNER JOIN dbo.tbl_Tenant t
        ON t.UnitId = u.UnitId
    WHERE
        t.IsActive = 1
        AND u.IsActive = 1
        AND u.PropertyId = t.PropertyId;


    PRINT '25 flats marked as Occupied.';


    /* =========================================================
       5. UPDATE PROPERTY TOTALS
       ========================================================= */

    UPDATE p
    SET
        p.TotalFloors = 2,
        p.TotalFlats = 2,
        p.UpdatedOn = GETUTCDATE(),
        p.UpdatedBy = 'TestData'
    FROM dbo.tbl_Property p
    INNER JOIN #SeedProperties sp
        ON sp.PropertyId = p.PropertyId;


    /* =========================================================
       6. DROP TEMP TABLES
       ========================================================= */

    DROP TABLE #SeedUnits;

    DROP TABLE #SeedProperties;


    /* =========================================================
       7. COMMIT
       ========================================================= */

    COMMIT TRANSACTION;


    PRINT '==============================================';
    PRINT 'TenantVerse Test Data Creation Completed';
    PRINT '==============================================';


END TRY

BEGIN CATCH

    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    PRINT '==============================================';
    PRINT 'Test Data Creation Failed';
    PRINT '==============================================';

    PRINT ERROR_MESSAGE();

    THROW;

END CATCH;
GO