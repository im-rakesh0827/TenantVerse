IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'tbl_users')
BEGIN

    CREATE TABLE tbl_users
    (
        UserId INT IDENTITY(1,1) PRIMARY KEY,

        UserCode NVARCHAR(50) NOT NULL,

        FirstName NVARCHAR(100) NOT NULL,

        LastName NVARCHAR(100) NULL,

        Email NVARCHAR(200) NOT NULL,

        PhoneNumber NVARCHAR(20) NULL,

        PasswordHash NVARCHAR(MAX) NOT NULL,

        ProfileImage NVARCHAR(500) NULL,

        IsActive BIT NOT NULL
            CONSTRAINT DF_tbl_users_IsActive DEFAULT(1),

        IsEmailVerified BIT NOT NULL
            CONSTRAINT DF_tbl_users_IsEmailVerified DEFAULT(0),

        IsLocked BIT NOT NULL
            CONSTRAINT DF_tbl_users_IsLocked DEFAULT(0),

        FailedLoginAttempts INT NOT NULL
            CONSTRAINT DF_tbl_users_FailedLoginAttempts DEFAULT(0),

        LockoutEnd DATETIME NULL,

        LastLogin DATETIME NULL,

        PasswordChangedDate DATETIME NULL,

        CreatedBy NVARCHAR(100) NOT NULL
            CONSTRAINT DF_tbl_users_CreatedBy DEFAULT('SYSTEM'),

        CreatedDate DATETIME NOT NULL
            CONSTRAINT DF_tbl_users_CreatedDate DEFAULT(GETDATE()),

        UpdatedBy NVARCHAR(100) NULL,

        UpdatedDate DATETIME NULL,

        IsDeleted BIT NOT NULL
            CONSTRAINT DF_tbl_users_IsDeleted DEFAULT(0),

        CONSTRAINT UQ_tbl_users_UserCode UNIQUE(UserCode),

        CONSTRAINT UQ_tbl_users_Email UNIQUE(Email)
    );

END
GO