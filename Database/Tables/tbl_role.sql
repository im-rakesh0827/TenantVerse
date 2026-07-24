IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'tbl_role')
BEGIN

    CREATE TABLE tbl_role
    (
        RoleId INT IDENTITY(1,1) PRIMARY KEY,

        RoleCode NVARCHAR(50) NOT NULL,

        RoleName NVARCHAR(100) NOT NULL,

        Description NVARCHAR(500) NULL,

        IsActive BIT NOT NULL
            CONSTRAINT DF_tbl_role_IsActive DEFAULT(1),

        DisplayOrder INT NOT NULL
            CONSTRAINT DF_tbl_role_DisplayOrder DEFAULT(0),

        CreatedBy NVARCHAR(100) NOT NULL
            CONSTRAINT DF_tbl_role_CreatedBy DEFAULT('SYSTEM'),

        CreatedDate DATETIME NOT NULL
            CONSTRAINT DF_tbl_role_CreatedDate DEFAULT(GETDATE()),

        UpdatedBy NVARCHAR(100) NULL,

        UpdatedDate DATETIME NULL,

        IsDeleted BIT NOT NULL
            CONSTRAINT DF_tbl_role_IsDeleted DEFAULT(0),

        CONSTRAINT UQ_tbl_role_RoleCode UNIQUE(RoleCode),

        CONSTRAINT UQ_tbl_role_RoleName UNIQUE(RoleName)

    );

END
GO